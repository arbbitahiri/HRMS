namespace HRMS.Models.Administration;
public class SetPassword
{
    public string UserId { get; set; }
    public string Name { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
}
