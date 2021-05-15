using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace challenge.Models
{
    public class CompensationDb
    {
        // Compensation database structure; we only want to keep the employee id in the database, not the entire employee model
        public String EmployeeId { get; set; }
        public decimal Salary { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
}
