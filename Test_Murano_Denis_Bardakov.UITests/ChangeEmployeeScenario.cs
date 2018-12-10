﻿using System;
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
    public class ChangeEmployeeScenario : SeleniumTest
    {
        public ChangeEmployeeScenario() : base("Test_Murano_Denis_Bardakov") { }
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

        [TestMethod]
        [TestCategory("System.Interface")]
        public void CancelChangeLastEmployee()
        {
            InitializeDatabase();
            ChromeDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
            ChromeDriver.Manage().Window.Maximize();
            ChromeDriver.Navigate().GoToUrl(GetAbsoluteUrl());
            // Search last employee's name.
            var lastRow = ChromeDriver.FindElement(By.ClassName("table-bordered")).
                FindElements(By.TagName("tr")).Last().
                FindElement(By.TagName("td")).Text;
            Assert.IsNotNull(lastRow);
            // Click delete on last employee.
            ChromeDriver.FindElements(By.CssSelector(".btn-danger")).Last().Click();
            // Click return employee list.
            ChromeDriver.FindElements(By.CssSelector(".no-color a")).Last().Click();
            // Check page.
            Assert.IsTrue(ChromeDriver.Title == "Список сотрудников - Моя компания");
            var currentlastRow = ChromeDriver.FindElement(By.ClassName("table-bordered")).
                FindElements(By.TagName("tr")).Last().
                FindElement(By.TagName("td")).Text;
            Assert.AreEqual(currentlastRow, lastRow);
            ChromeDriver.Dispose();
        }

        [DataTestMethod]
        [TestCategory("System.Interface")]
        [DynamicData(nameof(Data), DynamicDataSourceType.Property)]
        public void ChangeLastEmployeeInterface(string[][] employees)
        {
            InitializeDatabase(employees);
            ChromeDriver.Manage().Window.Maximize();
            ChromeDriver.Navigate().GoToUrl(GetAbsoluteUrl());
            // Search last employee's name.
            var lastRow = ChromeDriver.FindElement(By.ClassName("table-bordered")).
                FindElements(By.TagName("tr")).Last().
                FindElement(By.TagName("td")).Text;
            var lastRowPos = ChromeDriver.FindElement(By.ClassName("table-bordered")).
                FindElements(By.TagName("tr")).Last().
                FindElements(By.TagName("td"))[1].Text;
            var Status = ChromeDriver.FindElement(By.ClassName("table-bordered")).
  FindElements(By.TagName("tr")).Last().
  FindElements(By.TagName("td"))[2].Text;
            var lastRowSal = ChromeDriver.FindElement(By.ClassName("table-bordered")).
              FindElements(By.TagName("tr")).Last().
              FindElements(By.TagName("td"))[3].Text;
            Assert.IsNotNull(lastRow);
            // Click delete on last employee.
            ChromeDriver.FindElements(By.CssSelector(".btn-warning")).Last().Click();
            // Check page.
            Assert.IsTrue(ChromeDriver.Title == "Редактирование - Моя компания");
             //Change text

            ChromeDriver.
                FindElement(By.Id("FullName")).SendKeys("1");

            ChromeDriver.
                FindElement(By.Id("Position")).SendKeys("1");
            ChromeDriver.FindElement(By.Id("Salary")).Clear();
            ChromeDriver.
                FindElement(By.Id("Salary")).SendKeys("250010,00");

            ChromeDriver.FindElement(By.Id("Status")).Click();
            new SelectElement(ChromeDriver.FindElement(By.Id("Status"))).SelectByText("не активен");
            ChromeDriver.FindElement(By.Id("Status")).Click();
            // Click edit.

            ChromeDriver.FindElement(By.ClassName("btn-default")).Click();
            // Check page.
            var lastRow1 = ChromeDriver.FindElement(By.ClassName("table-bordered")).
                FindElements(By.TagName("tr")).Last().
                FindElement(By.TagName("td")).Text;
            var lastRowPos1 = ChromeDriver.FindElement(By.ClassName("table-bordered")).
                FindElements(By.TagName("tr")).Last().
                FindElements(By.TagName("td"))[1].Text;
            var Status1 = ChromeDriver.FindElement(By.ClassName("table-bordered")).
  FindElements(By.TagName("tr")).Last().
  FindElements(By.TagName("td"))[2].Text;
            var lastRowSal1 = ChromeDriver.FindElement(By.ClassName("table-bordered")).
              FindElements(By.TagName("tr")).Last().
              FindElements(By.TagName("td"))[3].Text;

            Assert.IsTrue(ChromeDriver.Title == "Список сотрудников - Моя компания");
       
            Assert.AreNotEqual(lastRow1, lastRow);
            Assert.AreNotEqual(lastRowPos, lastRowPos1);

            Assert.AreNotEqual(Status1, Status);
            Assert.AreNotEqual(lastRowSal1, lastRowSal);

            ChromeDriver.Dispose();
        }
        [DataTestMethod]
        [TestCategory("System.Interface")]
        [DynamicData(nameof(Data), DynamicDataSourceType.Property)]
        public void ChangeToEmptyEmployee(string[][] employees)
        {
            InitializeDatabase(employees);
            ChromeDriver.Manage().Window.Maximize();
            ChromeDriver.Navigate().GoToUrl(GetAbsoluteUrl());
            // Search last employee's name.
            var lastRow = ChromeDriver.FindElement(By.ClassName("table-bordered")).
                FindElements(By.TagName("tr")).Last().
                FindElement(By.TagName("td")).Text;
            var lastRowPos = ChromeDriver.FindElement(By.ClassName("table-bordered")).
                FindElements(By.TagName("tr")).Last().
                FindElements(By.TagName("td"))[1].Text;
            var Status = ChromeDriver.FindElement(By.ClassName("table-bordered")).
  FindElements(By.TagName("tr")).Last().
  FindElements(By.TagName("td"))[2].Text;
            var lastRowSal = ChromeDriver.FindElement(By.ClassName("table-bordered")).
              FindElements(By.TagName("tr")).Last().
              FindElements(By.TagName("td"))[3].Text;
            Assert.IsNotNull(lastRow);
            // Click delete on last employee.
            ChromeDriver.FindElements(By.CssSelector(".btn-warning")).Last().Click();
            // Check page.
            Assert.IsTrue(ChromeDriver.Title == "Редактирование - Моя компания");
            //Change text

            ChromeDriver.
                FindElement(By.Id("FullName")).Clear();

            ChromeDriver.
                FindElement(By.Id("Position")).Clear();
            ChromeDriver.FindElement(By.Id("Salary")).Clear();
             

          //НИЗЗЯ - >  ChromeDriver.FindElement(By.Id("Status")).Clear();
            
            // Click edit.

            ChromeDriver.FindElement(By.ClassName("btn-default")).Click();
            // Check page.
            var lastRow1 = ChromeDriver.FindElementByCssSelector(".field-validation-error").
                FindElements(By.TagName("span"))[0].Text;
                        var lastRow2 = ChromeDriver.FindElementByCssSelector(".field-validation-error").
              FindElements(By.TagName("span")).Last().Text;
            var lastRow4 = ChromeDriver.FindElementByCssSelector(".field-validation-error").
               FindElements(By.TagName("span")).Last().Text;



            Assert.IsTrue(ChromeDriver.Title == "Редактирование - Моя компания");

            Assert.AreNotEqual(lastRow,"");
            Assert.AreNotEqual(lastRow2, "");
            Assert.AreNotEqual(lastRow4, "");
            


            ChromeDriver.Dispose();
        }

    }
}
