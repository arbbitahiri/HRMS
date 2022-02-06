using HRMS.Data.Core;
using HRMS.Data.General;
using HRMS.Models;
using HRMS.Models.Holiday;
using HRMS.Resources;
using HRMS.Utilities;
using HRMS.Utilities.General;
using HRMS.Utilities.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.Controllers;
[Authorize]
public class HolidayController : BaseController
{
    public HolidayController(HRMSContext db, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        : base(db, signInManager, userManager)
    {
    }

    [Authorize("31:r"), Description("Arb Tahiri", "Entry form. List of holidays.")]
    public IActionResult Index() => View();

    #region List

    [HttpPost, ValidateAntiForgeryToken, Authorize("31:r")]
    [Description("Arb Tahiri", "List of holidays.")]
    public async Task<IActionResult> Search(Search search)
    {
        var manager = await db.StaffDepartment.AnyAsync(a => a.StaffTypeId == (int)StaffTypeEnum.Manager && a.Staff.UserId == user.Id);

        var holidays = await db.Holiday
            .Include(a => a.HolidayType)
            .Where(a => a.Active
                && a.HolidayTypeId == (search.HolidayTypeId ?? a.HolidayTypeId)
                && (string.IsNullOrEmpty(search.PersonalNumber) || a.Staff.PersonalNumber == search.PersonalNumber)
                && (string.IsNullOrEmpty(search.Name) || a.Staff.FirstName == search.Name || a.Staff.LastName == search.Name)
                && (manager || a.Staff.UserId == user.Id))
            .Select(a => new List
            {
                HolidayIde = CryptoSecurity.Encrypt(a.HolidayId),
                Firstname = a.Staff.FirstName,
                Lastname = a.Staff.LastName,
                ProfileImage = a.Staff.User.ProfileImage,
                PersonalNumber = a.Staff.PersonalNumber,
                HolidayType = user.Language == LanguageEnum.Albanian ? a.HolidayType.NameSq : a.HolidayType.NameEn,
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                StatusTypeEnum = (StatusTypeEnum)a.HolidayStatus.Select(a => a.StatusTypeId).FirstOrDefault(),
                InsertedDate = a.InsertedDate,
                UpdatedDate = a.UpdatedDate,
                Finished = a.HolidayStatus.Any(a => a.StatusTypeId != (int)StatusTypeEnum.Pending)
            }).ToListAsync();

        return Json(holidays);
    }

    #endregion

    #region Create

    [Authorize(Policy = "31:c"), Description("Arb Tahiri", "Form to create a holiday")]
    public IActionResult _Create() => PartialView();

    [HttpPost, Authorize(Policy = "31:c"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Form to create a holiday")]
    public async Task<IActionResult> Create(Create create)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        if (await db.Holiday.AnyAsync(a => a.Active && a.Staff.UserId == user.Id && a.HolidayStatus.Any(b => b.Active && b.StatusTypeId == (int)StatusTypeEnum.Pending)))
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.YouHaveHolidayPending });
        }

