using HRMS.Utilities;
using System.Collections.Generic;

namespace HRMS.Models.Home.SideProfile;
public class SideProfile
{
    public string Name { get; set; }
    public string ProfileImage { get; set; }
    public string Username { get; set; }
    public TemplateMode Mode { get; set; }
    public List<ProfileRoles> Roles { get; set; }
}

public class ProfileRoles
{
    public string RoleIde { get; set; }
    public string Name { get; set; }
    public string Title { get; set; }
}
