namespace HRMS.Models.Authorization;
public class ChangeAccess
{
    public string Role { get; set; }
    public string MenuIde { get; set; }
    public string SubMenuIde { get; set; }
    public bool Access { get; set; }
}
