using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCDemo.VO
{
    public class BaseModel : CommonFun
    {
        private int intID;
        private string strCreatedBy;
        private DateTime dtCreatedOn;
        private string strModifiedBy;
        private DateTime dtModifiedOn;
        private int intCreatedByID;
        private int intModifiedByID;

        public int ID
        {
            get { return intID; }
            set { intID = value; }
        }

        public string CreatedBy
        {
            get { return FormatString(strCreatedBy); }
            set { strCreatedBy = value; }
        }

        public int CreatedByID
        {
            get { return intCreatedByID; }
            set { intCreatedByID = value; }
        }

        public DateTime CreatedOn
        {
            get { return dtCreatedOn; }
            set { dtCreatedOn = value; }
        }

        public string ModifiedBy
        {
            get { return FormatString(strModifiedBy); }
            set { strModifiedBy = value; }
        }

        public int ModifiedByID
        {
            get { return intModifiedByID; }
            set { intModifiedByID = value; }
        }

        public DateTime ModifiedON
        {
            get { return dtModifiedOn; }
            set { dtModifiedOn = value; }
        }
    }
}
