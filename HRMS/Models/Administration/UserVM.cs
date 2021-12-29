using System;

namespace HRMS.Models.Administration;
public class UserVM
{
    public string UserId { get; set; }
    public string ProfileImage { get; set; }
    public string PersonalNumber { get; set; }
    public string Name { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Roles { get; set; }
    public DateTimeOffset? LockoutEnd { get; set; }
}
