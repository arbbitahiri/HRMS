using HRMS.Resources;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Models.Configuration.Subject;

public class CreateSubject
{
    public string SubjectIde { get; set; }

    [Display(Name = "Code", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string Code { get; set; }

    [Display(Name = "NameSq", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string NameSq { get; set; }

    [Display(Name = "NameEn", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string NameEn { get; set; }

    [Display(Name = "Active", ResourceType = typeof(Resource))]
    public bool Active { get; set; }
}
