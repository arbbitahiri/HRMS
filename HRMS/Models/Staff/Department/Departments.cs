using System;

namespace HRMS.Models.Staff.Department;

public class Departments
{
    public string StaffDepartmentIde { get; set; }

    public string Department { get; set; }
    public string StaffType { get; set; }
    public string JobType { get; set; }
    public string BruttoSalary { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public bool IsLecturer { get; set; }
}
