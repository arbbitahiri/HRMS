using HRMS.Utilities;
using Microsoft.AspNetCore.Http;
using System;

namespace HRMS.Models.Administration;
public class Edit
{
    public string UserId { get; set; }
    public string ImagePath { get; set; }
    public IFormFile ProfileImage { get; set; }
    public string PersonalNumber { get; set; }
    public string Username { get; set; }
    public string Firsname { get; set; }
    public string Lastname { get; set; }
    public DateTime Birthdate { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public LanguageEnum Language { get; set; }
}
