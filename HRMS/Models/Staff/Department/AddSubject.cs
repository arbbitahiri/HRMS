using System;

namespace HRMS.Models.Staff.Department;

public class AddSubject
{
    public string StaffDepartmentIde { get; set; }
    public string StaffDepartmentSubjectIde { get; set; }

    public int SubjectId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool Active { get; set; }
}
