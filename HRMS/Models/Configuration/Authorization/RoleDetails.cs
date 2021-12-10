namespace HRMS.Models.Authorization;
public class RoleDetails
{
    public string MenuIde { get; set; }
    public string SubMenuIde { get; set; }
    public string MenuTitle { get; set; }
    public string SubMenuTitle { get; set; }
    public string Icon { get; set; }
    public bool HasSubMenu { get; set; }
    public bool HasAccess { get; set; }
    public string ClaimPolicy { get; set; }
}
