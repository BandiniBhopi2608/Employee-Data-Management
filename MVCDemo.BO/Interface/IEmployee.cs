using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVCDemo.VO;

namespace MVCDemo.BO
{
    public interface IEmployee
    {
        List<EmployeeVO> GetEmployeeList(int intEmpID);
        void AddEmployee(EmployeeVO objEmployee);
        void DeleteEmployee(int iEmpID, int iModifiedBy, DateTime dtModifiedOn);
    }
}
