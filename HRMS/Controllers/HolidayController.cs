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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.Controllers;

public class HolidayController : BaseController
{
    public HolidayController(HRMSContext db, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        : base(db, signInManager, userManager)
    {
    }

    #region Entry home

    [HttpGet, Authorize(Policy = "9:m"), Description("Arb Tahiri", "Entry form for holiday events.")]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = string.Format(Resource.HolidayCalendarEvents, DateTime.Now.Year);

        var holidays = await db.Holiday
            .Where(a => a.Active)
            .Select(a => new Holidays
            {
                HolidayIde = CryptoSecurity.Encrypt(a.HolidayId),
                HolidayTypeId = a.HolidayTypeId,
                title = a.HolidayTypeId == (int)HolidayTypeEnum.Other ? a.Title : (user.Language == LanguageEnum.Albanian ? a.HolidayType.NameSq : a.HolidayType.NameEn),
                description = a.HolidayTypeId == (int)HolidayTypeEnum.Other ? a.Description : (a.Description ?? (user.Language == LanguageEnum.Albanian ? a.HolidayType.DescriptionSq : a.HolidayType.DescriptionEn)),
                start = a.Start.ToString("yyyy-MM-dd"),
                end = a.End.ToString("yyyy-MM-dd"),
                className = "fc-event-solid-primary fc-event"
            }).ToListAsync();
        return View(holidays);
    }

    [HttpGet, Authorize(Policy = "9:r"), Description("Arb Tahiri", "List of official holidays.")]
    public async Task<IActionResult> OfficialHolidays() =>
        PartialView(await db.HolidayType
            .Where(a => a.Active)
            .Select(a => new HolidayTypes
            {
                HolidayTypeIde = CryptoSecurity.Encrypt(a.HolidayTypeId),
                Title = user.Language == LanguageEnum.Albanian ? a.NameSq : a.NameEn,
                Description = user.Language == LanguageEnum.Albanian ? a.DescriptionSq : a.DescriptionEn
            }).ToListAsync());

    #endregion

    #region Add

    [Authorize(Policy = "9:c"), Description("Arb Tahiri", "Form to add new holiday.")]
    public IActionResult _AddEvent() => PartialView();

    [HttpPost, Authorize(Policy = "9:c"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Action to add new holiday.")]
    public async Task<IActionResult> AddEvent(ManageHoliday holiday)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        DateTime startDate = DateTime.ParseExact(holiday.StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);
        DateTime endDate = DateTime.ParseExact(holiday.EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

        bool weekly = holiday.RepeatTypeId == (int)RepeatTypeEnum.Weekly, monthly = holiday.RepeatTypeId == (int)RepeatTypeEnum.Monthly, anually = holiday.RepeatTypeId == (int)RepeatTypeEnum.Anually;
        int remainingWeeks = (startDate.AddMonths(1).AddDays(-startDate.Day) - startDate).Days / 7;
        int remainingMonths = 12 - endDate.Month;
        int length = weekly ? remainingWeeks : (monthly ? remainingMonths : (anually ? 10 : 0));

        var addHoliday = new List<Holiday>();
        for (int i = 0; i <= length; i++)
        {
            addHoliday.Add(new Holiday
            {
                HolidayTypeId = (int)HolidayTypeEnum.Other,
                Start = weekly ? startDate.AddDays(i * 7) : (monthly ? startDate.AddMonths(i) : anually ? startDate.AddYears(i) : startDate),
                End = weekly ? endDate.AddDays(i * 7) : (monthly ? endDate.AddMonths(i) : anually ? endDate.AddYears(i) : endDate),
                Title = holiday.Title,
                Description = holiday.Description,
                RepeatTypeId = holiday.RepeatTypeId,
                Active = true,
                InsertedDate = DateTime.Now,
                InsertedFrom = user.Id
            });
        }

        await db.Holiday.AddRangeAsync(addHoliday);
        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataRegisteredSuccessfully });
    }

    #endregion

    #region Edit

