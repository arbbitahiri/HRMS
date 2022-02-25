namespace HRMS.Models.Staff;

public class ReportVM
{
    public int StaffId { get; set; }
    public int DepartmentId { get; set; }

    public string PersonalNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string BirthDate { get; set; }
    public string Department { get; set; }
    public string Gender { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
}
