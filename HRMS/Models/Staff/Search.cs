using HRMS.Utilities;

namespace HRMS.Models.Staff;
public class Search
{
    public int? Department { get; set; }
    public int? StaffType { get; set; }
    public string PersonalNumber { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
}
