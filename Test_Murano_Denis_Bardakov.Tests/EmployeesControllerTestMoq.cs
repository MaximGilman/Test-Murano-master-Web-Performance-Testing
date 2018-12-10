using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using Test_Murano_Denis_Bardakov.Controllers;
using Test_Murano_Denis_Bardakov.Models;

namespace Test_Murano_Denis_Bardakov.Tests
{
    [TestClass]
    public class EmployeesControllerTestMoq
    {
        [TestMethod]
        [TestCategory("Integration.Moq.Interaction")]
        public void DeleteById_Interaction()
        {
            // Setup.
            var mock = new Mock<IRepository>();
            var id = 1; 
            mock.Setup(a => a.GetEmployeeById(id)).Returns(new Employees());
            EmployeesController controller = new EmployeesController(mock.Object);

            // Execute.
            var result = controller.Delete(id) as ViewResult;

            // Verify.
            mock.Verify(x => x.GetEmployeeById(id));
        }

        [TestMethod]
        [TestCategory("Integration.Moq.State-based")]
        public void DeleteById_Positive()
        {
            // Arrange.
            var mock = new Mock<IRepository>();
            var id = 1;
            var expected = new Employees { Id = id, FullName = "Фамилия Имя Отчество", Status = "активен", Position = "должность" };
            mock.Setup(a => a.GetEmployeeById(id)).Returns(expected);
            EmployeesController controller = new EmployeesController(mock.Object);

            // Act.
            var result = controller.Delete(id) as ViewResult;

            // Assert
            Assert.AreEqual(expected, result.Model);
        }

        [DataTestMethod]
        [TestCategory("Integration.Moq.State-based")]
        [DataRow(1)]
        [DataRow(null)]
        public void DeleteById_Negative(int? id)
        {
            // Arrange.
            var expected = new int[] { 400, 404 };
            var mock = new Mock<IRepository>();
            mock.Setup(a => a.GetEmployeeById(id)).Returns<Employees>(null);
            EmployeesController controller = new EmployeesController(mock.Object);

            // Act.
            var result = controller.Delete(id) as HttpStatusCodeResult;

            // Assert.
            CollectionAssert.Contains(expected, result.StatusCode);
        }

        [TestMethod]
        [TestCategory("Integration.Moq.Interaction")]
        public void DeleteConfirmed_Interaction()
        {
            // Setup.
            var mock = new Mock<IRepository>();
            var id = 1;
            mock.Setup(a => a.GetEmployeesList()).Returns(new List<Employees>());
            EmployeesController controller = new EmployeesController(mock.Object);

            // Execute.
            controller.DeleteConfirmed(id);

            // Verify.
            mock.Verify(x => x.Delete(id));
            mock.Verify(x => x.Save());
        }

        
        [TestMethod]
        [TestCategory("Integration.Moq.State-based")]
        public void DeleteConfirmed_Postive()
        {
            // Arrange.
            var mock = new Mock<IRepository>();
            var id = 1;
            var expectedRoute = "Index";
            var employeesList = new List<Employees>() {
                new Employees { Id = id, FullName = "Фамилия Имя Отчество", Status = "активен", Position = "должность" }
            };
            mock.Setup(a => a.GetEmployeesList()).Returns(employeesList);
            EmployeesController controller = new EmployeesController(mock.Object);
            var deletedEmployee = mock.Object.GetEmployeeById(id);

            // Act.
            RedirectToRouteResult result = controller.DeleteConfirmed(id) as RedirectToRouteResult;

            // Assert.
            CollectionAssert.DoesNotContain(employeesList, deletedEmployee);
            Assert.AreEqual(expectedRoute, result.RouteValues["action"]);
        }

        [DataTestMethod]
        [TestCategory("Integration.Moq.State-based")]
        [DataRow(2)]
        [DataRow(null)]
        public void DeleteConfirmed_Negative(int id)
        {
            // Arrange.
            var mock = new Mock<IRepository>();
            var expectedRoute = "Index";
            var employeesList = new List<Employees>() {
                new Employees { Id = 1, FullName = "Фамилия Имя Отчество", Status = "активен", Position = "должность" }
            };
            mock.Setup(a => a.GetEmployeesList()).Returns(employeesList);
            EmployeesController controller = new EmployeesController(mock.Object);

            // Act.
            RedirectToRouteResult result = controller.DeleteConfirmed(id) as RedirectToRouteResult;

            // Assert.
            Assert.AreEqual(expectedRoute, result.RouteValues["action"]);
        }
    }
}
