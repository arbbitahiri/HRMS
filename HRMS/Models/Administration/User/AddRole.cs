using System.Collections.Generic;

namespace HRMS.Models.Administration;
public class AddRole
{
    public string UserId { get; set; }
    public string Name { get; set; }
    public List<string> Role { get; set; }
    public string Password { get; set; }
}
