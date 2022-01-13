using HRMS.Resources;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Models.Staff.Qualification;

public class AddQualification
{
    public string StaffIde { get; set; }
    public string StaffQualificationIde { get; set; }

    [Display(Name = "ProfessionType", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public int ProfessionTypeId { get; set; }

    [Display(Name = "EducationLevelType", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public int EducationLevelTypeId { get; set; }

    [Display(Name = "Training", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public bool Training { get; set; }

    [Display(Name = "Title", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string Title { get; set; }

    [Display(Name = "FieldOfStudy", ResourceType = typeof(Resource))]
    public string FieldOfStudy { get; set; }

    [Display(Name = "City", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string City { get; set; }

    [Display(Name = "Country", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string Country { get; set; }

    [Display(Name = "Address", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string Address { get; set; }

    [Display(Name = "From", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string From { get; set; }

    [Display(Name = "To", ResourceType = typeof(Resource))]
    [Remote("CheckDatesQualification", "Staff", AdditionalFields = nameof(From), ErrorMessageResourceName = "StartDateVSEndDate", ErrorMessageResourceType = typeof(Resource))]
    public string To { get; set; }

    [Display(Name = "OnGoing", ResourceType = typeof(Resource))]
    public bool OnGoing { get; set; }

    [Display(Name = "Description", ResourceType = typeof(Resource))]
    public string Description { get; set; }

    [Display(Name = "FinalGrade", ResourceType = typeof(Resource))]
    public decimal? FinalGrade { get; set; }

    [Display(Name = "Thesis", ResourceType = typeof(Resource))]
    public string Thesis { get; set; }

    [Display(Name = "CreditType", ResourceType = typeof(Resource))]
    public string CreditType { get; set; }

    [Display(Name = "CreditNumber", ResourceType = typeof(Resource))]
    public int? CreditNumber { get; set; }

    [Display(Name = "Validity", ResourceType = typeof(Resource))]
    public string Validity { get; set; }
}
