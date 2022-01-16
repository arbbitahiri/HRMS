namespace HRMS.Data.SqlFunctions;

public class MenuListAccess
{
    public int MenuId { get; set; }
    public int SubMenuId { get; set; }
    public string Menu { get; set; }
    public string SubMenu { get; set; }
    public string Icon { get; set; }
    public bool HasSubMenu { get; set; }
    public bool HasAccess { get; set; }
    public string ClaimPolicy { get; set; }
}
