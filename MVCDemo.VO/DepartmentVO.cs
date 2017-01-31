using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCDemo.VO
{
    public class DepartmentVO :  BaseModel
    {
        private string strDepartmentName;

        public string DepartmentName
        {
            get { return FormatString(strDepartmentName); }
            set { strDepartmentName = value; }
        }
    }
}
