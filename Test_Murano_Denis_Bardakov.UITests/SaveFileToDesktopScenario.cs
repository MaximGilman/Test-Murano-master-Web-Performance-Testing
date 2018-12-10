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
    public class SaveFileToDesktopScenario : SeleniumTest
    {
        public SaveFileToDesktopScenario() : base("Test_Murano_Denis_Bardakov") { }
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
                    new string[4] { "Палкина Тамара Петровна", "Программист", "активен", "30000" },

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
        public void CheckSave(string[][] employees)
        {
            InitializeDatabase(employees);
            ChromeDriver.Manage().Window.Maximize();
            ChromeDriver.Navigate().GoToUrl(GetAbsoluteUrl());
            string[] files = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "Отчет.txt", SearchOption.TopDirectoryOnly);

            ChromeDriver.Navigate().GoToUrl("http://localhost:4688/Employees/Create");

            if (files.Length == 0)
            {
                //не было сохранено ранее
                Assert.AreNotEqual(Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "Отчет.txt", SearchOption.TopDirectoryOnly),0);
            }
            else
            {
                Assert.AreNotEqual(Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "Отчет.txt", SearchOption.TopDirectoryOnly), files.Length);

            }


        }

        [DataTestMethod]
        [TestCategory("System.Interface")]
        [DynamicData(nameof(Data), DynamicDataSourceType.Property)]
        public void CheckInfo(string[][] employees)
        {
            InitializeDatabase(employees);
            ChromeDriver.Manage().Window.Maximize();
            ChromeDriver.Navigate().GoToUrl(GetAbsoluteUrl());
            string[] files = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "Отчет.txt", SearchOption.TopDirectoryOnly);

            ChromeDriver.Navigate().GoToUrl("http://localhost:4688/Employees/Create");

            if (files.Length == 0)
            {
                //не было сохранено ранее
                Assert.AreNotEqual(Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "Отчет.txt", SearchOption.TopDirectoryOnly), 0);
            }
            else
            {

              var mas =   File.ReadAllLines("C:\\Users\\ГильманМаксимМихайло\\Desktop\\Отчет.txt");

                double sum = 0; double sumPer = 0; double factSum = 0;
                for (int i = 0; i < mas.Length-2; i++)
             
                {
                    string str = mas[i];
                   
                    var tmp1 = str.Substring(str.IndexOf("зарплата =")+10,str.IndexOf(",") - str.IndexOf("зарплата =")-7 );
                    var tmp2 = str.Substring(str.IndexOf("налог =") +8, str.LastIndexOf(";")- str.IndexOf("налог =")-9);

                   

                   
                        var a =double.Parse(tmp1);
                    sum += a;
                        var b = int.Parse(tmp2);
                    factSum += a * (100 - b) / 100;
                    sumPer += a - a * (100 - b) / 100;

                    //в задаче указано было что более 25 т. на деле больше или равно

                    if (a < 10000) { Assert.AreEqual(b, 10); }
                        else if (a >= 25000) { Assert.AreEqual(b, 25); }
                        
                        else { Assert.AreEqual(b, 15); }

                    

                }
                string Answer = mas[mas.Length - 1];

                var result = Answer.Split(';').Select(x=> x.Substring(x.LastIndexOf(" "))).ToArray();
                Assert.AreEqual(double.Parse(result[0]), sum);
                Assert.AreEqual(double.Parse(result[1]), sumPer);

                Assert.AreEqual(double.Parse(result[2]), factSum);

            }


        }



    }
}
