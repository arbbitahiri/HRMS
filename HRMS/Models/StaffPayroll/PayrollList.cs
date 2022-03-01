namespace HRMS.Models.StaffPayroll;

public class PayrollList
{
    public string StaffName { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string PersonalNumber { get; set; }
    public string ProfileImage { get; set; }
    public string Department { get; set; }
    public decimal Gross { get; set; }
    public decimal EmployeeContribution { get; set; }
    public decimal EmployerContribution { get; set; }
    public string EmployeeContributionS { get; set; }
    public string EmployerContributionS { get; set; }
    public decimal TaxableSalary { get; set; }
    public decimal Tax { get; set; }
    public decimal Net { get; set; }
    public string User { get; set; }
    public string Date { get; set; }
    public string MonthYear { get; set; }
}
