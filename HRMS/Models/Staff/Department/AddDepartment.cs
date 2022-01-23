using HRMS.Resources;
using HRMS.Utilities.Validations;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Models.Staff.Department;

public class AddDepartment
{
    public string StaffIde { get; set; }
    public string StaffDepartmentIde { get; set; }

    [Display(Name = "Department", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public int DepartmentId { get; set; }

    [Display(Name = "StaffType", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public int StaffTypeId { get; set; }

    [Display(Name = "StartDate", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string StartDate { get; set; }

    [Display(Name = "EndDate", ResourceType = typeof(Resource))]
    [Remote("CheckDates", "Staff", AdditionalFields = nameof(StartDate), ErrorMessageResourceName = "StartDateVSEndDate", ErrorMessageResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string EndDate { get; set; }

    [Display(Name = "BruttoSalary", ResourceType = typeof(Resource))]
    [Range(1, int.MaxValue, ErrorMessageResourceName = "SalaryRange", ErrorMessageResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public decimal Salary { get; set; }

    [Display(Name = "EmployeeContribution", ResourceType = typeof(Resource))]
    [IfRange(nameof(Outsider), 5, 15, ErrorMessageResourceName = "EmployeeContributionRange", ErrorMessageResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public decimal? EmployeeContribution { get; set; }

    [Display(Name = "EmployerContribution", ResourceType = typeof(Resource))]
    [IfRange(nameof(Outsider), 5, 15, ErrorMessageResourceName = "EmployerContributionRange", ErrorMessageResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public decimal? EmployerContribution { get; set; }

    [Display(Name = "Description", ResourceType = typeof(Resource))]
    public string Description { get; set; }

    public bool Outsider { get; set; }
}
