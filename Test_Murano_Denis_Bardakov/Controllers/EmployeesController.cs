using PagedList;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Test_Murano_Denis_Bardakov.Models;

namespace Test_Murano_Denis_Bardakov.Controllers
{
    public class EmployeesController : Controller
    {
        IRepository repo;

        public EmployeesController()
        {
            repo = new EmployeeRepository();
        }
        
        public EmployeesController(IRepository r)
        {
            repo = r;
        }

        public ActionResult SaveReport()
        {
            //Можно использовать LINQ
            /*var dataReport = from data in repo.GetEmployeesList()
                             orderby data.FullName
                             where data.Status.StartsWith("ак")
                             select new { data.FullName, data.Salary };*/

            var employeesList = repo.GetEmployeesList()
                                .Where(x => x.Status.StartsWith("ак"))
                                .OrderBy(x => x.FullName);

            ViewBag.FilePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Отчет.txt";
            using (StreamWriter file = new StreamWriter(ViewBag.FilePath))
            {
                byte tax = 0;
                decimal totalSalary = 0;
                decimal totalSalaryMinusTaxes = 0;
                
                foreach (var e in employeesList)
                {
                    if (e.Salary < 10000)
                        tax = 10;
                    else if (e.Salary >= 10000 && e.Salary < 25000)
                        tax = 15;
                    else if (e.Salary >= 25000)
                        tax = 25;

                    decimal salaryMinusTaxes = e.Salary - (e.Salary * tax / 100);

                    file.WriteLine("Имя = {0}; зарплата = {1}; налог = {2}%; зарплата за вычетом налогов = {3}",
                                    e.FullName, e.Salary, tax, salaryMinusTaxes);

                    totalSalary += e.Salary;
                    totalSalaryMinusTaxes += salaryMinusTaxes;
                }
                file.WriteLine();
                file.WriteLine("Итого: зарплата = {0}; сумма налогов = {1}; зарплата за вычетом налогов = {2}",
                                    totalSalary, (totalSalary - totalSalaryMinusTaxes), totalSalaryMinusTaxes);
            }
            return View();
        }

        public ActionResult Index(string filter, int? page)
        {
            var employeesList = repo.GetEmployeesList().OrderBy(z => z.FullName).AsQueryable();
            
            if (filter == "активен")
            {
                employeesList = employeesList.Where(x => x.Status.StartsWith("ак"));
            }
            else if (filter == "не активен")
            {
                employeesList = employeesList.Where(x => x.Status.StartsWith("не"));
            }

            return View(employeesList.ToPagedList(page ?? 1, 7));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create([Bind(Include = "Id,FullName,Position,Status,Salary")] Employees employee)
        {
            try 
            {
                if (ModelState.IsValid)
                {
                    repo.Create(employee);
                    repo.Save();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex);
                ViewData["CreateError"] = "Unable to create; view innerexception";
            }
            return View("Create");
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employees employee = repo.GetEmployeeById(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        [HttpPost]
        public ActionResult Edit([Bind(Include = "Id,FullName,Position,Status,Salary")] Employees employees)
        {
            if (ModelState.IsValid)
            {
                repo.Update(employees);
                repo.Save();
                return RedirectToAction("Index");
            }
            return View(employees);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employees employee = repo.GetEmployeeById(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            repo.Delete(id);
            repo.Save();
            return RedirectToAction("Index");
        }
    }
}
