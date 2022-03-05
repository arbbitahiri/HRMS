using System.Collections.Generic;

namespace HRMS.Models.Home;

public class AdministratorVM
{
    public int NumberOfUsers { get; set; }
    public int NumberOfStaff { get; set; }
    public int NumberOfDocuments { get; set; }
    public int NumberOfStaffSubjects { get; set; }
    public int NumberOfAvailableLeave { get; set; }

    public List<User> UserRoles { get; set; }
    public List<LogData> Logs { get; set; }
}

public class User
{
    public string Role { get; set; }
    public int UserCount { get; set; }
}
