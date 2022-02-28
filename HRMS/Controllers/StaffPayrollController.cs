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
        var list = await db.StaffDepartment
            .Include(a => a.Staff).Include(a => a.Department)
            .Where(a => a.EndDate >= DateTime.Now
                && a.StaffId == (search.StaffId ?? a.StaffId)
                && a.DepartmentId == (search.DepartmentId ?? a.DepartmentId)
                && a.JobTypeId == (search.JobTypeId ?? a.JobTypeId))
            .Select(a => new PayrollList
            {
                FullName = $"{a.Staff.FirstName} {a.Staff.LastName}",
                PersonalNumber = a.Staff.PersonalNumber,
                Department = user.Language == LanguageEnum.Albanian ? a.Department.NameSq : a.Department.NameEn,
                BruttoSalary = a.BruttoSalary,
                EmployeeContribution = a.EmployeeContribution,
                EmployerContribution = a.EmployerContribution,
                TaxedSalary = a.BruttoSalary - (a.EmployeeContribution ?? 0),
                TotalTax = CalculateTotalTax(a.BruttoSalary, a.JobTypeId),
                NettoSalary = CalculateNettoSalary(a.BruttoSalary, a.JobTypeId)
            }).ToListAsync();
        return Json(list);
    }
}
