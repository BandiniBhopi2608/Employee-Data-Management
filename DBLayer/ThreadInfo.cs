using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DBLayer
{
    public class ThreadInfo
    {
        // Fields
        private int intRecordsProcessed;
        private int intThreadNumber;
        private int intTimeSpent;

        // Methods
        public ThreadInfo(int ThreadNumber, int RecordsProcessed, int TimeSpent)
        {
            this.intThreadNumber = ThreadNumber;
            this.intRecordsProcessed = RecordsProcessed;
            this.intTimeSpent = TimeSpent;
        }

        // Properties
        public int RecordsProcessed =>
            this.intRecordsProcessed;

        public int ThreadNumber =>
            this.intThreadNumber;

        public int TimeSpent =>
            this.intTimeSpent;

    }
}
