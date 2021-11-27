using HRMS.Utilities;

namespace HRMS.Models.Shared;
public class UserModel
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string PhoneNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string ImageProfile { get; set; }
    public TemplateMode Mode { get; set; }
    public LanguageEnum Language { get; set; }
    public bool Notification { get; set; }
    public string PersonalNumber { get; set; }
    public string Address { get; set; }
}
