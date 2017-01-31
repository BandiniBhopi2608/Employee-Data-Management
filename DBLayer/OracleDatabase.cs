using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Data.OracleClient;
using System.Data;

namespace DBLayer
{
    public class OracleDatabase : Database
    {
        // Methods
        private OracleDatabase()
        {
        }

        private OracleDatabase(string ConnectionString)
        {
            base.strConnectionString = ConnectionString;
            base.objConnection = new OracleConnection(base.strConnectionString);
        }

        public override object BeginTransaction()
        {
            OracleConnection objConnection = (OracleConnection)base.objConnection;
            if (objConnection == null)
            {
                return null;
            }
            if (objConnection.State != ConnectionState.Open)
            {
                objConnection.Open();
            }
            return objConnection.BeginTransaction();
        }

        public override void CloseConnection()
        {
            if (base.objConnection != null)
            {
                OracleConnection objConnection = (OracleConnection)base.objConnection;
                if (objConnection.State == ConnectionState.Open)
                {
                    objConnection.Close();
                }
            }
        }

        public override void CommitTransaction(object Transaction)
        {
            if (Transaction != null)
            {
                ((OracleTransaction)Transaction).Commit();
            }
        }

        public static Database CreateDatabase(string ConnectionString) =>
            new OracleDatabase(ConnectionString);

        public override DataSet ExecuteDataSet(string strCommandText, CommandType cmdType, params object[] cmdParams)
        {
            OracleConnection objConnection = (OracleConnection)base.objConnection;
            OracleCommand command = new OracleCommand(strCommandText, objConnection);
            List<int> list = new List<int>();
            command.CommandType = cmdType;
            if (cmdType == CommandType.StoredProcedure)
            {
                try
                {
                    if (objConnection.State != ConnectionState.Open)
                    {
                        objConnection.Open();
                    }
                    OracleCommandBuilder.DeriveParameters(command);
                    objConnection.Close();
                    for (int j = 0; j < command.Parameters.Count; j++)
                    {
                        OracleParameter parameter = command.Parameters[j];
                        if (parameter.OracleType != OracleType.Cursor)
                        {
                            list.Add(j);
                        }
                    }
                }
                catch (OracleException exception)
                {
                    throw exception;
                }
                for (int i = 0; i < list.Count; i++)
                {
                    if (cmdParams.Length <= i)
                    {
                        throw new Exception("Parameter Count does not match.");
                    }
                    command.Parameters[list[i]].Value = cmdParams[i];
                }
            }
            OracleDataAdapter adapter = new OracleDataAdapter(command);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);
            return dataSet;
        }

        public override DataTable ExecuteDataTable(string strCommandText, CommandType cmdType, params object[] cmdParams)
        {
            OracleConnection objConnection = (OracleConnection)base.objConnection;
            OracleCommand command = new OracleCommand(strCommandText, objConnection);
            List<int> list = new List<int>();
            command.CommandType = cmdType;
            if (cmdType == CommandType.StoredProcedure)
            {
                try
                {
                    if (objConnection.State != ConnectionState.Open)
                    {
                        objConnection.Open();
                    }
                    OracleCommandBuilder.DeriveParameters(command);
                    objConnection.Close();
                    for (int j = 0; j < command.Parameters.Count; j++)
                    {
                        OracleParameter parameter = command.Parameters[j];
                        if (parameter.OracleType != OracleType.Cursor)
                        {
                            list.Add(j);
                        }
                    }
                }
                catch (OracleException exception)
                {
                    throw exception;
                }
                for (int i = 0; i < list.Count; i++)
                {
                    if (cmdParams.Length <= i)
                    {
                        throw new Exception("Parameter Count does not match.");
                    }
                    command.Parameters[list[i]].Value = cmdParams[i];
                }
            }
            OracleDataAdapter adapter = new OracleDataAdapter(command);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            return dataTable;
        }

