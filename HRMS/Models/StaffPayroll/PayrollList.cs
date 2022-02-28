namespace HRMS.Models.StaffPayroll;

public class PayrollList
{
    public string FullName { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string PersonalNumber { get; set; }
    public string ProfileImage { get; set; }
    public string Department { get; set; }
    public decimal BruttoSalary { get; set; }
    public decimal EmployeeContribution { get; set; }
    public decimal EmployerContribution { get; set; }
    public decimal TaxableSalary { get; set; }
    public decimal Tax { get; set; }
    public decimal NetSalary { get; set; }
}
