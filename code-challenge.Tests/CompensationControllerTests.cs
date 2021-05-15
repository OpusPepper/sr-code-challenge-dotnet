using challenge.Controllers;
using challenge.Data;
using challenge.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using code_challenge.Tests.Integration.Extensions;

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using code_challenge.Tests.Integration.Helpers;
using System.Text;

namespace code_challenge.Tests.Integration
{
    [TestClass]
    public class CompensationControllerTests
    {
        private static HttpClient _httpClient;
        private static TestServer _testServer;

        [ClassInitialize]
        public static void InitializeClass(TestContext context)
        {
            _testServer = new TestServer(WebHost.CreateDefaultBuilder()
                .UseStartup<TestServerStartup>()
                .UseEnvironment("Development"));

            _httpClient = _testServer.CreateClient();
        }

        [ClassCleanup]
        public static void CleanUpTest()
        {
            _httpClient.Dispose();
            _testServer.Dispose();
        }

        [TestMethod]
        public void CreateAndGetCompensationById_Returns_Ok()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var expectedFirstName = "John";
            var expectedLastName = "Lennon";

            var compensationDbRecord = new CompensationDb()
            {
                EmployeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f",
                Salary = 50000,
                EffectiveDate = new DateTime(2021, 05, 14)
            };

            var requestContent = new JsonSerialization().ToJson(compensationDbRecord);

            // Act
            // First we need to create the compensation record
            var postRequestTask = _httpClient.PostAsync("api/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response1 = postRequestTask.Result;

            // Now we get the compensation record
            // Compensation object is different than CompensationDb object because Compensation also gives the full
            //  Employee object also
            var getRequestTask = _httpClient.GetAsync($"api/compensation/{employeeId}");
            var response2 = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response1.StatusCode);
            var newCompensation = response1.DeserializeContent<CompensationDb>();
            Assert.IsNotNull(newCompensation);
            Assert.AreEqual(compensationDbRecord.Salary, newCompensation.Salary);
            Assert.AreEqual(compensationDbRecord.EffectiveDate, newCompensation.EffectiveDate);

            Assert.AreEqual(HttpStatusCode.OK, response2.StatusCode);
            var compensation = response2.DeserializeContent<Compensation>();
            Assert.IsNotNull(compensation);
            Assert.IsNotNull(compensation.Employee);
            Assert.AreEqual(expectedFirstName, compensation.Employee.FirstName);
            Assert.AreEqual(expectedLastName, compensation.Employee.LastName);
        }

        [TestMethod]
        public void GetCompensationById_EmployeeIdDoesNotExist_Returns_NotFound()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86g";

            // Act
            var getRequestTask = _httpClient.GetAsync($"api/compensation/{employeeId}");
            var response = getRequestTask.Result;

            // Assert            
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            var compensation = response.DeserializeContent<Compensation>();
            Assert.IsNull(compensation);
        }
    }
}
