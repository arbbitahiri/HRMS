using System;
using HRMS.Utilities;

namespace HRMS.Models.Staff;
public class StaffDetails
{
    public string Ide { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string PersonalNumber { get; set; }
    public string Gender { get; set; }
    public string Department { get; set; }
    public string ProfileImage { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string StaffType { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public string ZIP { get; set; }
    public MethodType MethodType { get; set; }
    public bool IsLecturer { get; set; }

    public DateTime InsertedDate { get; set; }
}
