using HRMS.Resources;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Models.Administration.Role;

public class CreateRole
{
    public string RoleIde { get; set; }

    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string NameSQ { get; set; }

    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string NameEN { get; set; }

    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string DescriptionSQ { get; set; }

    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string DescriptionEN { get; set; }
}
