using System.Collections.Generic;

namespace HRMS.Models.Shared;
public class MenuVM
{
    public string Title { get; set; }
    public string Area { get; set; }
    public string Controller { get; set; }
    public string Action { get; set; }
    public bool HasSubmenu { get; set; }
    public string Icon { get; set; }
    public string OpenFor { get; set; }

    public List<SubmenuVM> Submenus { get; set; }
}

public class SubmenuVM
{
    public int? SubmenuId { get; set; }
    public int? ParentId { get; set; }
    public string Title { get; set; }
    public string Area { get; set; }
    public string Controller { get; set; }
    public string Action { get; set; }
    public bool Submenu { get; set; }
    public string Icon { get; set; }
    public string OpenFor { get; set; }
}
