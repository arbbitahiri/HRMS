namespace HRMS.Models.StaffPayroll;

public class PayrollList
{
    public string FullName { get; set; }
    public string PersonalNumber { get; set; }
    public string Department { get; set; }
    public decimal BruttoSalary { get; set; }
    public decimal? EmployeeContribution { get; set; }
    public decimal? EmployerContribution { get; set; }
    public decimal TaxedSalary { get; set; }
    public decimal TotalTax { get; set; }
    public decimal NettoSalary { get; set; }
}
