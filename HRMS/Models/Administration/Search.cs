using System.Collections.Generic;

namespace HRMS.Models.Administration;
public class Search
{
    public List<string> Roles { get; set; }
    public string PersonalNumber { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
}
