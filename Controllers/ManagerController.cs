namespace CEManager.Controllers
{
    using CEManager.Models;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;

    public class ManagerController : Controller
    {
        DBConnection dbConnection = new DBConnection();
        CompanyController companyController = new CompanyController();
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetManagers()
        {
            var listOfManagerView = new List<EmployeeView>();
            var listOfEmployees = dbConnection.Employee.ToList().Where(x => x.Status == 1 && x.IsManager == true);
            foreach (var employee in listOfEmployees)
            {
                var company = dbConnection.Company.Where(x => x.Status == 1 && x.CompanyId == employee.CompanyId).FirstOrDefault();
                var employeeView = new EmployeeView();
                employeeView.Employee = employee;
                employeeView.CompanyName = company.Name;
                listOfManagerView.Add(employeeView);
            }
            return View(listOfManagerView);
        }

        public IActionResult AddManager(Employee manager)
        {
            var companiesName = new List<AnyName>();
            var company = dbConnection.Company.Where(x => x.CompanyId == manager.CompanyId).FirstOrDefault();
            if (manager.FullName != null && manager.Email != null && company.ManagerId == 0)
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
                manager.IsManager = true;
                company.NumberOfEmployees++;
                dbConnection.Employee.Add(manager);
                dbConnection.SaveChanges();
            }
            return View();
        }

        [HttpGet]
        public IActionResult EditManager(int managerId)
        {
            var manager = dbConnection.Employee.Where(x => x.EmployeeId == managerId).FirstOrDefault();
            return View(manager);
        }

        [HttpPost]
        public IActionResult EditManager(Employee newmanager)
        {
            var manager = dbConnection.Employee.Where(x => x.EmployeeId == newmanager.EmployeeId).FirstOrDefault();
            var company = dbConnection.Company.Find(manager.CompanyId);
            var data = dbConnection.Employee.Where(x => x.CompanyId == company.CompanyId && x.Status == 1 && x.IsManager == true).SingleOrDefault();
            if (newmanager.FullName != null && newmanager.Email != null && newmanager.Position != null)
            {
                manager.FullName = newmanager.FullName;
                manager.Email = newmanager.Email;
                manager.Position = newmanager.Position;
                if (data == null && newmanager.IsManager == true)
                {
                    manager.IsManager = true;
                    company.ManagerId = newmanager.EmployeeId;
                    dbConnection.SaveChanges();
                }
                if (newmanager.IsManager == false && data.EmployeeId == newmanager.EmployeeId)
                {
                    company.ManagerId = 0;
                    newmanager.IsManager = false;
                    dbConnection.SaveChanges();
                }
                else
                {
                    newmanager.IsManager = false;
                }
                manager.IsManager = newmanager.IsManager;
                dbConnection.SaveChanges();
            }
            return RedirectToAction("GetManagers");
        }

        [HttpGet]
        public ActionResult DeleteManagerAlert(int managerId)
        {
            var manager = dbConnection.Employee.Find(managerId);
            return View(manager);
        }

        [HttpPost]
        public ActionResult DeleteManager(int managerId)
        {
            var manager = dbConnection.Employee.Find(managerId);
            var company = dbConnection.Company.Find(manager.CompanyId);
            company.NumberOfEmployees--;
            company.ManagerId = 0;
            manager.Status = 0;
            dbConnection.SaveChanges();
            return RedirectToAction("GetManagers");
        }
    }
}
