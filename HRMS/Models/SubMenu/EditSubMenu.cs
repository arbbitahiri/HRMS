using System.ComponentModel.DataAnnotations;

namespace HRMS.Models.SubMenu;
public class EditSubMenu
{
    public string SubMenuIde { get; set; }

    [Required]
    public string NameSq { get; set; }
    [Required]
    public string NameEn { get; set; }
    public string Controller { get; set; }
    public string Action { get; set; }
    public int OrdinalNumber { get; set; }
    public bool Active { get; set; }
    public string ClaimPolicy { get; set; }
    public string Icon { get; set; }
    public string OpenFor { get; set; }
}
