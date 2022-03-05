using System.Collections.Generic;

namespace HRMS.Models.Home;

public class ManagerVM
{
    public int NumberOfStaff { get; set; }
    public int NumberOfStaffSubjects { get; set; }
    public int NumberOfDocuments { get; set; }
    public int NumberOfAvailableLeave { get; set; }

    public List<DepartmentStaff> StaffDepartments { get; set; }
    public List<LogData> Logs { get; set; }
}

public class DepartmentStaff
{
    public string Department { get; set; }
    public int StaffCount { get; set; }
}
