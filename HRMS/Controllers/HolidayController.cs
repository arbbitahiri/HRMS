using HRMS.Data.Core;
using HRMS.Data.General;
using HRMS.Models.Holiday;
using HRMS.Utilities;
using HRMS.Utilities.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.Controllers;
[Authorize]
public class HolidayController : BaseController
{
    public HolidayController(HRMS_WorkContext db, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        : base(db, signInManager, userManager)
    {
    }

    [Authorize("31:r"), Description("Entry form. List of holidays.")]
    public IActionResult Index() => View();

    #region List

    [HttpPost, ValidateAntiForgeryToken, Authorize("31:r")]
    [Description("List of holidays.")]
    public async Task<IActionResult> Search(Search search)
    {
        var holidays = await db.HolidayRequest
            .Include(a => a.HolidayType)
            .Where(a => a.Active
                && a.HolidayTypeId == (search.HolidayTypeId ?? a.HolidayTypeId)
                && a.HolidayRequestStatus.Any(a => a.StatusTypeId == (search.StatusTypeId ?? a.StatusTypeId))
                && (string.IsNullOrEmpty(search.PersonalNumber) || a.StaffDepartment.Staff.PersonalNumber == search.PersonalNumber)
                && (string.IsNullOrEmpty(search.FirstName) || a.StaffDepartment.Staff.FirstName == search.FirstName)
                && (string.IsNullOrEmpty(search.LastName) || a.StaffDepartment.Staff.LastName == search.LastName))
            .Select(a => new List
            {
                HolidayRequestIde = CryptoSecurity.Encrypt(a.HolidayRequestId),
                Firstname = a.StaffDepartment.Staff.FirstName,
                Lastname = a.StaffDepartment.Staff.LastName,
                ProfileImage = a.StaffDepartment.Staff.User.ProfileImage,
                PersonalNumber = a.StaffDepartment.Staff.PersonalNumber,
                HolidayType = user.Language == LanguageEnum.Albanian ? a.HolidayType.NameSq : a.HolidayType.NameEn,
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                StatusType = a.HolidayRequestStatus.Select(a => user.Language == LanguageEnum.Albanian ? a.StatusType.NameSq : a.StatusType.NameEn).LastOrDefault()
            }).ToListAsync();
        return Json(holidays);
    }

    #endregion
}
