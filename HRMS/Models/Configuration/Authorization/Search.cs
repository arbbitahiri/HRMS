using System.ComponentModel.DataAnnotations;
using HRMS.Resources;

namespace HRMS.Models.Authorization;
public class Search
{
    [Display(Name = "Role", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string Role { get; set; }
}
