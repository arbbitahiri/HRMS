using HRMS.Utilities;
using System;

namespace HRMS.Models.Staff;
public class StaffPost
{
    public MethodType MethodType { get; set; }
    public string StaffIde { get; set; }

    public string PersonalNumber { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public DateTime BirthDate { get; set; }
    public int Gender { get; set; }
    public string BirthPlace { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public string Address { get; set; }
    public string PostalCode { get; set; }
    public string Nationality { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public LanguageEnum Language { get; set; }
}
