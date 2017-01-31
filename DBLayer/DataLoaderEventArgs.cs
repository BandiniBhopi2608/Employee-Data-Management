using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DBLayer
{
    public class DataLoaderEventArgs : EventArgs
    {
        // Fields
        private int intTotalRecords;
        private int intTotalRecordsLoaded;
        private int intTotalRecordsSkipped;
        private List<ErrorInfo> lstSkippedLines;
        private List<ThreadInfo> lstThreadInfo;
        private string strExitErrorMessage;

        // Methods
        private DataLoaderEventArgs()
        {
        }

        public DataLoaderEventArgs(int TotalRecords, int TotalRecordsLoaded, List<ErrorInfo> SkippedLines, List<ThreadInfo> ThreadInfoCollection, string ExitMessage)
        {
            this.intTotalRecords = TotalRecords;
            this.intTotalRecordsLoaded = TotalRecordsLoaded;
            this.lstSkippedLines = SkippedLines;
            this.intTotalRecordsSkipped = this.lstSkippedLines.Count;
            this.strExitErrorMessage = ExitMessage;
            this.lstThreadInfo = ThreadInfoCollection;
        }

        // Properties
        public string ExitErrorMessage =>
            this.strExitErrorMessage;

        public List<ErrorInfo> SkippedLines =>
            this.lstSkippedLines;

        public List<ThreadInfo> ThreadInfoCollection =>
            this.lstThreadInfo;

        public int TotalRecords =>
            this.intTotalRecords;

        public int TotalRecordsLoaded =>
            this.intTotalRecordsLoaded;

        public int TotalRecordsSkipped =>
            this.intTotalRecordsSkipped;

    }
}
