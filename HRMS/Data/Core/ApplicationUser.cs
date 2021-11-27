using HRMS.Utilities;
using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Data.Core;
public class ApplicationUser : IdentityUser
{
    public ApplicationUser()
    {
        Mode = TemplateMode.Light;
        InsertedDate = DateTime.Now;
    }

    [Required, StringLength(64)]
    public string PersonalNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime Birthdate { get; set; }
    [StringLength(512)]
    public string ProfileImage { get; set; }
    public TemplateMode Mode { get; set; }
    public LanguageEnum Language { get; set; }
    public bool AllowNotification { get; set; }
    public string InsertedFrom { get; set; }
    public DateTime InsertedDate { get; set; }
}
