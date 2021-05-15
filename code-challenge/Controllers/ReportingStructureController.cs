using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using challenge.Services;
using challenge.Models;
using Microsoft.Azure.KeyVault.Models;

namespace challenge.Controllers
{
    [Route("api/reportingstructure")]
    public class ReportingStructureController : Controller
    {
        private readonly ILogger _logger;
        private readonly IEmployeeService _employeeService;

        public ReportingStructureController(ILogger<ReportingStructureController> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService;
        }

        [HttpGet("{id}", Name = "getNumberOfReportsById")]
        public IActionResult getNumberOfReportsById(String id)
        {
            // TODO: Can we add validation for negative salary? Need to check requirements
            _logger.LogDebug($"Received employee reporting structure get request for '{id}'");

            var employee = _employeeService.GetById(id);

            if (employee == null)
                return NotFound();

            try
            {
                var reportingStructure = new ReportingStructure()
                {
                    Employee = employee,
                    NumberOfReports = RecursiveCountReports(id, new List<Employee>() { employee })
                };
                return Ok(reportingStructure);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                return NoContent();
            }


            //-----------------------------------------------------------------------------------------------------
            // inline functions
            int RecursiveCountReports(string employeeId, List<Employee> directReports)
            {
                var emp = _employeeService.GetById(employeeId);
                var reports = emp.DirectReports;

                if (reports is null)
                    return 0;

                var returnValue = reports.Count;

                foreach(var report in reports)
                {
                    // Check if we already have someone by the same employee id in the list of direct reports, if so error
                    if (directReports.Any(x => x.EmployeeId == report.EmployeeId))
                        throw new OperationCanceledException($"Circular reference detected, cancelling operation; employeeId: {report.EmployeeId}");

                    if (report.DirectReports is null)
                        continue;
                        
                    returnValue += RecursiveCountReports(report.EmployeeId, directReports);
                }
                return returnValue;
            }


        }

    }
}