    [HttpGet, Authorize(Policy = "9:e"), Description("Arb Tahiri", "Form to edit new holiday.")]
    public async Task<IActionResult> _EditEvent(string ide)
    {
        var holidayId = CryptoSecurity.Decrypt<int>(ide);
        var holiday = await db.Holiday
            .Where(a => a.HolidayId == holidayId)
            .Select(a => new ManageHoliday
            {
                HolidayIde = ide,
                Title = a.Title,
                Description = a.Description,
                StartDate = a.Start.ToString("dd/MM/yyyy"),
                EndDate = a.End.ToString("dd/MM/yyyy"),
                RepeatTypeId = a.RepeatTypeId
            }).FirstOrDefaultAsync();
        return PartialView(holiday);
    }

    [HttpPost, Authorize(Policy = "9:e"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Action to edit new holiday.")]
    public async Task<IActionResult> EditEvent(ManageHoliday holiday)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        DateTime startDate = DateTime.ParseExact(holiday.StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);
        DateTime endDate = DateTime.ParseExact(holiday.EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

        var holidayId = CryptoSecurity.Decrypt<int>(holiday.HolidayIde);

        var title = await db.Holiday.Where(a => a.HolidayId == holidayId).Select(a => a.Title).FirstOrDefaultAsync();
        await db.Holiday
            .Where(a => a.Active && a.Title == title)
            .ForEachAsync(a =>
            {
                a.Active = false;
                a.UpdatedDate = DateTime.Now;
                a.UpdatedFrom = user.Id;
                a.UpdatedNo = UpdateNo(a.UpdatedNo);
            });

        bool weekly = holiday.RepeatTypeId == (int)RepeatTypeEnum.Weekly, monthly = holiday.RepeatTypeId == (int)RepeatTypeEnum.Monthly, anually = holiday.RepeatTypeId == (int)RepeatTypeEnum.Anually;
        int remainingWeeks = (startDate.AddMonths(1).AddDays(-startDate.Day) - startDate).Days / 7;
        int remainingMonths = 12 - startDate.Month;
        int length = weekly ? remainingWeeks : (monthly ? remainingMonths : (anually ? 10 : 0));

        var addHoliday = new List<Holiday>();
        for (int i = 0; i <= length; i++)
        {
            addHoliday.Add(new Holiday
            {
                HolidayTypeId = (int)HolidayTypeEnum.Other,
                Start = weekly ? startDate.AddDays(i * 7) : (monthly ? startDate.AddMonths(i) : anually ? startDate.AddYears(i) : startDate),
                End = weekly ? endDate.AddDays(i * 7) : (monthly ? endDate.AddMonths(i) : anually ? endDate.AddYears(i) : endDate),
                Title = holiday.Title,
                Description = holiday.Description,
                RepeatTypeId = holiday.RepeatTypeId,
                Active = true,
                InsertedDate = DateTime.Now,
                InsertedFrom = user.Id
            });
        }

        await db.Holiday.AddRangeAsync(addHoliday);
        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataUpdatedSuccessfully });
    }

    #endregion

    #region Details

    [HttpGet, Authorize(Policy = "9:e"), Description("Arb Tahiri", "Form to display holiday details.")]
    public async Task<IActionResult> _Details(string ide)
    {
        var holidayId = CryptoSecurity.Decrypt<int>(ide);
        var holiday = await db.Holiday
            .Where(a => a.HolidayId == holidayId)
            .Select(a => new ManageHoliday
            {
                HolidayIde = ide,
                Title = a.Title,
                Description = a.Description,
                StartDate = a.Start.ToString("dd/MM/yyyy"),
                EndDate = a.End.ToString("dd/MM/yyyy"),
                RepeatType = user.Language == LanguageEnum.Albanian ? a.RepeatType.NameSq : a.RepeatType.NameEn
            }).FirstOrDefaultAsync();
        return PartialView(holiday);
    }

    #endregion

    #region Remove

