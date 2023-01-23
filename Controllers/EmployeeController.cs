namespace CEManager.Controllers
{
    using CEManager.Models;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;

    public class EmployeeController : Controller
    {
        readonly DBConnection dbConnection = new DBConnection();
        readonly CompanyController companyController = new CompanyController();
        public IActionResult Index()
        {
            return View();
        }

        public ActionResult AddEmployee(Employee employee)
        {
            var companiesName = new List<AnyName>();
            var company = dbConnection.Company.Where(x => x.CompanyId == employee.CompanyId).FirstOrDefault();
            if (employee.FullName != null && employee.Email != null && employee.CompanyId != 0 && employee.Position != null)
            {
                var companies = dbConnection.Company.Where(x => x.Status == 1).ToList();
                foreach (var companyItem in companies)
                {
                    var companyName = new AnyName();
                    companyName.Name = companyItem.Name;
                    companyName.Id = companyItem.CompanyId;
                    companiesName.Add(companyName);
                }
                ViewBag.Companies = companyController.GenerateList(companiesName);
                company.NumberOfEmployees += 1;
                employee.Status = 1;
                dbConnection.Employee.Add(employee);
                dbConnection.SaveChanges();
            }
            return View();
        }

        public Employee GetEmployee(int employeeId)
        {
            var employee = dbConnection.Employee.Where(x => x.EmployeeId == employeeId).FirstOrDefault();
            if (employee == null)
            {
                throw new Exception("Employee Not Found");
            }
            return employee;
        }

        public IActionResult GetEmployees()
        {
            var listOfEmployeeView = new List<EmployeeView>();
            var listOfEmployees = dbConnection.Employee.ToList().Where(x => x.Status == 1 && x.IsManager == false);
            foreach (var employee in listOfEmployees)
            {
                var company = dbConnection.Company.Where(x => x.Status == 1 && x.CompanyId == employee.CompanyId).FirstOrDefault();
                var employeeView = new EmployeeView();
                employeeView.Employee = employee;
                employeeView.CompanyName = company.Name;
                listOfEmployeeView.Add(employeeView);
            }
            return View(listOfEmployeeView);
        }

        [HttpGet]
        public IActionResult EditEmployee(int employeeId)
        {
            var employee = dbConnection.Employee.Where(x => x.EmployeeId == employeeId).FirstOrDefault();
            return View(employee);
        }

        [HttpPost]
        public IActionResult EditEmployee(Employee newemployee)
        {
            var employee = dbConnection.Employee.Where(x => x.EmployeeId == newemployee.EmployeeId).FirstOrDefault();
            var company = dbConnection.Company.Find(employee.CompanyId);
            var data = dbConnection.Employee.Where(x => x.CompanyId == company.CompanyId && x.Status == 1 && x.IsManager == true).SingleOrDefault();
            if (newemployee.FullName != null && newemployee.Email != null && newemployee.Position != null)
            {
                employee.FullName = newemployee.FullName;
                employee.Email = newemployee.Email;
                employee.Position = newemployee.Position;
                if (newemployee.IsManager == true && data == null)
                {
                    employee.IsManager = true;
                    company.ManagerId = newemployee.EmployeeId;
                }
                if (newemployee.IsManager == true && data != null)
                {
                    employee.IsManager = true;
                    data.IsManager = false;
                    company.ManagerId = newemployee.EmployeeId;
                }
                if (newemployee.IsManager == false && data == null)
                {
                    employee.IsManager = false;
                }
                if (newemployee.IsManager == false && data != null)
                {
                    employee.IsManager = false;
                    company.ManagerId = 0;
                }
                dbConnection.SaveChanges();
            }
            return RedirectToAction("GetEmployees");
        }

        [HttpGet]
        public ActionResult DeleteEmployeeAlert(int employeeId)
        {
            var employee = dbConnection.Employee.Find(employeeId);
            return View(employee);
        }

        [HttpPost]
        public ActionResult DeleteEmployee(int employeeId)
        {
            var employee = dbConnection.Employee.Find(employeeId);
            var company = dbConnection.Company.Find(employee.CompanyId);
            company.NumberOfEmployees--;
            employee.Status = 0;
            dbConnection.SaveChanges();
            return RedirectToAction("GetEmployees");
        }
    }
}