        if (await db.Holiday.AnyAsync(a => a.Active && a.Staff.UserId == user.Id && a.EndDate >= DateTime.Now && a.HolidayStatus.Any(b => b.Active && b.StatusTypeId == (int)StatusTypeEnum.Approved)))
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.CannotRequestHoliday });
        }

        var staffId = await db.Staff.Where(a => a.UserId == user.Id).Select(a => a.StaffId).FirstOrDefaultAsync();

        var startDate = DateTime.ParseExact(create.StartDate, "dd/MM/yyyy", null);
        var endDate = DateTime.ParseExact(create.EndDate, "dd/MM/yyyy", null);

        if (startDate >= endDate)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.StartDateVSEndDate });
        }

        var days = WorkingDays(startDate, endDate);
        int remainingDays;
        var lastHoliday = await db.Holiday.Where(a => a.Active && a.Staff.UserId == user.Id && a.HolidayTypeId == create.AHolidayTypeId && a.StartDate.Year == DateTime.Now.Year && a.HolidayStatus.Any(b => b.Active && b.StatusTypeId != (int)StatusTypeEnum.Rejected)).OrderBy(a => a.HolidayId).LastOrDefaultAsync();
        remainingDays = lastHoliday != null ? lastHoliday.RemainingDays : 20;

        if (remainingDays - days < 0)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = string.Format(Resource.NoAvailableDaysHoliday, remainingDays) });
        }

        var holiday = new Holiday
        {
            HolidayTypeId = create.AHolidayTypeId,
            StaffId = staffId,
            StartDate = startDate,
            EndDate = endDate,
            RemainingDays = (int)(remainingDays - days),
            Description = create.Description,
            Active = true,
            InsertedDate = DateTime.Now,
            InsertedFrom = user.Id
        };
        db.Holiday.Add(holiday);
        await db.SaveChangesAsync();

        db.HolidayStatus.Add(new HolidayStatus
        {
            HolidayId = holiday.HolidayId,
            StatusTypeId = (int)StatusTypeEnum.Pending,
            Active = true,
            InsertedDate = DateTime.Now,
            InsertedFrom = user.Id
        });
        await db.SaveChangesAsync();

        // TODO: send email
        // TODO: send email to staff for the holiday. Remind of remaining days.

        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataRegisteredSuccessfully });
    }

    #endregion

    #region Review

    [Authorize(Policy = "31r:r"), Description("Arb Tahiri", "Form to review a holiday request.")]
    public async Task<IActionResult> _Review(string ide)
    {
        if (string.IsNullOrEmpty(ide))
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        var holidayReview = await db.Holiday
            .Where(a => a.Active && a.HolidayId == CryptoSecurity.Decrypt<int>(ide))
            .Select(a => new Review
            {
                HolidayIde = ide,
                StaffName = $"{a.Staff.FirstName} {a.Staff.LastName}",
                HolidayType = user.Language == LanguageEnum.Albanian ? a.HolidayType.NameSq : a.HolidayType.NameEn
            }).FirstOrDefaultAsync();

        return PartialView(holidayReview);
    }

    [HttpPost, Authorize(Policy = "31r:r"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Action to review a holiday request.")]
    public async Task<IActionResult> Review(Review review)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        var holidayStatus = await db.HolidayStatus.FirstOrDefaultAsync(a => a.Active && a.HolidayId == CryptoSecurity.Decrypt<int>(review.HolidayIde));
        holidayStatus.StatusTypeId = review.StatusTypeId;
        holidayStatus.Description = review.Description;
        holidayStatus.Active = true;
        holidayStatus.UpdatedDate = DateTime.Now;
        holidayStatus.UpdatedFrom = user.Id;
        holidayStatus.UpdatedNo = holidayStatus.UpdatedNo.HasValue ? ++holidayStatus.UpdatedNo : holidayStatus.UpdatedNo = 1;

        await db.SaveChangesAsync();
        // TODO: send email to inform staff for the holiday request. Remind of remaining days.

        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.HolidayReviewedSuccessfully });
    }

    #endregion

    #region Edit

    [Authorize(Policy = "31:e"), Description("Arb Tahiri", "Form to edit a holiday.")]
    public async Task<IActionResult> _Edit(string ide)
    {
        var holiday = await db.Holiday
            .Where(a => a.Active && a.HolidayId == CryptoSecurity.Decrypt<int>(ide))
            .Select(a => new Create
            {
                HolidayIde = ide,
                AHolidayTypeId = a.HolidayTypeId,
                StartDate = a.StartDate.ToString("dd/MM/yyyy"),
                EndDate = a.EndDate.ToString("dd/MM/yyyy"),
                Description = a.Description,
                RemainingDays = a.RemainingDays,
            }).FirstOrDefaultAsync();
        return PartialView(holiday);
    }

    [HttpPost, Authorize(Policy = "31:e"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Action to edit a holiday.")]
    public async Task<IActionResult> Edit(Create edit)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        if (await db.Holiday.AnyAsync(a => a.Active && a.Staff.UserId == user.Id && a.EndDate >= DateTime.Now && a.HolidayStatus.Any(b => b.Active && b.StatusTypeId == (int)StatusTypeEnum.Approved)))
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.CannotRequestHoliday });
        }

        var startDate = DateTime.ParseExact(edit.StartDate, "dd/MM/yyyy", null);
        var endDate = DateTime.ParseExact(edit.EndDate, "dd/MM/yyyy", null);

        if (startDate >= endDate)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.StartDateVSEndDate });
        }

        var days = WorkingDays(startDate, endDate);
        int remainingDays;
        var lastHoliday = await db.Holiday.Where(a => a.Active && a.Staff.UserId == user.Id && a.HolidayTypeId == edit.AHolidayTypeId && a.StartDate.Year == DateTime.Now.Year && a.HolidayStatus.Any(b => b.Active && b.StatusTypeId != (int)StatusTypeEnum.Rejected)).OrderBy(a => a.HolidayId).LastOrDefaultAsync();
        remainingDays = lastHoliday != null ? lastHoliday.RemainingDays : 20;

        if (remainingDays - days < 0)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = string.Format(Resource.NoAvailableDaysHoliday, remainingDays) });
        }

        var holiday = await db.Holiday.FirstOrDefaultAsync(a => a.Active && a.HolidayId == CryptoSecurity.Decrypt<int>(edit.HolidayIde));
        holiday.StartDate = startDate;
        holiday.EndDate = endDate;
        holiday.RemainingDays = (int)(remainingDays - days);
        holiday.Description = edit.Description;
        holiday.UpdatedDate = DateTime.Now;
        holiday.UpdatedFrom = user.Id;
        holiday.UpdatedNo = holiday.UpdatedNo.HasValue ? ++holiday.UpdatedNo : holiday.UpdatedNo = 1;

        await db.SaveChangesAsync();

        // TODO: send email to manager for the change. Changed date from this to this.
        // TODO: send email to staff for the holiday change. Remind of remaining days.

        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataUpdatedSuccessfully });
    }

    #endregion

    #region Details

    [Authorize(Policy = "31:r"), Description("Arb Tahiri", "Form to view holiday details.")]
    public async Task<IActionResult> _Details(string ide)
    {
        var holiday = await db.Holiday
            .Where(a => a.Active && a.HolidayId == CryptoSecurity.Decrypt<int>(ide))
            .Select(a => new Details
            {
                StaffName = $"{a.Staff.FirstName} {a.Staff.LastName}",
                StaffReviewer = $"{a.HolidayStatus.Select(a => a.UpdatedFromNavigation.FirstName).FirstOrDefault()} {a.HolidayStatus.Select(a => a.UpdatedFromNavigation.LastName).FirstOrDefault()}",
                HolidayType = user.Language == LanguageEnum.Albanian ? a.HolidayType.NameSq : a.HolidayType.NameEn,
                StatusType = a.HolidayStatus.Select(a => user.Language == LanguageEnum.Albanian ? a.StatusType.NameSq : a.StatusType.NameEn).FirstOrDefault(),
                StartDate = a.StartDate.ToString("dd/MM/yyyy"),
                EndDate = a.EndDate.ToString("dd/MM/yyyy"),
                TotalDays = WorkingDays(a.StartDate, a.EndDate),
                InsertedDate = a.InsertedDate.ToString("dd/MM/yyyy"),
                UpdatedDate = a.UpdatedDate.HasValue ? a.UpdatedDate.Value.ToString("dd/MM/yyyy") : "///",
                Description = a.Description
            }).FirstOrDefaultAsync();
        return PartialView(holiday);
    }

    #endregion

    #region Delete

    [HttpPost, Authorize(Policy = "31:d"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Action to edit a holiday.")]
    public async Task<IActionResult> Delete(string ide)
    {
        if (string.IsNullOrEmpty(ide))
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        var holiday = await db.Holiday.FirstOrDefaultAsync(a => a.Active && a.HolidayId == CryptoSecurity.Decrypt<int>(ide));
        holiday.Active = false;
        holiday.UpdatedDate = DateTime.Now;
        holiday.UpdatedFrom = user.Id;
        holiday.UpdatedNo  = holiday.UpdatedNo.HasValue ? ++holiday.UpdatedNo : holiday.UpdatedNo = 1;

        var holidayStatus = await db.HolidayStatus.FirstOrDefaultAsync(a => a.Active && a.HolidayId == CryptoSecurity.Decrypt<int>(ide));
        holidayStatus.StatusTypeId = (int)StatusTypeEnum.Rejected;
        holidayStatus.Active = false;
        holidayStatus.UpdatedDate = DateTime.Now;
        holidayStatus.UpdatedFrom = user.Id;
        holidayStatus.UpdatedNo = holidayStatus.UpdatedNo.HasValue ? ++holidayStatus.UpdatedNo : holidayStatus.UpdatedNo = 1;

        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataDeletedSuccessfully });
    }

    #endregion

    #region Remote

    [Description("Arb Tahiri", "Method to check startdate and enddate.")]
    public async Task<IActionResult> CheckDate(string StartDate, string EndDate, int HolidayTypeId)
    {
        var startDate = DateTime.ParseExact(StartDate, "dd/MM/yyyy", null);
        var endDate = DateTime.ParseExact(EndDate, "dd/MM/yyyy", null);

        if (startDate >= endDate)
        {
            return Json(Resource.StartDateVSEndDate);
        }

        var days = WorkingDays(startDate, endDate);
        int remainingDays;
        var lastHoliday = await db.Holiday.Where(a => a.Active && a.Staff.UserId == user.Id && a.HolidayTypeId == HolidayTypeId && a.StartDate.Year == DateTime.Now.Year && a.HolidayStatus.Any(b => b.Active && b.StatusTypeId != (int)StatusTypeEnum.Rejected)).OrderBy(a => a.HolidayId).LastOrDefaultAsync();
        remainingDays = lastHoliday != null ? lastHoliday.RemainingDays : 20;

        if (remainingDays - days >= 0)
        {
            return Json(true);
        }
        else
        {
            return Json(string.Format(Resource.NoAvailableDaysHoliday, remainingDays));
        }
    }

    #endregion

    #region Methods

    [HttpGet, Description("Arb Tahiri", "Method to get remaining days depending on selected holiday type.")]
    public async Task<IActionResult> RemainingDays(int htypeId, string userId)
    {
        if (htypeId == 0 || string.IsNullOrEmpty(userId))
        {
            return Json(false);
        }

        int remainingDays;
        var lastHoliday = await db.Holiday.Where(a => a.Active && a.Staff.UserId == userId && a.HolidayTypeId == htypeId && a.StartDate.Year == DateTime.Now.Year && a.HolidayStatus.Any(b => b.Active && b.StatusTypeId != (int)StatusTypeEnum.Rejected)).OrderBy(a => a.HolidayId).LastOrDefaultAsync();
        remainingDays = lastHoliday != null ? lastHoliday.RemainingDays : 20;

        return Json(remainingDays);
    }

    public static double WorkingDays(DateTime startDate, DateTime endDate)
    {
        double workingDays = 1 + ((endDate - startDate).TotalDays * 5 - (startDate.DayOfWeek - endDate.DayOfWeek) * 2) / 7;
        if (endDate.DayOfWeek == DayOfWeek.Saturday)
        {
            workingDays--;
        }
        if (startDate.DayOfWeek == DayOfWeek.Sunday)
        {
            workingDays--;
        }
        return workingDays;
    }

    #endregion
}
