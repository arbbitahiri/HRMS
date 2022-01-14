using HRMS.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Models.Administration;
public class Edit
{
    public string UserId { get; set; }

    public IFormFile ProfileImage { get; set; }

    public string ImagePath { get; set; }

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

    [Remote("CheckEmail", "Administration", AdditionalFields = nameof(UserId), ErrorMessageResourceName = "EmailExists", ErrorMessageResourceType = typeof(Resource))]
    [Display(Name = "Email", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    [EmailAddress(ErrorMessageResourceName = "EmailNotValid", ErrorMessageResourceType = typeof(Resource))]
    [StringLength(32, ErrorMessageResourceName = "CharacterLength", ErrorMessageResourceType = typeof(Resource), MinimumLength = 2)]
    public string Email { get; set; }
}
