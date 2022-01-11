using HRMS.Resources;
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
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string EndDate { get; set; }

    [Display(Name = "Description", ResourceType = typeof(Resource))]
    public string Description { get; set; }
}
