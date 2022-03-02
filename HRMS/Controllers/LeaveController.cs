using HRMS.Data.Core;
using HRMS.Data.General;
using HRMS.Models;
using HRMS.Models.Leave;
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
public class LeaveController : BaseController
{
    public LeaveController(HRMSContext db, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        : base(db, signInManager, userManager)
    {
    }

    [Authorize("31:r"), Description("Arb Tahiri", "Entry form. List of holidays.")]
    public IActionResult Index() => View();

    #region List

    [HttpPost, ValidateAntiForgeryToken, Authorize("31:r")]
    [Description("Arb Tahiri", "List of pending leave.")]
    public async Task<IActionResult> _SearchPending(Search search)
    {
        var manager = await db.StaffDepartment.AnyAsync(a => a.StaffTypeId == (int)StaffTypeEnum.Manager && a.Staff.UserId == user.Id);

        var holidays = await db.Leave
            .Where(a => a.Active && a.LeaveStatus.Any(b => b.StatusTypeId == (int)Status.Pending)
                && a.LeaveTypeId == (search.LeaveTypeId ?? a.LeaveTypeId)
                && a.StaffId == (search.StaffId ?? a.StaffId)
                && (manager || a.Staff.UserId == user.Id))
            .Select(a => new List
            {
                LeaveIde = CryptoSecurity.Encrypt(a.LeaveId),
                Firstname = a.Staff.FirstName,
                Lastname = a.Staff.LastName,
                ProfileImage = a.Staff.User.ProfileImage,
                PersonalNumber = a.Staff.PersonalNumber,
                LeaveType = user.Language == LanguageEnum.Albanian ? a.LeaveType.NameSq : a.LeaveType.NameEn,
                StartDate = a.StartDate,
                ReturnDate = a.EndDate,
                InsertedDate = a.InsertedDate,
                ReviewedDate = a.LeaveStatus.Any(b => b.StatusTypeId == (int)Status.Approved) ? a.UpdatedDate.Value.ToString("dd/MM/yyyy") : "///",
                Finished = a.LeaveStatus.Any(b => b.StatusTypeId != (int)Status.Pending)
            }).ToListAsync();
        return PartialView(holidays);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize("31:r")]
    [Description("Arb Tahiri", "List of approved leave.")]
    public async Task<IActionResult> _SearchApproved(Search search)
    {
        var manager = await db.StaffDepartment.AnyAsync(a => a.StaffTypeId == (int)StaffTypeEnum.Manager && a.Staff.UserId == user.Id);

        var holidays = await db.Leave
            .Where(a => a.Active && a.LeaveStatus.Any(b => b.StatusTypeId == (int)Status.Approved)
                && a.LeaveTypeId == (search.LeaveTypeId ?? a.LeaveTypeId)
                && a.StaffId == (search.StaffId ?? a.StaffId)
                && (manager || a.Staff.UserId == user.Id))
            .Select(a => new List
            {
                LeaveIde = CryptoSecurity.Encrypt(a.LeaveId),
                Firstname = a.Staff.FirstName,
                Lastname = a.Staff.LastName,
                ProfileImage = a.Staff.User.ProfileImage,
                PersonalNumber = a.Staff.PersonalNumber,
                LeaveType = user.Language == LanguageEnum.Albanian ? a.LeaveType.NameSq : a.LeaveType.NameEn,
                StartDate = a.StartDate,
                ReturnDate = a.EndDate,
                InsertedDate = a.InsertedDate,
                ReviewedDate = a.LeaveStatus.Any(b => b.StatusTypeId == (int)Status.Rejected) ? a.UpdatedDate.Value.ToString("dd/MM/yyyy") : "///",
                Finished = a.LeaveStatus.Any(a => a.StatusTypeId != (int)Status.Pending)
            }).ToListAsync();
        return PartialView(holidays);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize("31:r")]
    [Description("Arb Tahiri", "List of rejected leave.")]
    public async Task<IActionResult> _SearchRejected(Search search)
    {
        var manager = await db.StaffDepartment.AnyAsync(a => a.StaffTypeId == (int)StaffTypeEnum.Manager && a.Staff.UserId == user.Id);

        var holidays = await db.Leave
            .Where(a => a.Active && a.LeaveStatus.Any(b => b.StatusTypeId == (int)Status.Rejected)
                && a.LeaveTypeId == (search.LeaveTypeId ?? a.LeaveTypeId)
                && a.StaffId == (search.StaffId ?? a.StaffId)
                && (manager || a.Staff.UserId == user.Id))
            .Select(a => new List
            {
                LeaveIde = CryptoSecurity.Encrypt(a.LeaveId),
                Firstname = a.Staff.FirstName,
                Lastname = a.Staff.LastName,
                ProfileImage = a.Staff.User.ProfileImage,
                PersonalNumber = a.Staff.PersonalNumber,
                LeaveType = user.Language == LanguageEnum.Albanian ? a.LeaveType.NameSq : a.LeaveType.NameEn,
                StartDate = a.StartDate,
                ReturnDate = a.EndDate,
                InsertedDate = a.InsertedDate,
                Finished = a.LeaveStatus.Any(a => a.StatusTypeId != (int)Status.Pending)
            }).ToListAsync();
        return PartialView(holidays);
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

        if (await db.Leave.AnyAsync(a => a.Active && a.Staff.UserId == user.Id && a.LeaveStatus.Any(b => b.Active && b.StatusTypeId == (int)Status.Pending)))
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.YouHaveLeavePending });
        }

        if (await db.Leave.AnyAsync(a => a.Active && a.Staff.UserId == user.Id && a.EndDate >= DateTime.Now && a.LeaveStatus.Any(b => b.Active && b.StatusTypeId == (int)Status.Approved)))
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.CannotRequestLeave });
        }

        var staffId = await db.Staff.Where(a => a.UserId == user.Id).Select(a => a.StaffId).FirstOrDefaultAsync();

        var startDate = DateTime.ParseExact(create.StartDate, "dd/MM/yyyy", null);
        var endDate = DateTime.ParseExact(create.EndDate, "dd/MM/yyyy", null);

        if (startDate >= endDate)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.StartDateVSEndDate });
        }

        var leaveDays = await db.LeaveStaffDays
            .Where(a => a.Active && a.Staff.UserId == user.Id && a.LeaveTypeId == create.ALeaveTypeId && a.InsertedDate.Year == DateTime.Now.Year)
            .OrderByDescending(a => a.LeaveStaffDaysId).FirstOrDefaultAsync();

        int remainingLeaveDays = leaveDays != null ? leaveDays.RemainingDays : DaysForLeave((LeaveTypeEnum)create.ALeaveTypeId);
        var days = WorkingDays(startDate, endDate);
        int actualDays = (int)(leaveDays.RemainingDays - days);

        if (remainingLeaveDays - days < 0)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = string.Format(Resource.NoAvailableDaysLeave, remainingLeaveDays) });
        }

        var holiday = new Leave
        {
            LeaveTypeId = create.ALeaveTypeId,
            StaffId = staffId,
            StartDate = startDate,
            EndDate = endDate,
            RemainingDays = (int)(leaveDays.RemainingDays - days),
            Description = create.Description,
            Active = true,
            InsertedDate = DateTime.Now,
            InsertedFrom = user.Id
        };
        db.Leave.Add(holiday);
        await db.SaveChangesAsync();

        db.LeaveStatus.Add(new LeaveStatus
        {
            LeaveId = holiday.LeaveId,
            StatusTypeId = (int)Status.Pending,
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

        var holidayReview = await db.Leave
            .Where(a => a.Active && a.LeaveId == CryptoSecurity.Decrypt<int>(ide))
            .Select(a => new Review
            {
                LeaveIde = ide,
                StaffIde = CryptoSecurity.Encrypt(a.StaffId),
                StaffName = $"{a.Staff.FirstName} {a.Staff.LastName}",
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                LeaveType = user.Language == LanguageEnum.Albanian ? a.LeaveType.NameSq : a.LeaveType.NameEn,
                LeaveTypeEnum = (LeaveTypeEnum)a.LeaveTypeId
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

        if (review.StatusTypeId == (int)Status.Approved && review.LeaveTypeEnum != LeaveTypeEnum.Unpaid)
        {
            var leaveDays = await db.LeaveStaffDays
                .Where(a => a.Active && a.Staff.UserId == user.Id && a.LeaveTypeId == (int)review.LeaveTypeEnum && a.InsertedDate.Year == DateTime.Now.Year)
                .OrderByDescending(a => a.LeaveStaffDaysId).FirstOrDefaultAsync();

            int remainingLeaveDays = leaveDays != null ? leaveDays.RemainingDays : DaysForLeave(review.LeaveTypeEnum);
            var days = WorkingDays(review.StartDate, review.EndDate);

            if (remainingLeaveDays - days < 0)
            {
                return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = string.Format(Resource.NoAvailableDaysLeave, remainingLeaveDays) });
            }

            db.LeaveStaffDays.Add(new LeaveStaffDays
            {
                LeaveTypeId = (int)review.LeaveTypeEnum,
                StaffId = CryptoSecurity.Decrypt<int>(review.StaffIde),
                RemainingDays = (int)(leaveDays.RemainingDays - days),
                Active = true,
                InsertedDate = DateTime.Now,
                InsertedFrom = user.Id
            });
        }

        var holidayStatus = await db.LeaveStatus.FirstOrDefaultAsync(a => a.Active && a.LeaveId == CryptoSecurity.Decrypt<int>(review.LeaveIde));
        holidayStatus.StatusTypeId = review.StatusTypeId;
        holidayStatus.Description = review.Description;
        holidayStatus.Active = true;
        holidayStatus.UpdatedDate = DateTime.Now;
        holidayStatus.UpdatedFrom = user.Id;
        holidayStatus.UpdatedNo = holidayStatus.UpdatedNo.HasValue ? ++holidayStatus.UpdatedNo : holidayStatus.UpdatedNo = 1;

        await db.SaveChangesAsync();
        // TODO: send email to inform staff for the holiday request. Remind of remaining days.

        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.LeaveReviewedSuccessfully });
    }

    #endregion

    #region Edit

    [Authorize(Policy = "31:e"), Description("Arb Tahiri", "Form to edit a holiday.")]
    public async Task<IActionResult> _Edit(string ide)
    {
        var holiday = await db.Leave
            .Where(a => a.Active && a.LeaveId == CryptoSecurity.Decrypt<int>(ide))
            .Select(a => new Create
            {
                LeaveIde = ide,
                ALeaveTypeId = a.LeaveTypeId,
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

        if (await db.Leave.AnyAsync(a => a.Active && a.Staff.UserId == user.Id && a.EndDate >= DateTime.Now && a.LeaveStatus.Any(b => b.Active && b.StatusTypeId == (int)Status.Approved)))
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.CannotRequestLeave });
        }

        var startDate = DateTime.ParseExact(edit.StartDate, "dd/MM/yyyy", null);
        var endDate = DateTime.ParseExact(edit.EndDate, "dd/MM/yyyy", null);

        if (startDate >= endDate)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.StartDateVSEndDate });
        }

        var leaveDays = await db.LeaveStaffDays
            .Where(a => a.Active && a.Staff.UserId == user.Id && a.LeaveTypeId == edit.ALeaveTypeId && a.InsertedDate.Year == DateTime.Now.Year)
            .OrderByDescending(a => a.LeaveStaffDaysId).FirstOrDefaultAsync();

        int remainingLeaveDays = leaveDays != null ? leaveDays.RemainingDays : DaysForLeave((LeaveTypeEnum)edit.ALeaveTypeId);
        var days = WorkingDays(startDate, endDate);
        int actualDays = (int)(leaveDays.RemainingDays - days);

        if (remainingLeaveDays - days < 0)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = string.Format(Resource.NoAvailableDaysLeave, remainingLeaveDays) });
        }

        var holiday = await db.Leave.FirstOrDefaultAsync(a => a.Active && a.LeaveId == CryptoSecurity.Decrypt<int>(edit.LeaveIde));
        holiday.StartDate = startDate;
        holiday.EndDate = endDate;
        holiday.RemainingDays = (int)(leaveDays.RemainingDays - days);
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
        var holiday = await db.Leave
            .Where(a => a.Active && a.LeaveId == CryptoSecurity.Decrypt<int>(ide))
            .Select(a => new Details
            {
                StaffName = $"{a.Staff.FirstName} {a.Staff.LastName}",
                StaffReviewer = $"{a.LeaveStatus.Select(a => a.UpdatedFromNavigation.FirstName).FirstOrDefault()} {a.LeaveStatus.Select(a => a.UpdatedFromNavigation.LastName).FirstOrDefault()}",
                LeaveType = user.Language == LanguageEnum.Albanian ? a.LeaveType.NameSq : a.LeaveType.NameEn,
                StatusType = a.LeaveStatus.Select(a => user.Language == LanguageEnum.Albanian ? a.StatusType.NameSq : a.StatusType.NameEn).FirstOrDefault(),
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

        var holiday = await db.Leave.FirstOrDefaultAsync(a => a.Active && a.LeaveId == CryptoSecurity.Decrypt<int>(ide));
        holiday.Active = false;
        holiday.UpdatedDate = DateTime.Now;
        holiday.UpdatedFrom = user.Id;
        holiday.UpdatedNo  = holiday.UpdatedNo.HasValue ? ++holiday.UpdatedNo : holiday.UpdatedNo = 1;

        var holidayStatus = await db.LeaveStatus.FirstOrDefaultAsync(a => a.Active && a.LeaveId == CryptoSecurity.Decrypt<int>(ide));
        holidayStatus.StatusTypeId = (int)Status.Rejected;
        holidayStatus.Active = false;
        holidayStatus.UpdatedDate = DateTime.Now;
        holidayStatus.UpdatedFrom = user.Id;
        holidayStatus.UpdatedNo = holidayStatus.UpdatedNo.HasValue ? ++holidayStatus.UpdatedNo : holidayStatus.UpdatedNo = 1;

        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataDeletedSuccessfully });
    }

    #endregion

    #region Remote

    [Description("Arb Tahiri", "Method to check startdate and enddate.")]
    public async Task<IActionResult> CheckDate(string StartDate, string EndDate, int LeaveTypeId)
    {
        var startDate = DateTime.ParseExact(StartDate, "dd/MM/yyyy", null);
        var endDate = DateTime.ParseExact(EndDate, "dd/MM/yyyy", null);

        if (startDate >= endDate)
        {
            return Json(Resource.StartDateVSEndDate);
        }

        var days = WorkingDays(startDate, endDate);
        int remainingDays;
        var lastLeave = await db.Leave.Where(a => a.Active && a.Staff.UserId == user.Id && a.LeaveTypeId == LeaveTypeId && a.StartDate.Year == DateTime.Now.Year && a.LeaveStatus.Any(b => b.Active && b.StatusTypeId != (int)Status.Rejected)).OrderBy(a => a.LeaveId).LastOrDefaultAsync();
        remainingDays = lastLeave != null ? lastLeave.RemainingDays : 20;

        if (remainingDays - days >= 0)
        {
            return Json(true);
        }
        else
        {
            return Json(string.Format(Resource.NoAvailableDaysLeave, remainingDays));
        }
    }

    #endregion

    #region Methods

    [HttpGet, Description("Arb Tahiri", "Method to get remaining days depending on selected holiday type.")]
    public async Task<IActionResult> RemainingDays(int ltypeId, string userId)
    {
        if (ltypeId == 0 || string.IsNullOrEmpty(userId))
        {
            return Json(false);
        }

        var leave = await db.LeaveStaffDays
            .Where(a => a.Active && a.Staff.UserId == userId && a.LeaveTypeId == ltypeId && a.InsertedDate.Year == DateTime.Now.Year)
            .OrderByDescending(a => a.LeaveStaffDaysId).FirstOrDefaultAsync();

        int remainingDays = leave != null ? leave.RemainingDays : DaysForLeave((LeaveTypeEnum)ltypeId);

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
