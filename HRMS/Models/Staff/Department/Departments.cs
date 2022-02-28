using System;

namespace HRMS.Models.Staff.Department;

public class Departments
{
    public string StaffDepartmentIde { get; set; }

    public string Department { get; set; }
    public string StaffType { get; set; }
    public string JobType { get; set; }
    public string BruttoSalary { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Description { get; set; }
}
