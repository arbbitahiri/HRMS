using HRMS.Utilities;
using System.Collections.Generic;

namespace HRMS.Models.Staff.Department;

public class DepartmentVM
{
    public string StaffIde { get; set; }

    public StaffDetails StaffDetails { get; set; }
    public List<Departments> Departments { get; set; }
    public List<Subjects> Subjects { get; set; }

    public int DepartmentCount { get; set; }
    public int SubjectCount { get; set; }
    public MethodType MethodType { get; set; }
}
