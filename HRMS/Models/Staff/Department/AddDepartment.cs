using System;

namespace HRMS.Models.Staff.Department;

public class AddDepartment
{
    public string StaffIde { get; set; }
    public string StaffCollegeIde { get; set; }

    public int DepartmentId { get; set; }
    public int StaffTypeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Description { get; set; }
}
