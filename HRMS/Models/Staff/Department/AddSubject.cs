using HRMS.Resources;
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
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string StartDate { get; set; }

    [Display(Name = "EndDate", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string EndDate { get; set; }

    [Display(Name = "Active", ResourceType = typeof(Resource))]
    public bool Active { get; set; }
}
