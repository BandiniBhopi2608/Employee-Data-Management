using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace MVCDemo.VO
{
    public class EmployeeVO : BaseModel, IEmployeeVO
    {
        //Test
        private string strEmpName;
        private string strGender;
        private string strCity;
        private DateTime? dtDateOfBirth;
        private DepartmentVO objDepartment;

        public string EmpName
        {
            get { return FormatString(strEmpName); }
            set { strEmpName = value; }
        }

        [Required]
        public string Gender
        {
            get { return FormatString(strGender); }
            set { strGender = value; }
        }

        [Required]
        public string City
        {
            get { return FormatString(strCity); }
            set { strCity = value; }
        }

        [Required]
        public DateTime? DateOfBirth
        {
            get { return dtDateOfBirth; }
            set { dtDateOfBirth = value; }
        }

        public DepartmentVO EmpDepartment
        {
            get
            {
                if (objDepartment == null)
                    objDepartment = new DepartmentVO();
                return objDepartment;
            }
            set { objDepartment = value; }
        }
    }
}
