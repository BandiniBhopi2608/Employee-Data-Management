using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DBLayer
{
    public class ErrorInfo
    {
        // Fields
        private int intLineNumber;
        private string strErrorMessage;
        private string strLineData;

        // Methods
        public ErrorInfo(int LineNumber, string LineData, string ErrorMessage)
        {
            this.intLineNumber = LineNumber;
            this.strLineData = LineData;
            this.strErrorMessage = ErrorMessage;
        }

        // Properties
        public string ErrorMessage
        {
            get { return this.strErrorMessage; }
            set
            {
                this.strErrorMessage = value;
            }
        }

        public string LineData
        {
            get { return this.strLineData; }
            set
            {
                this.strLineData = value;
            }
        }

        public int LineNumber
        {
            get { return this.intLineNumber; }

            set
            {
                this.intLineNumber = value;
            }
        }

    }
}
