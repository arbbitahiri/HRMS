using System.Collections.Generic;

namespace HRMS.Models.Staff.Department;

public class DepartmentVM
{
    public StaffDetails StaffDetails { get; set; }
    public List<Departments> Departments { get; set; }
    public List<Subjects> Subjects { get; set; }

    public int DepartmentCount { get; set; }
    public int SubjectCount { get; set; }
}
