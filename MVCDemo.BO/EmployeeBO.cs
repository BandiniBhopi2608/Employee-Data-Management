using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVCDemo.VO;
using DBLayer;
using System.Configuration;
using System.Data;
using CommonUtility;

namespace MVCDemo.BO
{
    public class EmployeeBO : IEmployee
    {
        public List<EmployeeVO> GetEmployeeList(int intEmpID)
        {
            List<EmployeeVO> lstEmployee = null;
            Database dbUtils = SqlDatabase.CreateDatabase(Convert.ToString(ConfigurationManager.ConnectionStrings["EmployeeContext"]));
            DataTable dtEmpList = dbUtils.ExecuteDataTable("FETCH_EMPLOYEE_MST_RECORD", CommandType.StoredProcedure, intEmpID);
            if (dtEmpList != null && dtEmpList.Rows.Count > 0)
            {
                lstEmployee = new List<EmployeeVO>();
                foreach (DataRow drEmp in dtEmpList.Rows)
                {
                    EmployeeVO objEmp = new EmployeeVO();
                    ReflectClass.ReflectObject(objEmp, drEmp);
                    objEmp.EmpDepartment.DepartmentName = drEmp["DepartmentName"].ToString();
                    lstEmployee.Add(objEmp);
                }
            }
            return lstEmployee;
        }

        public void AddEmployee(EmployeeVO objEmployee)
        {
            int intRetVal = 0;
            object objTrans = null;
            Database dbUtils = SqlDatabase.CreateDatabase(Convert.ToString(ConfigurationManager.ConnectionStrings["EmployeeContext"]));
            try
            {
                objTrans = dbUtils.BeginTransaction();
                Params objParams = dbUtils.ExecuteStoredProcedureWithTransaction(objTrans, "MANAGE_EMPLOYEE_MST"
                                                                                           ,objEmployee.ID
                                                                                           ,objEmployee.EmpName
                                                                                           ,objEmployee.Gender
                                                                                           ,objEmployee.City
                                                                                           ,objEmployee.DateOfBirth
                                                                                           ,objEmployee.CreatedByID
                                                                                           ,objEmployee.CreatedOn
                                                                                           ,objEmployee.ModifiedByID
                                                                                           ,objEmployee.ModifiedON
                                                                                           );
                intRetVal = Convert.ToInt32(objParams["@RET_VAL"]);
                if (intRetVal > 0)
                {
                    dbUtils.CommitTransaction(objTrans);
                }
                else
                {
                    dbUtils.RollBackTransaction(objTrans);
                }
            }
            catch (Exception ex)
            {
                dbUtils.RollBackTransaction(objTrans);
            }
            finally
            {
                dbUtils.CloseConnection();
            }
        }

        public void DeleteEmployee(int iEmpID, int iModifiedBy, DateTime dtModifiedOn)
        {
            int intRetVal = 0;
            object objTrans = null;
            Database dbUtils = SqlDatabase.CreateDatabase(Convert.ToString(ConfigurationManager.ConnectionStrings["EmployeeContext"]));
            try
            {
                objTrans = dbUtils.BeginTransaction();
                Params objParams = dbUtils.ExecuteStoredProcedureWithTransaction(objTrans, "DELETE_EMPLOYEE_MST"
                                                                                           , iEmpID
                                                                                           , iModifiedBy
                                                                                           , dtModifiedOn
                                                                                           );
                intRetVal = Convert.ToInt32(objParams["@RET_VAL"]);
                if (intRetVal > 0)
                {
                    dbUtils.CommitTransaction(objTrans);
                }
                else
                {
                    dbUtils.RollBackTransaction(objTrans);
                }
            }
            catch (Exception ex)
            {
                dbUtils.RollBackTransaction(objTrans);
            }
            finally
            {
                dbUtils.CloseConnection();
            }
        }
    }
}
