using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace DBLayer
{
    public class LengthDelimitedColumnMapping : IComparable<LengthDelimitedColumnMapping>
    {
        // Fields
        private bool blnUseConstant;
        private bool blnUseSequence;
        public static Comparison<LengthDelimitedColumnMapping> ColumnNoComparison = (M1, M2) => M1.StartIndex.CompareTo(M2.StartIndex);
        private int intLength;
        private int intStartIndex;
        private string strConstantValue;
        private string strDestColumnName;
        private string strValue;

        // Methods
        public LengthDelimitedColumnMapping(bool UseSequence, string MappedColumnName)
        {
            this.intStartIndex = 0;
            this.intLength = 0;
            this.strDestColumnName = MappedColumnName;
            this.blnUseSequence = UseSequence;
        }

        public LengthDelimitedColumnMapping(string ConstantValue, string MappedColumnName)
        {
            this.intStartIndex = 0;
            this.intLength = 0;
            this.strConstantValue = ConstantValue;
            this.strDestColumnName = MappedColumnName;
            this.blnUseConstant = true;
        }

        public LengthDelimitedColumnMapping(int StartIndex, int Length, string MappedColumnName)
        {
            this.intStartIndex = StartIndex;
            this.intLength = Length;
            this.strDestColumnName = MappedColumnName;
        }

        //[CompilerGenerated]
        //private static int <.cctor>b__0(LengthDelimitedColumnMapping M1, LengthDelimitedColumnMapping M2) =>
        //    M1.StartIndex.CompareTo(M2.StartIndex);

        public int CompareTo(LengthDelimitedColumnMapping other) =>
            this.StartIndex.CompareTo(other.StartIndex);

        // Properties
        public string ConstantValue =>
            this.strConstantValue;

        public int Length =>
            this.intLength;

        public string MappedColumnName =>
            this.strDestColumnName;

        public int StartIndex =>
            this.intStartIndex;

        public bool UseConstant =>
            this.blnUseConstant;

        public bool UseSequence =>
            this.blnUseSequence;

        public string Value
        {
            get { return this.strValue; } 
            set
            {
                this.strValue = value;
            }
            }
        }
    }
