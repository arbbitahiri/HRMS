using HRMS.Resources;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Models.Staff.Department;

public class AddSubject
{
    public string StaffDepartmentIde { get; set; }
    public string StaffDepartmentSubjectIde { get; set; }

    [Display(Name = "Subject", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public int SubjectId { get; set; }

    [Display(Name = "StartDate", ResourceType = typeof(Resource))]
    [Remote("CheckDates", "Staff", AdditionalFields = nameof(EndDate), ErrorMessageResourceName = "StartDateVSEndDate", ErrorMessageResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string StartDate { get; set; }

    [Display(Name = "EndDate", ResourceType = typeof(Resource))]
    [Remote("CheckEndDate", "Staff", AdditionalFields = nameof(DepartmentEndDate),ErrorMessageResourceName = "EndDateVSOtherEndDate", ErrorMessageResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string EndDate { get; set; }

    public DateTime DepartmentEndDate { get; set; }

    [Display(Name = "Active", ResourceType = typeof(Resource))]
    public bool Active { get; set; }
}