        public override Params ExecuteStoredProcedure(string strProcedureName, params object[] cmdParams)
        {
            Params params2;
            Params @params = new OracleParams();
            OracleConnection objConnection = (OracleConnection)base.objConnection;
            try
            {
                OracleCommand command = new OracleCommand(strProcedureName, objConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                if (objConnection.State != ConnectionState.Open)
                {
                    objConnection.Open();
                }
                OracleCommandBuilder.DeriveParameters(command);
                for (int i = 0; i < command.Parameters.Count; i++)
                {
                    if (command.Parameters[i].Direction == ParameterDirection.Input)
                    {
                        if (cmdParams.Length <= i)
                        {
                            throw new Exception("Parameter Count does not match.");
                        }
                        command.Parameters[i].Value = cmdParams[i];
                    }
                    else
                    {
                        command.Parameters[i].Value = DBNull.Value;
                    }
                }
                command.ExecuteNonQuery();
                foreach (OracleParameter parameter in command.Parameters)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput) || (parameter.Direction == ParameterDirection.Output))
                    {
                        @params.Add(parameter);
                    }
                }
                params2 = @params;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.CloseConnection();
            }
            return params2;
        }

        public override Params ExecuteStoredProcedureWithTransaction(object trnTransaction, string strProcedureName, params object[] cmdParams)
        {
            Params params2;
            Params @params = new OracleParams();
            OracleConnection objConnection = (OracleConnection)base.objConnection;
            OracleTransaction tx = (OracleTransaction)trnTransaction;
            try
            {
                OracleCommand command = new OracleCommand(strProcedureName, objConnection, tx)
                {
                    CommandType = CommandType.StoredProcedure
                };
                if (objConnection.State == ConnectionState.Closed)
                {
                    objConnection.Open();
                }
                OracleCommandBuilder.DeriveParameters(command);
                for (int i = 0; i < command.Parameters.Count; i++)
                {
                    if (command.Parameters[i].Direction == ParameterDirection.Input)
                    {
                        if (cmdParams.Length <= i)
                        {
                            throw new Exception("Parameter Count does not match.");
                        }
                        command.Parameters[i].Value = cmdParams[i];
                    }
                    else
                    {
                        command.Parameters[i].Value = DBNull.Value;
                    }
                }
                command.ExecuteNonQuery();
                foreach (OracleParameter parameter in command.Parameters)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput) || (parameter.Direction == ParameterDirection.Output))
                    {
                        @params.Add(parameter);
                    }
                }
                params2 = @params;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return params2;
        }

        public override bool ExecuteText(string strCommandText, CompileAs type)
        {
            bool flag;
            string[] strArray = new string[0];
            if (type == CompileAs.Objects)
            {
                strArray = strCommandText.Split(new char[] { ';' });
            }
            else
            {
                strArray = strCommandText.Split(new char[] { '/' });
            }
            OracleConnection objConnection = (OracleConnection)base.objConnection;
            try
            {
                if (objConnection.State != ConnectionState.Open)
                {
                    objConnection.Open();
                }
                foreach (string str in strArray)
                {
                    if (str.Trim().Replace("\r\n", "") != "")
                    {
                        string commandText = str.Replace("\r\n", "\n");
                        if (commandText.IndexOf("\n") == 0)
                        {
                            commandText = commandText.Substring(1);
                        }
                        if ((commandText.LastIndexOf("\n") + 1) == commandText.Length)
                        {
                            commandText = commandText.Substring(0, commandText.Length - 1);
                        }
                        new OracleCommand(commandText, objConnection) { CommandType = CommandType.Text }.ExecuteNonQuery();
                    }
                }
                flag = true;
            }
            catch
            {
                flag = false;
            }
            finally
            {
                this.CloseConnection();
            }
            return flag;
        }

        public override void RollBackTransaction(object Transaction)
        {
            if (Transaction != null)
            {
                ((OracleTransaction)Transaction).Rollback();
            }
        }

        public override bool TestConnection()
        {
            bool flag;
            OracleConnection connection = new OracleConnection(base.strConnectionString);
            try
            {
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

        // Nested Types
        private class OracleParams : Params
        {
            // Fields
            private Dictionary<string, int> dctParams = new Dictionary<string, int>();
            private List<OracleParameter> lstParams = new List<OracleParameter>();

            // Methods
            public override void Add(object Param)
            {
                OracleParameter item = (OracleParameter)Param;
                this.lstParams.Add(item);
                this.dctParams.Add(item.ParameterName.ToUpper(), this.lstParams.Count - 1);
            }

            // Properties
            public override object this[int index] =>
                this.lstParams[index].Value;

            public override object this[string ParamName]
            {
                get
                {
                    if (this.dctParams.ContainsKey(ParamName.ToUpper()))
                    {
                        return this.lstParams[this.dctParams[ParamName.ToUpper()]].Value;
                    }
                    return null;
                }
            }
        }
    }
}
