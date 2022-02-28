using HRMS.Models.StaffPayroll;
using HRMS.Data.Core;
using HRMS.Data.General;
using HRMS.Utilities.General;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;
using HRMS.Utilities;
using System.Collections.Generic;
using Microsoft.Reporting.NETCore;
using HRMS.Resources;

namespace HRMS.Controllers;

[Authorize(Policy = "80:r")]
public class StaffPayrollController : BaseController
{
    public StaffPayrollController(HRMSContext db, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        : base(db, signInManager, userManager)
    {
    }

    [Authorize(Policy = "80:r"), Description("Arb Tahiri", "Entry form to display list of staff payroll.")]
    public IActionResult Index() => View();

    [HttpPost, Authorize(Policy = "81:r"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Action to search for staff payroll.")]
    public async Task<IActionResult> Search(Search search)
    {
        var list = await db.StaffPayroll
            .Include(a => a.Staff).ThenInclude(a => a.User)
            .Include(a => a.Department)
            .Where(a => a.StaffId == (search.StaffId ?? a.StaffId)
                && a.DepartmentId == (search.DepartmentId ?? a.DepartmentId)
                && a.JobTypeId == (search.JobTypeId ?? a.JobTypeId)
                && a.Month == (search.Month ?? a.Month))
            .Select(a => new PayrollList
            {
                Firstname = a.Staff.FirstName,
                Lastname = a.Staff.FirstName,
                PersonalNumber = a.Staff.PersonalNumber,
                ProfileImage = a.Staff.User.ProfileImage,
                Department = user.Language == LanguageEnum.Albanian ? a.Department.NameSq : a.Department.NameEn,
                BruttoSalary = a.BruttoSalary,
                EmployeeContribution = a.EmployeeContribution,
                EmployerContribution = a.EmployerContribution,
                TaxableSalary = a.BruttoSalary - (a.BruttoSalary - a.EmployeeContribution / 100),
                Tax = CalculateTotalTax(a.BruttoSalary, a.EmployeeContribution, a.JobTypeId),
                NetSalary = CalculateNettoSalary(a.BruttoSalary, a.EmployeeContribution, a.JobTypeId)
            }).ToListAsync();
        return Json(list);
    }

    [HttpPost, Authorize(Policy = "81:r"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Report for list of staff depending of the search.")]
    public async Task<IActionResult> Report(Search search, ReportType reportType)
    {
        var list = await db.StaffPayroll
            .Include(a => a.Staff).Include(a => a.Department)
            .Where(a => a.StaffId == (search.StaffId ?? a.StaffId)
                && a.DepartmentId == (search.DepartmentId ?? a.DepartmentId)
                && a.JobTypeId == (search.JobTypeId ?? a.JobTypeId)
                && a.Month == (search.Month ?? a.Month))
            .Select(a => new PayrollList
            {
                FullName = $"{a.Staff.FirstName} {a.Staff.LastName}",
                Department = user.Language == LanguageEnum.Albanian ? a.Department.NameSq : a.Department.NameEn,
                BruttoSalary = a.BruttoSalary,
                EmployeeContribution = a.EmployeeContribution,
                EmployerContribution = a.EmployerContribution,
                TaxableSalary = a.BruttoSalary - (a.BruttoSalary - a.EmployeeContribution / 100),
                Tax = CalculateTotalTax(a.BruttoSalary, a.EmployeeContribution, a.JobTypeId),
                NetSalary = CalculateNettoSalary(a.BruttoSalary, a.EmployeeContribution, a.JobTypeId)
            }).ToListAsync();

        var dataSource = new List<ReportDataSource>() { new ReportDataSource("StaffPayroll", list) };
        var reportByte = RDLCReport.GenerateReport("StaffPayroll.rdlc", reportType, ReportOrientation.Portrait, dataSource);
        string contentType = reportType switch
        {
            ReportType.PDF => "application/pdf",
            ReportType.Excel => "application/ms-excel",
            ReportType.Word => "application/msword",
            _ => "application/pdf"
        };
        string fileName = reportType switch
        {
            ReportType.PDF => Resource.PayrollList,
            ReportType.Excel => $"{Resource.PayrollList}.xlsx",
            ReportType.Word => $"{Resource.PayrollList}.docx",
            _ => Resource.PayrollList
        };
        return reportType == ReportType.PDF ?
            File(reportByte, contentType) :
            File(reportByte, contentType, fileName);
    }
}
