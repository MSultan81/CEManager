namespace CEManager.Controllers
{
    using CEManager.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class CompanyController : Controller
    {
        DBConnection dbConnection = new DBConnection();
        EmployeeController employeeController = new EmployeeController();
        public int usersif = 0;
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create(Company company)
        {
            if (company.Name != null)
            {
                company.AddedDate = DateTime.Now;
                company.ManagerId = 0;
                company.NumberOfEmployees = 0;
                dbConnection.Company.Add(company);
                dbConnection.SaveChanges();
            }
            return View();
        }

        [HttpPost]
        public IActionResult AddCompanyEmployee(Employee employee)
        {
            var data = dbConnection.Company.Where(x => x.CompanyId == employee.CompanyId && x.Status == 1).FirstOrDefault();
            if (ModelState.IsValid)
            {
                if (employee.FullName != null && employee.Email != null && employee.Position != null)
                {
                    employee.CompanyId = data.CompanyId;
                    var company = dbConnection.Company.Where(x => x.CompanyId == employee.CompanyId).FirstOrDefault();
                    company.NumberOfEmployees += 1;
                    employee.Status = 1;
                    employee.IsManager = false;
                    dbConnection.Employee.Add(employee);
                    dbConnection.SaveChanges();
                    ViewBag.Success = "Employee created Successfully!!";
                }
            }

            return View();
        }

        [HttpGet]
        public ActionResult AddCompanyEmployee(int id)
        {
            var data = dbConnection.Employee.Where(x => x.CompanyId == id).FirstOrDefault();
            return View(data);
        }

        public string GetManager(int managerId, int n)
        {
            var manager = employeeController.GetEmployee(managerId);
            if (managerId == 0)
            {
                throw new ArgumentException("No Manager For This Company");
            }
            if (manager == null)
            {
                throw new ArgumentNullException("Manager Not found!!");
            }
            ViewBag.ManagerName[n] = manager.FullName;
            return ViewBag.ManagerName;
        }

        public List<AnyName> ListCompanyNames(int companyId)
        {
            var EmployeesNameList = new List<AnyName>();
            var employees = dbConnection.Employee.Where(x => x.Status == 1 && x.CompanyId == companyId).ToList();
            foreach (var employee in employees)
            {
                var employeeName = new AnyName();
                employeeName.Id = employee.EmployeeId;
                employeeName.Name = employee.FullName;
                EmployeesNameList.Add(employeeName);
            }
            return EmployeesNameList;
        }

        [HttpGet]
        public IActionResult EditCompany(int id)
        {
            var editCompanyView = new EditCompanyView();
            var companyEmployees = new List<AnyName>();
            var company = dbConnection.Company.Where(x => x.CompanyId == id && x.Status == 1).FirstOrDefault();
            editCompanyView.Company = company;
            var employees = dbConnection.Employee.Where(y => y.CompanyId == company.CompanyId && y.Status == 1).ToList();
            foreach (Employee employee in employees)
            {
                var employeeName = new AnyName();
                employeeName.Id = employee.EmployeeId;
                employeeName.Name = employee.FullName;
                companyEmployees.Add(employeeName);
            }
            editCompanyView.EmployeesList = companyEmployees;
            ViewBag.Employees = GenerateList(companyEmployees);
            return View(editCompanyView);
        }

        [HttpPost]
        public IActionResult EditCompany(Company company)
        {
            var data = dbConnection.Company.Find(company.CompanyId);

            if (company.Name != null && company.Description != null && company.Website != null)
            {
                var oldManager = dbConnection.Employee.Where(x => x.Status == 1 && x.IsManager == true && x.CompanyId == data.CompanyId).SingleOrDefault();
                if (oldManager != null)
                {
                    oldManager.IsManager = false;
                    dbConnection.SaveChanges();
                }
                data.Name = company.Name;
                data.Description = company.Description;
                data.Website = company.Website;
                data.ManagerId = company.ManagerId;
                var newManager = dbConnection.Employee.Find(company.ManagerId);
                if (newManager != null)
                {
                    newManager.IsManager = true;
                }
                dbConnection.SaveChanges();
            }
            return RedirectToAction("GetCompanies");
        }

        [HttpGet]
        public ActionResult ViewEmployees(int id)
        {
            var listOfEmployees = dbConnection.Employee.Where(x => x.Status == 1 && x.CompanyId == id).ToList();
            return View(listOfEmployees);
        }

        public List<CompanyView> MakeCompanyView(List<Company> companies)
        {
            var listOfCompaniesViews = new List<CompanyView>();
            foreach (var company in companies)
            {
                CompanyView companyView = new CompanyView();
                var manager = dbConnection.Employee.Where(x => x.CompanyId == company.CompanyId && x.IsManager == true).FirstOrDefault();
                companyView.Company = company;
                if (manager != null)
                {
                    companyView.CompanyManager = manager.FullName;
                }
                listOfCompaniesViews.Add(companyView);
            }
            return listOfCompaniesViews;
        }

        public ActionResult GetCompanies()
        {
            List<Company> listOfCompanies = dbConnection.Company.Where(x => x.Status == 1).ToList();
            var list = MakeCompanyView(listOfCompanies);
            return View(list);
        }

        public Company GetCompany(int companyId)
        {
            var company = dbConnection.Company.Where(x => x.CompanyId == companyId).FirstOrDefault();
            return company;
        }

        [HttpGet]
        public ActionResult DeleteCompanyAlert(int companyId)
        {
            var company = dbConnection.Company.Find(companyId);
            return View(company);
        }

        [HttpPost]
        public ActionResult DeleteCompany(int companyId)
        {
            var company = dbConnection.Company.Find(companyId);
            var companyEmployees = dbConnection.Employee.Where(x => x.CompanyId == companyId && x.Status == 1).ToList();
            foreach (var employee in companyEmployees)
            {
                employee.Status = 0;
            }
            company.Status = 0;
            dbConnection.SaveChanges();
            return RedirectToAction("GetCompanies");
        }

        public List<SelectListItem> GenerateList(List<AnyName> employeeList)
        {
            var list = new List<SelectListItem>();
            foreach (var employee in employeeList)
            {
                var employeeId = employee.Id.ToString();
                list.Add(new SelectListItem { Text = employee.Name, Value = employeeId });
            }
            return list;
        }
    }
}
