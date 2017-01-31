using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace DBLayer
{
    public class SrcToDestColumnMapping : IComparable<SrcToDestColumnMapping>
    {
        // Fields
        private bool blnUseConstant;
        private bool blnUseSequence;
        public static Comparison<SrcToDestColumnMapping> ColumnNoComparison = (M1, M2) => M1.SourceColumnNo.CompareTo(M2.SourceColumnNo);
        private int intSourceColumnNo;
        private string strConstantValue;
        private string strDestinationColumnName;

        // Methods
        public SrcToDestColumnMapping(bool UseSequence, string DestinationColumnName)
        {
            this.intSourceColumnNo = this.SourceColumnNo;
            this.strDestinationColumnName = DestinationColumnName.ToUpper();
            this.blnUseSequence = UseSequence;
        }

        public SrcToDestColumnMapping(int SourceColumnNo, string DestinationColumnName)
        {
            this.intSourceColumnNo = SourceColumnNo;
            this.strDestinationColumnName = DestinationColumnName.ToUpper();
            this.blnUseSequence = false;
        }

        public SrcToDestColumnMapping(string ConstantValue, string DestinationColumnName)
        {
            this.intSourceColumnNo = this.SourceColumnNo;
            this.strDestinationColumnName = DestinationColumnName.ToUpper();
            this.strConstantValue = ConstantValue;
            this.blnUseSequence = false;
            this.blnUseConstant = true;
        }

        //[CompilerGenerated]
        //private static int <.cctor>b__0(SrcToDestColumnMapping M1, SrcToDestColumnMapping M2) =>
        //    M1.SourceColumnNo.CompareTo(M2.SourceColumnNo);

        public int CompareTo(SrcToDestColumnMapping other) =>
            this.SourceColumnNo.CompareTo(other.SourceColumnNo);

        // Properties
        public string ConstantValue =>
            this.strConstantValue;

        public string DestinationColumn =>
            this.strDestinationColumnName;

        public int SourceColumnNo =>
            this.intSourceColumnNo;

        public bool UseConstant =>
            this.blnUseConstant;

        public bool UseSequence =>
            this.blnUseSequence;
    }
}
