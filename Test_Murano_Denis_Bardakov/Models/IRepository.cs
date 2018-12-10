using System;
using System.Collections.Generic;

namespace Test_Murano_Denis_Bardakov.Models
{
    public interface IRepository : IDisposable
    {
        List<Employees> GetEmployeesList();
        Employees GetEmployeeById(int? id);
        void Create(Employees item);
        void Update(Employees item);
        void Delete(int id);
        void Save();
    }
}
