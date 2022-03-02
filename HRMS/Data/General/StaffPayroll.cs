using System;
using System.Collections.Generic;

namespace HRMS.Data.General
{
    public partial class StaffPayroll
    {
        public int StaffPayrollId { get; set; }
        public int StaffId { get; set; }
        public int DepartmentId { get; set; }
        public int Month { get; set; }
        public int JobTypeId { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal EmployeeContribution { get; set; }
        public decimal EmployerContribution { get; set; }
        public decimal TotalTax { get; set; }
        public decimal NetSalary { get; set; }
        public string InsertedFrom { get; set; }
        public DateTime? InsertedDate { get; set; }

        public virtual Department Department { get; set; }
        public virtual AspNetUsers InsertedFromNavigation { get; set; }
        public virtual JobType JobType { get; set; }
        public virtual Staff Staff { get; set; }
    }
}
