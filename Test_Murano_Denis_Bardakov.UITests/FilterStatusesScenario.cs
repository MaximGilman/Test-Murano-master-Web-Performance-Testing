using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace Test_Murano_Denis_Bardakov.UITests
{
    [TestClass]
    public class FilterStatusesScenario : SeleniumTest
    {
        public FilterStatusesScenario() : base("Test_Murano_Denis_Bardakov") { }
        string base_url = "http://localhost:4688/";
        SqlConnection conn;

        [TestInitialize]
        public void InitializeSqlConnection()
        {
            conn = new SqlConnection() { ConnectionString = ConfigurationManager.
                ConnectionStrings["EmployeesContextTest"].ConnectionString };
            conn.Open();
        }

        [TestCleanup]
        public void CleanupSqlConnection()
        {
            conn.Close();
        }

        public static IEnumerable<object[]> Data
        {
            get
            {
                yield return new object[] { new string[][]
                {
                    new string[4] { "Сидоров Николай Петрович", "Менеджер", "активен", "25000" } ,
                    new string[4] { "Кларов Арсений Евгеньевич", "Бухгалтер", "не активен", "15000" } ,
                    new string[4] { "Палкина Тамара Петровна", "Программист", "активен", "30000" }
                }};
                yield return new object[] { new string[][]
                {
                    new string[4] { "Сидоров Николай Петрович", "Менеджер", "активен", "25000" }
                }};
            }
        }

        public void InitializeDatabase()
        {
            using (SqlCommand cmd = new SqlCommand { Connection = conn })
            {
                cmd.CommandText = @"
                TRUNCATE TABLE [dbo].[list_employees];
                INSERT INTO [dbo].[list_employees] (FullName, Position, Status, Salary)
                VALUES (N'Сидоров Николай Петрович', N'Менеджер', N'активен', 25000),
                        (N'Кларов Арсений Евгеньевич', N'Бухгалтер', N'не активен', 15000),
                        (N'Палкина Тамара Петровна', N'Программист', N'активен', 30000);
                ";
                cmd.ExecuteNonQuery();
            }
        }

        public void InitializeDatabase(string[][] employees)
        {
            var values = "";
            for (int i = 0; i < employees.Count(); i++)
            {
                var employee = employees[i];
                values += $"(N'{employee[0]}', N'{employee[1]}', N'{employee[2]}', {employee[3]})";
                if (i < employees.Count() - 1)
                    values += ", ";

            }
            using (SqlCommand cmd = new SqlCommand { Connection = conn })
            {
                cmd.CommandText = $@"
                TRUNCATE TABLE [dbo].[list_employees];
                INSERT INTO [dbo].[list_employees] (FullName, Position, Status, Salary)
                VALUES {values}
                ";
                cmd.ExecuteNonQuery();
            }
        }


        [TestMethod]
        [TestCategory("System.Interface")]
        public void CheckConnection()
        {
            // Arrange
            var client = new WebClient();
            // Act
            var url = GetAbsoluteUrl();
            Assert.IsNotNull(url);
            var result = client.DownloadString(url);
            // Assert
            Assert.IsNotNull(result);
        }



        [DataTestMethod]
        [TestCategory("System.Interface")]
        [DynamicData(nameof(Data), DynamicDataSourceType.Property)]
        public void FilterAll(string[][] employees)
        {
            InitializeDatabase(employees);
            ChromeDriver.Manage().Window.Maximize();
            ChromeDriver.Navigate().GoToUrl(GetAbsoluteUrl());
            // Search last employee's name.
            var Count = ChromeDriver.FindElement(By.ClassName("table-bordered")).
                FindElements(By.TagName("tr")).Count();

            Assert.AreEqual(Count-1, employees.Count());

            
         

            ChromeDriver.Dispose();
        }


        [DataTestMethod]
        [TestCategory("System.Interface")]
        [DynamicData(nameof(Data), DynamicDataSourceType.Property)]
        public void FilterActive(string[][] employees)
        {
            InitializeDatabase(employees);
            ChromeDriver.Manage().Window.Maximize();
            ChromeDriver.Navigate().GoToUrl(GetAbsoluteUrl());

            ChromeDriver.FindElement(By.Id("filter")).Click();
            ChromeDriver.FindElement(By.Name("submit")).Click();
            // Search last employee's name.
            var Count = ChromeDriver.FindElement(By.ClassName("table-bordered")).
                FindElements(By.TagName("tr")).Count();

            Assert.AreEqual(Count - 1, employees.Where(x=>x[2]=="активен").Count());




            ChromeDriver.Dispose();
        }
        [DataTestMethod]
        [TestCategory("System.Interface")]
        [DynamicData(nameof(Data), DynamicDataSourceType.Property)]
        public void FilterNonActive(string[][] employees)
        {
            InitializeDatabase(employees);
            ChromeDriver.Manage().Window.Maximize();
            ChromeDriver.Navigate().GoToUrl(GetAbsoluteUrl());

            ChromeDriver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='Фильтровать по cтатусу:'])[1]/following::input[2]")).Click(); ChromeDriver.FindElement(By.Name("submit")).Click();
            // Search last employee's name.
            var Count = ChromeDriver.FindElement(By.ClassName("table-bordered")).
                FindElements(By.TagName("tr")).Count();

            if(employees.Where(x => x[2] == "не активен").Count() == 0) {
                Assert.AreEqual(0, employees.Where(x => x[2] == "не активен").Count());
            }
            else
            Assert.AreEqual(Count - 1, employees.Where(x => x[2] == "не активен").Count());




            ChromeDriver.Dispose();
        }


    }
}
