using System.ComponentModel.DataAnnotations;

namespace HRMS.Models.Menu;
public class EditMenu
{
    public string MenuIde { get; set; }

    [Required]
    public string NameSq { get; set; }
    [Required]
    public string NameEn { get; set; }
    public string Controller { get; set; }
    public string Action { get; set; }
    public int OrdinalNumber { get; set; }
    public bool Active { get; set; }
    public string ClaimPolicy { get; set; }
    public bool HasSubMenu { get; set; }
    public string Icon { get; set; }
    public string OpenFor { get; set; }
}

