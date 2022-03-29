using System;

namespace HRMS.Data.SqlFunctions;

public class SearchHome
{
    public int StaffId { get; set; }
    public string ProfileImage { get; set; }
    public string PersonalNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Department { get; set; }
    public string StaffType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
