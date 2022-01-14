using HRMS.Resources;
using HRMS.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Models.Administration;
public class Create
{
    [Display(Name = "ProfileImage", ResourceType = typeof(Resource))]
    public IFormFile ProfileImage { get; set; }

    [Display(Name = "PersonalNumber", ResourceType = typeof(Resource))]
    [StringLength(10, ErrorMessageResourceName = "CharacterLength", ErrorMessageResourceType = typeof(Resource), MinimumLength = 10)]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    [RegularExpression(pattern: @"^\d+$", ErrorMessageResourceName = "OnlyNumbers", ErrorMessageResourceType = typeof(Resource))]
    public string PersonalNumber { get; set; }

    public string Username { get; set; }

    [Display(Name = "Firstname", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string Firstname { get; set; }

    [Display(Name = "Lastname", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string Lastname { get; set; }

    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
    [Display(Name = "Birthdate", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string Birthdate { get; set; }

    [Display(Name = "PhoneNumber", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string PhoneNumber { get; set; }

    [Remote("CheckEmail", "Administration", ErrorMessageResourceName = "EmailExists", ErrorMessageResourceType = typeof(Resource))]
    [Display(Name = "Email", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    [EmailAddress(ErrorMessageResourceName = "EmailNotValid", ErrorMessageResourceType = typeof(Resource))]
    [StringLength(32, ErrorMessageResourceName = "CharacterLength", ErrorMessageResourceType = typeof(Resource), MinimumLength = 2)]
    public string Email { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Password", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    [StringLength(32, ErrorMessageResourceName = "CharacterLength", ErrorMessageResourceType = typeof(Resource), MinimumLength = 6)]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "ConfirmPassword", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    [Compare("Password", ErrorMessageResourceName = "PasswordNotMatch", ErrorMessageResourceType = typeof(Resource))]
    public string ConfirmPassword { get; set; }

    [Display(Name = "Language", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public LanguageEnum Language { get; set; }

    [Display(Name = "Roles", ResourceType = typeof(Resource))]
    public List<string> Roles { get; set; }
}
