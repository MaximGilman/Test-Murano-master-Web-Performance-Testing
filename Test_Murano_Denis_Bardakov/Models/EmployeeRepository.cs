using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Test_Murano_Denis_Bardakov.Models
{
    public class EmployeeRepository : IRepository
    {
        private EmployeesContext db;
        public EmployeeRepository()
        {
            this.db = new EmployeesContext();
        }
        public List<Employees> GetEmployeesList()
        {
            return db.Employees.ToList();
        }
        public Employees GetEmployeeById(int? id)
        {
            return db.Employees.Find(id);
        }

        public void Create(Employees e)
        {
            db.Employees.Add(e);
        }

        public void Update(Employees e)
        {
            db.Entry(e).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            Employees emp = db.Employees.Find(id);
            if (emp != null)
                db.Employees.Remove(emp);
        }

        public void Save()
        {
            db.SaveChanges();
        }

        private bool disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}