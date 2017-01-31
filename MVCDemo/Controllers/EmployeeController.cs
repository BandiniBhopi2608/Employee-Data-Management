using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCDemo.VO;
using MVCDemo.BO;

namespace MVCDemo.Controllers
{
    public class EmployeeController : Controller
    {
        // GET: Employee
        IEmployee objEmpBO;
        public EmployeeController()
        {
            objEmpBO = new EmployeeBO();
        }

        public ActionResult Index()
        {
            List<EmployeeVO> EmployeeList = objEmpBO.GetEmployeeList(0);
            return View(EmployeeList);
        }

        [HttpGet]
        [ActionName("Create")]
        public ActionResult Create_Get()
        {
            return View();
        }

        #region Commented Code
        /* Insert data using FormCollection : One way
        [HttpPost]
        public ActionResult Create(FormCollection formCollection)
        {
            EmployeeVO objEmployee = new EmployeeVO();
            objEmployee.EmpName = formCollection["EmpName"];
            objEmployee.Gender = formCollection["Gender"];
            objEmployee.City = formCollection["City"];
            objEmployee.CreatedByID = 1;
            objEmployee.CreatedOn = DateTime.Now;
            objEmpBO.AddEmployee(objEmployee);
            return RedirectToAction("Index");
        }
        */

        /*Insert data passing Parameters : Second way
        [HttpPost]
        public ActionResult Create(string EmpName, string Gender, string City)
        {
            EmployeeVO objEmployee = new EmployeeVO();
            objEmployee.EmpName = EmpName;
            objEmployee.Gender = Gender;
            objEmployee.City = City;
            objEmployee.CreatedByID = 1;
            objEmployee.CreatedOn = DateTime.Now;
            objEmpBO.AddEmployee(objEmployee);
            return RedirectToAction("Index");
        }
        */

        /*Insert Data by passing Employee Object : Third way
        [HttpPost]
        public ActionResult Create(EmployeeVO objEmployee)
        {
            objEmployee.CreatedByID = 1;
            objEmployee.CreatedOn = DateTime.Now;
            objEmpBO.AddEmployee(objEmployee);
            return RedirectToAction("Index");
        }
        */
        #endregion

        //Insert Data without passing any parameter and using UpdateModel
        [HttpPost]
        [ActionName("Create")]
        public ActionResult Create_Post()
        {
            EmployeeVO objEmployee = new EmployeeVO();
            TryUpdateModel(objEmployee);

            if (ModelState.IsValid)
            {
                objEmployee.CreatedByID = 1;
                objEmployee.CreatedOn = DateTime.Now;
                objEmpBO.AddEmployee(objEmployee);
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpGet]
        public ActionResult Edit(int Id)
        {
            List<EmployeeVO> EmployeeList = objEmpBO.GetEmployeeList(Id);
            if (EmployeeList != null && EmployeeList.Count > 0)
                return View(EmployeeList[0]);
            return View();
        }
        //Chapter 22: Including and excluding properties from Model Binding using Interfaces
        [HttpPost]
        [ActionName("Edit")]
        public ActionResult Edit_Post(int Id)
        {
            List<EmployeeVO> EmployeeList = objEmpBO.GetEmployeeList(Id);
            if (EmployeeList != null && EmployeeList.Count > 0)
            {
                EmployeeVO objEmployee = EmployeeList[0];
                TryUpdateModel<IEmployeeVO>(objEmployee);
                if (ModelState.IsValid)
                {
                    objEmployee.ModifiedByID = 1;
                    objEmployee.ModifiedON = DateTime.Now;
                    objEmpBO.AddEmployee(objEmployee);
                    return RedirectToAction("Index");
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult Delete(int Id)
        {
            objEmpBO.DeleteEmployee(Id, 1, DateTime.Now);
            return RedirectToAction("Index");
        }

        #region Commented Code
        /*Update Data by passing Employee Object
         * Problem : Unintended updates in MVC
        [HttpPost]
        public ActionResult Edit(EmployeeVO objEmployee)
        {
            if (ModelState.IsValid)
            {
                objEmployee.ModifiedByID = 1;
                objEmployee.ModifiedON = DateTime.Now;
                objEmpBO.AddEmployee(objEmployee);
                return RedirectToAction("Index");
            }
            return View();
        }
        */

        /*Chapter 20 : Preventing Unintended updates in MVC
         * using INCLUDELIST AND EXCLUDELIST
        [HttpPost]
        [ActionName("Edit")]
        public ActionResult Edit_Post(int Id)
        {
            List<EmployeeVO> EmployeeList = objEmpBO.GetEmployeeList(Id);
            if (EmployeeList != null && EmployeeList.Count > 0)
            {
                EmployeeVO objEmployee = EmployeeList[0];
                UpdateModel(objEmployee, null, null, new string[] { "EmpName" });
                if (ModelState.IsValid)
                {
                    objEmployee.ModifiedByID = 1;
                    objEmployee.ModifiedON = DateTime.Now;
                    objEmpBO.AddEmployee(objEmployee);
                    return RedirectToAction("Index");
                }
            }
            return View();
        }
        */

        /*
        //Chapter 21: Including and excluding properties from Model Binding using Bind attribute
        [HttpPost]
        [ActionName("Edit")]
        public ActionResult Edit_Post([Bind(Exclude = "EmpName")]EmployeeVO objEmployee)
        {
            List<EmployeeVO> EmployeeList = objEmpBO.GetEmployeeList(objEmployee.ID);
            if (EmployeeList != null && EmployeeList.Count > 0)
            {
                objEmployee.EmpName = EmployeeList[0].EmpName;                
                if (ModelState.IsValid)
                {
                    objEmployee.ModifiedByID = 1;
                    objEmployee.ModifiedON = DateTime.Now;
                    objEmpBO.AddEmployee(objEmployee);
                    return RedirectToAction("Index");
                }
            }
            return View();
        }
        */
        #endregion

        /*
        public ActionResult Details()
        {
            EmployeeContext objEmployeeContext = new EmployeeContext();
            Employee objEmp=objEmployeeContext.Employees.Single(emp => emp.ID == id);
            return View(objEmp);
        }
        */
    }
}