    [HttpPost, Authorize(Policy = "9:d"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Action to remove new holiday.")]
    public async Task<IActionResult> Remove(string ide)
    {
        var holidayId = CryptoSecurity.Decrypt<int>(ide);
        var holiday = await db.Holiday.FirstOrDefaultAsync(a => a.HolidayId == holidayId);

        if (holiday.HolidayTypeId == (int)HolidayTypeEnum.Other)
        {
            await db.Holiday
                .Where(a => a.Active && a.Title == holiday.Title)
                .ForEachAsync(a =>
                {
                    a.Active = false;
                    a.UpdatedDate = DateTime.Now;
                    a.UpdatedFrom = user.Id;
                    a.UpdatedNo = UpdateNo(a.UpdatedNo);
                });

            return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataDeletedSuccessfully });
        }

        holiday.Active = false;
        holiday.UpdatedDate = DateTime.Now;
        holiday.UpdatedFrom = user.Id;
        holiday.UpdatedNo = UpdateNo(holiday.UpdatedNo);
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.HolidayRemovedSuccess });
    }

    #endregion

    #region Drag events

    [HttpPost, Authorize(Policy = "9:dg"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Action to drag the holiday.")]
    public async Task<IActionResult> DragHoliday(DragHoliday holiday)
    {
        var holidayTypeId = CryptoSecurity.Decrypt<int>(holiday.HolidayTypeIde);
        var holidayEvent = await db.HolidayType.FirstOrDefaultAsync(a => a.HolidayTypeId == holidayTypeId);

        if (await db.Holiday.AnyAsync(a => a.HolidayTypeId == holidayTypeId && a.Start.Year == holiday.Date.Year))
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.HolidayNotDuplicate });
        }

        db.Holiday.Add(new Holiday
        {
            HolidayTypeId = holidayTypeId,
            Start = holiday.Date,
            End = holiday.Date.AddDays(1),
            Title = holidayEvent.NameSq,
            Description = holidayEvent.DescriptionSq,
            RepeatTypeId = (int)RepeatTypeEnum.Once,
            Active = true,
            InsertedDate = DateTime.Now,
            InsertedFrom = user.Id
        });
        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataRegisteredSuccessfully });
    }

    [HttpPost, Authorize(Policy = "9:dp"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Action to drag the holiday.")]
    public async Task<IActionResult> DropHoliday(DragHoliday holiday)
    {
        var holidayId = CryptoSecurity.Decrypt<int>(holiday.HolidayIde);
        var holidayEvent = await db.Holiday.FirstOrDefaultAsync(a => a.HolidayId == holidayId);
        await db.Holiday
            .Where(a => a.Active && a.Title == holidayEvent.Title)
            .ForEachAsync(a =>
            {
                a.Active = false;
                a.UpdatedDate = DateTime.Now;
                a.UpdatedFrom = user.Id;
                a.UpdatedNo = UpdateNo(a.UpdatedNo);
            });

        bool weekly = holidayEvent.RepeatTypeId == (int)RepeatTypeEnum.Weekly, monthly = holidayEvent.RepeatTypeId == (int)RepeatTypeEnum.Monthly, anually = holidayEvent.RepeatTypeId == (int)RepeatTypeEnum.Anually;
        int remainingWeeks = (holiday.StartDate.AddMonths(1).AddDays(-holiday.StartDate.Day) - holiday.StartDate).Days / 7;
        int remainingMonths = 12 - holiday.StartDate.Month;
        int length = weekly ? remainingWeeks : (monthly ? remainingMonths : (anually ? 10 : 0));

        var addHoliday = new List<Holiday>();
        for (int i = 0; i <= length; i++)
        {
            addHoliday.Add(new Holiday
            {
                HolidayTypeId = (int)HolidayTypeEnum.Other,
                Start = weekly ? holiday.StartDate.AddDays(i * 7) : (monthly ? holiday.StartDate.AddMonths(i) : anually ? holiday.StartDate.AddYears(i) : holiday.StartDate),
                End = weekly ? holiday.EndDate.AddDays(i * 7) : (monthly ? holiday.EndDate.AddMonths(i) : anually ? holiday.EndDate.AddYears(i) : holiday.EndDate),
                Title = holidayEvent.Title,
                Description = holidayEvent.Description,
                RepeatTypeId = holidayEvent.RepeatTypeId,
                Active = true,
                InsertedDate = DateTime.Now,
                InsertedFrom = user.Id
            });
        }

        await db.Holiday.AddRangeAsync(addHoliday);
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataUpdatedSuccessfully });
    }

    #endregion
}
