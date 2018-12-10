using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Test_Murano_Denis_Bardakov.Controllers;
using Test_Murano_Denis_Bardakov.Models;

namespace Test_Murano_Denis_Bardakov.Tests
{
    [TestClass]
    public class EmployeesControllerTest
    {

        EmployeesController EmployeesController;
        EmployeeRepository EmployeeRepository;
        SqlConnection SqlConnection;

        [TestInitialize]
        public void InitializeController()
        {
            EmployeeRepository = new EmployeeRepository();
            EmployeesController = new EmployeesController(EmployeeRepository);
        }



        [TestMethod]
        [TestCategory("Integration.SQL")]
        public void CheckSqlConnectionByConfig()
        {
            using (SqlConnection cn = new SqlConnection { ConnectionString = ConfigurationManager.ConnectionStrings["EmployeesContextTest"].ConnectionString })
            {
                using (SqlCommand cmd = new SqlCommand { Connection = cn })
                {
                    cmd.CommandText = "select Id, FullName, Position, Status, Salary from [dbo].[list_employees]";
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        [TestMethod]
        [TestCategory("Integration.SQL")]
        public void CheckSqlConnectionByString()
        {
            SqlConnection = new SqlConnection(@"server=(localdb)\mssqllocaldb; database=EmployeesTest; integrated security=True");
            if (SqlConnection.State != ConnectionState.Open)
                SqlConnection.Open();
            var sql = "select Id, FullName, Position, Status, Salary from [dbo].[list_employees]";
            var cmd = new SqlCommand(sql, SqlConnection);

            using (SqlDataReader MyReader = cmd.ExecuteReader())
            {
                while (MyReader.Read())
                {
                    string[] row = new string[MyReader.FieldCount];
                    for (int i = 0; i < MyReader.FieldCount; i++)
                        row[i] = MyReader[i].ToString().Trim();
                }
            }
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void Create()
        {
            //Arrange.
            var expected = new Employees()
            {
                FullName = "Чусовлянкин Алексей Александрович",
                Position = "Директор",
                Salary = 100000,
                Status = "активен"
            };

            //Act.
            EmployeesController.Create(expected);
            var actual = EmployeeRepository.GetEmployeeById(expected.Id);

            //Assert.
            Assert.AreEqual(actual, expected);
        }

        [TestMethod]
        [TestCategory("Integration.SQL")]
        public void CreateSQL()
        {
            //Arrange.
            var EmployeeRepository = new EmployeeRepository();
            var EmployeesController = new EmployeesController(EmployeeRepository);
            var expected = new Employees()
            {
                FullName = "Чусовлянкин Алексей Александрович",
                Position = "Директор",
                Salary = 100000,
                Status = "активен"
            };

            //Act.
            EmployeesController.Create(expected);
            var id = expected.Id;
            int count = 0;
            using (SqlConnection cn = new SqlConnection { ConnectionString = ConfigurationManager.ConnectionStrings["EmployeesContextTest"].ConnectionString })
            {
                using (SqlCommand cmd = new SqlCommand { Connection = cn })
                {
                    cmd.CommandText = $"select Id, FullName, Position, Status, Salary from [dbo].[list_employees] where Id = {id}";
                    cn.Open();
                    using (SqlDataReader MyReader = cmd.ExecuteReader())
                    {
                        while (MyReader.Read())
                        {
                            string[] row = new string[MyReader.FieldCount];
                            for (int i = 0; i < MyReader.FieldCount; i++)
                            {
                                row[i] = MyReader[i].ToString().Trim();
                                count = 1;
                            }
                        }
                    }
                }
            }

            //Assert.
            Assert.AreEqual(count, 1);
        }
    }
}
