using System;

namespace HRMS.Models.Staff;

public class Subjects
{
    public string StaffDepartmentSubjectIde { get; set; }
    public string Subject { get; set; }
    public string Department { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime InsertDate { get; set; }
}
