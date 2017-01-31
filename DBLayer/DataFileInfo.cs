using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DBLayer
{
    public class DataFileInfo
    {
        // Fields
        private bool blnConsiderAllColumns;
        private bool blnConsiderNullTrailingCols;
        private bool blnHasHeader;
        private bool blnIsLengthDelimitedFile;
        private LoadTypes enmLoadType;
        private int intMaxColumnNumber;
        private int intNoOfLinesToSkip;
        private List<SrcToDestColumnMapping> lstColMapping;
        private List<LengthDelimitedColumnMapping> lstLColMapping;
        private string strDataEnclosing;
        private string strDelimiter;
        private string strFilePath;
        private string strStagingTableName;

        // Methods
        public DataFileInfo(string FilePath, string StagingTableName, List<LengthDelimitedColumnMapping> ColumnsMapping, LoadTypes LoadType, bool ConsiderTrailingNullCols)
        {
            this.blnConsiderNullTrailingCols = true;
            this.strFilePath = FilePath;
            this.strStagingTableName = StagingTableName;
            this.blnConsiderAllColumns = false;
            this.lstLColMapping = ColumnsMapping;
            this.lstLColMapping.Sort();
            this.enmLoadType = LoadType;
            this.blnConsiderNullTrailingCols = ConsiderTrailingNullCols;
            this.blnIsLengthDelimitedFile = true;
        }

        public DataFileInfo(string FilePath, string Delimiter, string StagingTableName, string DataEnclosing, LoadTypes LoadType, bool HasHeader)
        {
            this.blnConsiderNullTrailingCols = true;
            this.strFilePath = FilePath;
            this.strDelimiter = Delimiter;
            this.strStagingTableName = StagingTableName;
            this.strDataEnclosing = DataEnclosing;
            this.blnConsiderAllColumns = true;
            this.enmLoadType = LoadType;
            this.blnHasHeader = HasHeader;
        }

        public DataFileInfo(string FilePath, string Delimiter, string StagingTableName, string DataEnclosing, List<SrcToDestColumnMapping> ColumnsMapping, LoadTypes LoadType, bool ConsiderTrailingNullCols, bool HasHeader)
        {
            this.blnConsiderNullTrailingCols = true;
            this.strFilePath = FilePath;
            this.strDelimiter = Delimiter;
            this.strStagingTableName = StagingTableName;
            this.strDataEnclosing = DataEnclosing;
            this.blnConsiderAllColumns = false;
            this.lstColMapping = ColumnsMapping;
            this.lstColMapping.Sort();
            this.intMaxColumnNumber = this.lstColMapping[this.lstColMapping.Count - 1].SourceColumnNo;
            this.enmLoadType = LoadType;
            this.blnConsiderNullTrailingCols = ConsiderTrailingNullCols;
            this.blnHasHeader = HasHeader;
        }

        // Properties
        public bool ConsiderAllColumns =>
            this.blnConsiderAllColumns;

        public bool ConsiderTrailingNullCols =>
            this.blnConsiderNullTrailingCols;

        public string DataEnclosing =>
            this.strDataEnclosing;

        public string Delimiter =>
            this.strDelimiter;

        public string FilePath =>
            this.strFilePath;

        public bool HasHeader =>
            this.blnHasHeader;

        public bool IsLengthDelimitedFile =>
            this.blnIsLengthDelimitedFile;

        public List<LengthDelimitedColumnMapping> LengthDelimitedColMappingList =>
            this.lstLColMapping;

        public LoadTypes LoadType =>
            this.enmLoadType;

        public List<SrcToDestColumnMapping> MappingList =>
            this.lstColMapping;

        public int MaxColumnNumber =>
            this.intMaxColumnNumber;

        public int SpecifiedLength
        {
            get
            {
                if ((this.lstLColMapping != null) && (this.lstLColMapping.Count > 0))
                {
                    return (this.lstLColMapping[this.lstLColMapping.Count - 1].StartIndex + this.lstLColMapping[this.lstLColMapping.Count - 1].Length);
                }
                return 0;
            }
        }

        public string StagingTableName =>
            this.strStagingTableName;

        public int StartingLinesToSkip
        {
            get { return this.intNoOfLinesToSkip; }
            set
            {
                this.intNoOfLinesToSkip = value;
                if (this.intNoOfLinesToSkip >= 0)
                {
                    this.blnHasHeader = false;
                }
            }
        }

    }
}
