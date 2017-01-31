using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.IO;
using System.Threading;
using System.Data;

namespace DBLayer
{
    public class SqlServerDataLoader : DataLoader
    {
        // Fields
        private SqlConnection conn;
        private DataFileInfo flInfo;
        private int intCounter;
        private int intExitCode;
        private int intLineNumber;
        private int[] intRecordsSaved;
        private int intThreadsCount;
        private int intTotalCols;
        private int intTotalRecords;
        private List<ErrorInfo> lstSkippedLines;
        private List<ThreadInfo> lstThreadInfo;
        private StreamReader sr;
        private string strCommandText;
        private string strConnString;
        private string strErrorFilePath;
        private string strErrorMsg;
        private string[] strNulls;
        private Thread[] threads;
        private ThreadPriority tPriority;
        private SqlTransaction trn;

        // Methods
        private SqlServerDataLoader()
        {
            this.lstSkippedLines = new List<ErrorInfo>();
            this.strErrorFilePath = "";
            this.lstThreadInfo = new List<ThreadInfo>();
        }

        private SqlServerDataLoader(string ConnectionString, DataFileInfo FileInfo, int ThreadsCount, ThreadPriority Priority)
        {
            this.lstSkippedLines = new List<ErrorInfo>();
            this.strErrorFilePath = "";
            this.lstThreadInfo = new List<ThreadInfo>();
            if (TestConnection(ConnectionString))
            {
                if (!File.Exists(FileInfo.FilePath))
                {
                    this.strErrorMsg = "Input file does not exist.";
                    this.intExitCode = 1;
                }
                else
                {
                    this.strConnString = ConnectionString;
                    this.conn = new SqlConnection(ConnectionString);
                    this.flInfo = FileInfo;
                    this.strErrorFilePath = FileInfo.FilePath.Substring(0, FileInfo.FilePath.LastIndexOf(@"\")) + @"\Error_" + DateTime.Now.ToString("ddMMyyHHmmss") + "_" + new FileInfo(FileInfo.FilePath).Name;
                    this.intThreadsCount = ThreadsCount;
                    this.intRecordsSaved = new int[ThreadsCount];
                    if (FileInfo.IsLengthDelimitedFile)
                    {
                        this.strCommandText = PrepareInsertQuery(this.flInfo.StagingTableName, ConnectionString, this.flInfo.LengthDelimitedColMappingList, out this.intTotalCols, out this.strErrorMsg);
                    }
                    else if (FileInfo.ConsiderAllColumns)
                    {
                        this.strCommandText = PrepareInsertQuery(this.flInfo.StagingTableName, ConnectionString, out this.intTotalCols, out this.strErrorMsg);
                    }
                    else
                    {
                        this.strCommandText = PrepareInsertQuery(this.flInfo.StagingTableName, ConnectionString, this.flInfo.MappingList, out this.intTotalCols, out this.strErrorMsg);
                    }
                    if (this.strErrorMsg != "")
                    {
                        this.intExitCode = 1;
                    }
                    else
                    {
                        this.intExitCode = 0;
                        this.strErrorMsg = "";
                        this.strNulls = new string[this.intTotalCols];
                        for (int i = 0; i < this.intTotalCols; i++)
                        {
                            this.strNulls[i] = "NULL";
                        }
                        this.threads = new Thread[this.intThreadsCount];
                        this.sr = new StreamReader(this.flInfo.FilePath);
                        Thread.CurrentThread.Priority = Priority;
                        this.tPriority = Priority;
                        if (this.flInfo.LoadType == LoadTypes.Truncate)
                        {
                            TruncateData(this.flInfo.StagingTableName, ConnectionString);
                        }
                    }
                }
            }
            else
            {
                this.strErrorMsg = "Connection Failed.";
                this.intExitCode = 1;
            }
        }

        public static DataLoader CreateDataLoader(string ConnectionString, DataFileInfo FileInfo, int ThreadsCount, ThreadPriority Priority) =>
            new SqlServerDataLoader(ConnectionString, FileInfo, ThreadsCount, Priority);

        private static string[] LineToArray(string strData, string strDelimiter, string strEnclosing)
        {
            string[] strArray = strData.Split(new string[] { strDelimiter }, StringSplitOptions.None);
            string str = "";
            bool flag = false;
            string str2 = "";
            for (int i = 0; i < strArray.Length; i++)
            {
                if (strArray[i].IndexOf(strEnclosing) == -1)
                {
                    if (flag)
                    {
                        str2 = str2 + strArray[i] + strDelimiter;
                    }
                    else
                    {
                        str2 = "";
                        str = str + strArray[i] + "\n";
                    }
                }
                else if (strArray[i].Trim().IndexOf(strEnclosing) == 0)
                {
                    if (flag)
                    {
                        string str3 = strArray[i];
                        str3 = str3.Substring(str3.IndexOf(strEnclosing) + 1);
                        if (str3.Length == 0)
                        {
                            str = str + str2 + "\n";
                            flag = false;
                        }
                        else if (str3.Trim().LastIndexOf(strEnclosing) == (str3.Trim().Length - 1))
                        {
                            str = str + str2 + strArray[i].Substring(0, strArray[i].LastIndexOf(strEnclosing)) + "\n";
                            flag = false;
                        }
                        else
                        {
                            str2 = str2 + str3 + strDelimiter;
                        }
                    }
                    else if ((strArray[i].Trim().LastIndexOf(strEnclosing) == (strArray[i].Trim().Length - 1)) && (strArray[i].Length >= (strEnclosing + strEnclosing).Length))
                    {
                        strArray[i] = strArray[i].Substring(strArray[i].IndexOf(strEnclosing) + 1);
                        strArray[i] = strArray[i].Substring(0, strArray[i].LastIndexOf(strEnclosing));
                        str = str + strArray[i] + "\n";
                    }
                    else
                    {
                        str2 = strArray[i].Substring(1) + strDelimiter;
                        flag = true;
                    }
                }
                else if (strArray[i].Trim().IndexOf("\"") == (strArray[i].Trim().Length - 1))
                {
                    strArray[i] = strArray[i].Substring(0, strArray[i].LastIndexOf(strEnclosing));
                    str = str + str2 + strArray[i] + "\n";
                    flag = false;
                }
            }
            if (str.Length > 0)
            {
                str = str.Substring(0, str.Length - 1);
            }
            return str.Split(new string[] { "\n" }, StringSplitOptions.None);
        }

        private static bool LineToArray(string strData, int SpecifiedLength, List<LengthDelimitedColumnMapping> lstLColMapping, bool blnTrailingNullCols)
        {
            int count = lstLColMapping.Count;
            if (!blnTrailingNullCols && (SpecifiedLength > strData.Length))
            {
                return false;
            }
            foreach (LengthDelimitedColumnMapping mapping in lstLColMapping)
            {
                mapping.Value = strData.Substring(mapping.StartIndex, mapping.Length);
            }
            return true;
        }

        private static string PrepareInsertQuery(string strTableName, string ConnectionString, out int TotalColumns, out string ErrorMsg)
        {
            string str3;
            DataSet dataSet = new DataSet();
            DataTable table = new DataTable();
            string str2 = "";
            SqlConnection connection = new SqlConnection(ConnectionString);
            TotalColumns = 0;
            ErrorMsg = "";
            try
            {
                SqlCommand selectCommand = new SqlCommand("SP_HELP '" + strTableName + "'", connection);
                SqlDataAdapter adapter = new SqlDataAdapter(selectCommand);
                try
                {
                    adapter.Fill(dataSet);
                }
                catch
                {
                    ErrorMsg = "Invalid Table Name. Aborting...";
                    return "";
                }
                if (((dataSet != null) && (dataSet.Tables.Count >= 2)) && (dataSet.Tables[1].Rows.Count > 0))
                {
                    table = dataSet.Tables[1];
                }
                else
                {
                    table = null;
                }
                if (table == null)
                {
                    ErrorMsg = "Invalid Table Name. Aborting...";
                    return "";
                }
                TotalColumns = table.Rows.Count;
                foreach (DataRow row in table.Rows)
                {
                    str2 = str2 + row["COLUMN_NAME"].ToString().ToUpper() + ",";
                }
                if (str2.Length > 0)
                {
                    str2 = str2.Substring(0, str2.Length - 1);
                }
                str3 = "INSERT INTO " + strTableName.ToUpper() + " ( " + str2 + " ) VALUES ( VALUEPART ) ";
            }
            catch (SqlException exception)
            {
                throw exception;
            }
            return str3;
        }

        private static string PrepareInsertQuery(string strTableName, string ConnectionString, List<LengthDelimitedColumnMapping> lstColMapping, out int TotalColumns, out string ErrorMsg)
        {
            string str3;
            DataSet dataSet = new DataSet();
            DataTable table = new DataTable();
            string str2 = "";
            TotalColumns = 0;
            ErrorMsg = "";
            SqlConnection connection = new SqlConnection(ConnectionString);
            try
            {
                SqlCommand selectCommand = new SqlCommand("SP_HELP '" + strTableName + "'", connection);
                SqlDataAdapter adapter = new SqlDataAdapter(selectCommand);
                try
                {
                    adapter.Fill(dataSet);
                }
                catch
                {
                    ErrorMsg = "Invalid Table Name. Aborting...";
                    return "";
                }
                if (((dataSet != null) && (dataSet.Tables.Count >= 2)) && (dataSet.Tables[1].Rows.Count > 0))
                {
                    table = dataSet.Tables[1];
                }
                else
                {
                    table = null;
                }
                if (table == null)
                {
                    ErrorMsg = "Invalid Table Name. Aborting...";
                    return "";
                }
                List<string> list = new List<string>();
                foreach (DataRow row in table.Rows)
                {
                    list.Add(row["COLUMN_NAME"].ToString().ToUpper());
                }
                TotalColumns = lstColMapping.Count;
                foreach (LengthDelimitedColumnMapping mapping in lstColMapping)
                {
                    if (!list.Contains(mapping.MappedColumnName))
                    {
                        ErrorMsg = "Invalid Column Name. Aborting...";
                        return "";
                    }
                    str2 = str2 + mapping.MappedColumnName + ",";
                }
                if (str2.Length > 0)
                {
                    str2 = str2.Substring(0, str2.Length - 1);
                }
                str3 = "INSERT INTO " + strTableName.ToUpper() + " ( " + str2 + " ) VALUES ( VALUEPART ) ";
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return str3;
        }

        private static string PrepareInsertQuery(string strTableName, string ConnectionString, List<SrcToDestColumnMapping> lstColMapping, out int TotalColumns, out string ErrorMsg)
        {
            string str3;
            DataSet dataSet = new DataSet();
            DataTable table = new DataTable();
            string str2 = "";
            TotalColumns = 0;
            ErrorMsg = "";
            SqlConnection connection = new SqlConnection(ConnectionString);
            try
            {
                SqlCommand selectCommand = new SqlCommand("SP_HELP '" + strTableName + "'", connection);
                SqlDataAdapter adapter = new SqlDataAdapter(selectCommand);
                try
                {
                    adapter.Fill(dataSet);
                }
                catch
                {
                    ErrorMsg = "Invalid Table Name. Aborting...";
                    return "";
                }
                if (((dataSet != null) && (dataSet.Tables.Count >= 2)) && (dataSet.Tables[1].Rows.Count > 0))
                {
                    table = dataSet.Tables[1];
                }
                else
                {
                    table = null;
                }
                if (table == null)
                {
                    ErrorMsg = "Invalid Table Name. Aborting...";
                    return "";
                }
                List<string> list = new List<string>();
                foreach (DataRow row in table.Rows)
                {
                    list.Add(row["COLUMN_NAME"].ToString().ToUpper());
                }
                TotalColumns = lstColMapping.Count;
                foreach (SrcToDestColumnMapping mapping in lstColMapping)
                {
                    if (!list.Contains(mapping.DestinationColumn))
                    {
                        ErrorMsg = "Invalid Column Name. Aborting...";
                        return "";
                    }
                    str2 = str2 + mapping.DestinationColumn + ",";
                }
                if (str2.Length > 0)
                {
                    str2 = str2.Substring(0, str2.Length - 1);
                }
                str3 = "INSERT INTO " + strTableName.ToUpper() + " ( " + str2 + " ) VALUES ( VALUEPART ) ";
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return str3;
        }

        private void ReadLineAndLoadData(object ThreadNumber)
        {
            TimeSpan span;
            DateTime now = DateTime.Now;
            int recordsProcessed = 0;
            string str = null;
            string newValue = "";
            int lineNumber = 0;
            int index = Convert.ToInt32(ThreadNumber);
            bool considerAllColumns = this.flInfo.ConsiderAllColumns;
            lock (this.sr)
            {
                if (((this.intLineNumber == 0) && this.flInfo.HasHeader) && !this.sr.EndOfStream)
                {
                    this.sr.ReadLine();
                }
                else if ((this.intLineNumber == 0) && (this.flInfo.StartingLinesToSkip > 0))
                {
                    for (int i = 0; (i < this.flInfo.StartingLinesToSkip) && !this.sr.EndOfStream; i++)
                    {
                        this.sr.ReadLine();
                        this.intLineNumber++;
                    }
                }
                if (this.sr.EndOfStream)
                {
                    return;
                }
                str = this.sr.ReadLine().Replace("'", "''");
                lineNumber = ++this.intLineNumber;
                goto Label_0793;
            }
        Label_010C:
            if (string.IsNullOrEmpty(str))
            {
                lock (this.sr)
                {
                    if (this.sr.EndOfStream)
                    {
                        goto Label_0799;
                    }
                    str = this.sr.ReadLine().Replace("'", "''");
                    lineNumber = ++this.intLineNumber;
                    goto Label_0793;
                }
            }
            string strCommandText = this.strCommandText;
            newValue = "";
            string[] strArray = new string[0];
            if (this.flInfo.IsLengthDelimitedFile)
            {
                if (LineToArray(str, this.flInfo.SpecifiedLength, this.flInfo.LengthDelimitedColMappingList, this.flInfo.ConsiderTrailingNullCols))
                {
                    goto Label_0305;
                }
                this.lstSkippedLines.Add(new ErrorInfo(lineNumber, str, "Invalid Character Positioning specified."));
                recordsProcessed++;
                lock (this.sr)
                {
                    if (this.sr.EndOfStream)
                    {
                        goto Label_0799;
                    }
                    str = this.sr.ReadLine().Replace("'", "''");
                    lineNumber = ++this.intLineNumber;
                    goto Label_0793;
                }
            }
            if (string.IsNullOrEmpty(this.flInfo.DataEnclosing))
            {
                strArray = str.Split(new string[] { this.flInfo.Delimiter }, StringSplitOptions.None);
            }
            else
            {
                try
                {
                    strArray = LineToArray(str, this.flInfo.Delimiter, this.flInfo.DataEnclosing);
                }
                catch (Exception exception)
                {
                    this.lstSkippedLines.Add(new ErrorInfo(lineNumber, str, exception.ToString()));
                    recordsProcessed++;
                    lock (this.sr)
                    {
                        if (this.sr.EndOfStream)
                        {
                            goto Label_0799;
                        }
                        str = this.sr.ReadLine();
                        lineNumber = ++this.intLineNumber;
                    }
                    goto Label_0793;
                }
            }
        Label_0305:
            if (considerAllColumns)
            {
                newValue = string.Join("#$%", strArray, 0, (strArray.Length >= this.intTotalCols) ? this.intTotalCols : strArray.Length);
                if (!string.IsNullOrEmpty(this.flInfo.DataEnclosing))
                {
                    newValue = newValue.Replace(this.flInfo.DataEnclosing, "");
                }
                newValue = "'" + newValue.Replace("#$%", "','") + "'";
                if (strArray.Length < this.intTotalCols)
                {
                    newValue = newValue + "," + string.Join(",", this.strNulls, 0, this.intTotalCols - strArray.Length);
                }
            }
            else if (this.flInfo.IsLengthDelimitedFile)
            {
                foreach (LengthDelimitedColumnMapping mapping in this.flInfo.LengthDelimitedColMappingList)
                {
                    if (mapping.UseSequence)
                    {
                        newValue = newValue + lineNumber.ToString() + ",";
                    }
                    else if (mapping.UseConstant)
                    {
                        newValue = newValue + "'" + (!string.IsNullOrEmpty(mapping.ConstantValue) ? mapping.ConstantValue : "") + "',";
                    }
                    else
                    {
                        newValue = newValue + "'" + mapping.Value + "',";
                    }
                }
                if (newValue.Length > 0)
                {
                    newValue = newValue.Substring(0, newValue.Length - 1);
                }
            }
            else if (this.flInfo.ConsiderTrailingNullCols)
            {
                foreach (SrcToDestColumnMapping mapping2 in this.flInfo.MappingList)
                {
                    if (mapping2.UseSequence)
                    {
                        newValue = newValue + lineNumber.ToString() + ",";
                    }
                    else if (!string.IsNullOrEmpty(mapping2.ConstantValue))
                    {
                        newValue = newValue + "'" + mapping2.ConstantValue + "',";
                    }
                    else if ((mapping2.SourceColumnNo - 1) >= strArray.Length)
                    {
                        newValue = newValue + "NULL,";
                    }
                    else
                    {
                        newValue = newValue + "'" + strArray[mapping2.SourceColumnNo - 1] + "',";
                    }
                }
                if (newValue.Length > 0)
                {
                    newValue = newValue.Substring(0, newValue.Length - 1);
                }
            }
            else
            {
                if (this.flInfo.MaxColumnNumber > strArray.Length)
                {
                    this.lstSkippedLines.Add(new ErrorInfo(lineNumber, str, "Lesser columns than specified"));
                    recordsProcessed++;
                    lock (this.sr)
                    {
                        if (this.sr.EndOfStream)
                        {
                            goto Label_0799;
                        }
                        str = this.sr.ReadLine();
                        lineNumber = ++this.intLineNumber;
                        goto Label_0793;
                    }
                }
                newValue = "";
                foreach (SrcToDestColumnMapping mapping3 in this.flInfo.MappingList)
                {
                    if (mapping3.UseSequence)
                    {
                        newValue = newValue + lineNumber.ToString() + ",";
                    }
                    else if (!string.IsNullOrEmpty(mapping3.ConstantValue))
                    {
                        newValue = newValue + "'" + mapping3.ConstantValue + "',";
                    }
                    else
                    {
                        newValue = newValue + "'" + strArray[mapping3.SourceColumnNo - 1] + "',";
                    }
                }
                if (newValue.Length > 0)
                {
                    newValue = newValue.Substring(0, newValue.Length - 1);
                }
            }
            strCommandText = strCommandText.Replace("VALUEPART", newValue);
            lock (this.trn)
            {
                this.intCounter++;
                SqlCommand command = new SqlCommand(strCommandText, this.conn, this.trn);
                try
                {
                    command.ExecuteNonQuery();
                    this.intRecordsSaved[index]++;
                }
                catch (SqlException exception2)
                {
                    this.lstSkippedLines.Add(new ErrorInfo(lineNumber, str, exception2.ToString()));
                }
            }
            recordsProcessed++;
            lock (this.sr)
            {
                if (this.sr.EndOfStream)
                {
                    goto Label_0799;
                }
                str = this.sr.ReadLine().Replace("'", "''");
                lineNumber = ++this.intLineNumber;
            }
        Label_0793:
            if (str != null)
            {
                goto Label_010C;
            }
        Label_0799:
            span = DateTime.Now.Subtract(now);
            this.lstThreadInfo.Add(new ThreadInfo(index, recordsProcessed, ((span.Hours * 60) + (span.Minutes * 60)) + span.Seconds));
        }

        public override void StartLoadingData()
        {
            if (this.intExitCode == 1)
            {
                base.OnComplete(this, new DataLoaderEventArgs(0, 0, new List<ErrorInfo>(), new List<ThreadInfo>(), this.strErrorMsg));
                if (this.sr != null)
                {
                    this.sr.Close();
                }
            }
            else
            {
                DateTime now = DateTime.Now;
                this.conn.Open();
                this.trn = this.conn.BeginTransaction();
                for (int i = 0; i < this.threads.Length; i++)
                {
                    this.threads[i] = new Thread(new ParameterizedThreadStart(this.ReadLineAndLoadData));
                    this.threads[i].Priority = this.tPriority;
                    this.threads[i].IsBackground = true;
                    this.threads[i].Start(i.ToString());
                }
                for (int j = 0; j < this.threads.Length; j++)
                {
                    this.threads[j].Join();
                }
                this.trn.Commit();
                this.conn.Close();
                int totalRecordsLoaded = 0;
                for (int k = 0; k < this.intRecordsSaved.Length; k++)
                {
                    totalRecordsLoaded += this.intRecordsSaved[k];
                }
                this.intTotalRecords = this.lstSkippedLines.Count + totalRecordsLoaded;
                base.OnComplete(this, new DataLoaderEventArgs(this.intTotalRecords, totalRecordsLoaded, this.lstSkippedLines, this.lstThreadInfo, this.strErrorMsg));
                if (this.sr != null)
                {
                    this.sr.Close();
                }
            }
        }

        private static bool TestConnection(string ConnectionString)
        {
            bool flag;
            SqlConnection connection = new SqlConnection();
            try
            {
                connection = new SqlConnection(ConnectionString);
                connection.Open();
                connection.Close();
                flag = true;
            }
            catch
            {
                flag = false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return flag;
        }

        private static void TruncateData(string strTableName, string ConnectionString)
        {
            SqlConnection connection = new SqlConnection(ConnectionString);
            try
            {
                SqlCommand command = new SqlCommand("TRUNCATE TABLE " + strTableName, connection);
                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (SqlException exception)
            {
                throw exception;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        // Properties
        public override int ExitCode =>
            this.intExitCode;

        public override string ExitMessage =>
            this.strErrorMsg;
    }
}
