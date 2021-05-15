using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using challenge.Services;
using challenge.Models;

namespace challenge.Controllers
{
    [Route("api/compensation")]
    public class CompensationController : Controller
    {
        private readonly ILogger _logger;
        private readonly ICompensationService _compensationService;
        private readonly IEmployeeService _employeeService;

        public CompensationController(ILogger<CompensationController> logger, ICompensationService compensationService, IEmployeeService employeeService)
        {
            _logger = logger;
            _compensationService = compensationService;
            _employeeService = employeeService;
        }

        [HttpPost]
        public IActionResult CreateCompensation([FromBody] CompensationDb compensationDb)
        {
            _logger.LogDebug($"Received compensation create request for employee id '{compensationDb.EmployeeId}'");

            var employee = _employeeService.GetById(compensationDb.EmployeeId);

            if (employee == null)
            {
                _logger.LogDebug($"Could not create compensation record for employee id '{compensationDb.EmployeeId}'. Employee not found in the database.");
                return NotFound();
            }

            _logger.LogDebug($"Found employee for compensation request for '{employee.FirstName} {employee.LastName}'");

            _compensationService.Create(compensationDb);

            return CreatedAtRoute("getCompensationByEmployeeId", new { id = compensationDb.EmployeeId }, compensationDb);
        }

        [HttpGet("{id}", Name = "getCompensationByEmployeeId")]
        public IActionResult GetCompensationByEmployeeId(String id)
        {
            // TODO: I'm concerned about using two models here; a DB model and a return model
            //  Can we consolidate this better?
            _logger.LogDebug($"Received compensation get request for '{id}'");

            var employee = _employeeService.GetById(id);

            if (employee == null)
            {
                _logger.LogDebug($"Could not retrieve compensation record for employee id '{id}'. Employee not found in the database.");
                return NotFound();
            }

            var compensationDbRecord = _compensationService.GetById(id);

            if (compensationDbRecord == null)
                return NotFound();

            var newCompensation = new Compensation()
            {
                Employee = employee,
                Salary = compensationDbRecord.Salary,
                EffectiveDate = compensationDbRecord.EffectiveDate
            };

            return Ok(newCompensation);
        }
    }
}
