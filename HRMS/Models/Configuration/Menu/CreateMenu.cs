using System.ComponentModel.DataAnnotations;
using HRMS.Resources;

namespace HRMS.Models.Menu;
public class CreateMenu
{
    [Display(Name = "NameSq", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string NameSq { get; set; }

    [Display(Name = "NameEn", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string NameEn { get; set; }

    [Display(Name = "Controller", ResourceType = typeof(Resource))]
    public string Controller { get; set; }

    [Display(Name = "Action", ResourceType = typeof(Resource))]
    public string Action { get; set; }

    [Display(Name = "OrdinalNumber", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public int OrdinalNumber { get; set; }

    [Display(Name = "ClaimPolicy", ResourceType = typeof(Resource))]
    public string ClaimPolicy { get; set; }

    public bool HasSubMenu { get; set; }

    [Display(Name = "Icon", ResourceType = typeof(Resource))]
    public string Icon { get; set; }

    [Display(Name = "OpenFor", ResourceType = typeof(Resource))]
    public string OpenFor { get; set; }
}

