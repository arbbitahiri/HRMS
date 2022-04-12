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
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.Controllers;

[Authorize]
public class LeaveController : BaseController
{
    private readonly IEmailSender emailSender;

    public LeaveController(IEmailSender emailSender,
        HRMSContext db, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        : base(db, signInManager, userManager)
    {
        this.emailSender = emailSender;
    }

    [Authorize("31:m"), Description("Arb Tahiri", "Entry form. List of holidays.")]
    public IActionResult Index() => View();

    #region List

    [HttpPost, ValidateAntiForgeryToken, Authorize("31:r")]
    [Description("Arb Tahiri", "List of pending leave.")]
    public async Task<IActionResult> _SearchPending(Search search)
    {
        var manager = await db.StaffDepartment.AnyAsync(a => a.StaffTypeId == (int)StaffTypeEnum.Manager && a.Staff.UserId == user.Id);

        var holidays = await db.Leave
            .Where(a => a.Active && (manager || a.Staff.UserId == user.Id)
                && a.LeaveTypeId == (search.LeaveTypeId ?? a.LeaveTypeId)
                && a.StaffId == (search.StaffId ?? a.StaffId)
                && a.LeaveStatus.Any(b => b.Active && b.StatusTypeId == (int)Status.Pending))
            .Select(a => new List
            {
                LeaveIde = CryptoSecurity.Encrypt(a.LeaveId),
                Firstname = a.Staff.FirstName,
                Lastname = a.Staff.LastName,
                ProfileImage = a.Staff.User.ProfileImage,
                PersonalNumber = a.Staff.PersonalNumber,
                LeaveType = user.Language == LanguageEnum.Albanian ? a.LeaveType.NameSq : a.LeaveType.NameEn,
                StartDate = a.StartDate.ToString("dd/MM/yyyy"),
                ReturnDate = a.EndDate.ToString("dd/MM/yyyy"),
                InsertedDate = a.InsertedDate.ToString("dd/MM/yyyy hh:mm"),
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
            .Where(a => a.Active && a.LeaveStatus.Any(b => b.Active && b.StatusTypeId == (int)Status.Approved)
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
                StartDate = a.StartDate.ToString("dd/MM/yyyy"),
                ReturnDate = a.EndDate.ToString("dd/MM/yyyy"),
                InsertedDate = a.InsertedDate.ToString("dd/MM/yyyy hh:mm"),
                ReviewedDate = a.LeaveStatus.Any(b => b.Active && b.StatusTypeId == (int)Status.Approved) ? a.InsertedDate.ToString("dd/MM/yyyy") : "///"
            }).ToListAsync();
        return PartialView(holidays);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize("31:r")]
    [Description("Arb Tahiri", "List of rejected leave.")]
    public async Task<IActionResult> _SearchRejected(Search search)
    {
        var manager = await db.StaffDepartment.AnyAsync(a => a.StaffTypeId == (int)StaffTypeEnum.Manager && a.Staff.UserId == user.Id);

        var holidays = await db.Leave
            .Where(a => a.Active && a.LeaveStatus.Any(b => b.Active && b.StatusTypeId == (int)Status.Rejected)
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
                StartDate = a.StartDate.ToString("dd/MM/yyyy"),
                ReturnDate = a.EndDate.ToString("dd/MM/yyyy"),
                InsertedDate = a.InsertedDate.ToString("dd/MM/yyyy hh:mm"),
                ReviewedDate = a.LeaveStatus.Any(b => b.Active && b.StatusTypeId == (int)Status.Rejected) ? a.InsertedDate.ToString("dd/MM/yyyy") : "///"
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

        var staff = await db.Staff.Where(a => a.UserId == user.Id).FirstOrDefaultAsync();

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
        int actualDays = (int)await WorkingDays(startDate, endDate);

        if (actualDays < 0)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = string.Format(Resource.NoAvailableDaysLeave, remainingLeaveDays) });
        }

        var leave = new Leave
        {
            LeaveTypeId = create.ALeaveTypeId,
            StaffId = staff.StaffId,
            StartDate = startDate,
            EndDate = endDate,
            RemainingDays = actualDays,
            Description = create.Description,
            Active = true,
            InsertedDate = DateTime.Now,
            InsertedFrom = user.Id
        };
        db.Leave.Add(leave);
        await db.SaveChangesAsync();

        db.LeaveStatus.Add(new LeaveStatus
        {
            LeaveId = leave.LeaveId,
            StatusTypeId = (int)Status.Pending,
            Active = true,
            InsertedDate = DateTime.Now,
            InsertedFrom = user.Id
        });
        await db.SaveChangesAsync();

        //var managerEmail = await db.StaffDepartment.Where(a => a.EndDate >= DateTime.Now && a.StaffTypeId == (int)StaffTypeEnum.Manager).Select(a => a.Staff.User.Email).FirstOrDefaultAsync();
        //var leaveType = await db.LeaveType.Where(a => a.LeaveTypeId == create.ALeaveTypeId).Select(a => user.Language == LanguageEnum.Albanian ? a.NameSq : a.NameEn).FirstOrDefaultAsync();

        //await emailSender.SendEmailAsync(managerEmail, Resource.RequestForLeave, string.Format(Resource.LeaveRequest, leaveType, $"{staff.FirstName} {staff.LastName}", startDate.ToString("dd/MM/yyyy"), endDate.ToString("dd/MM/yyyy")));

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

        int leaveId = CryptoSecurity.Decrypt<int>(review.LeaveIde);
        int staffId = CryptoSecurity.Decrypt<int>(review.StaffIde);

        if (review.StatusTypeId == (int)Status.Approved && review.LeaveTypeEnum != LeaveTypeEnum.Unpaid)
        {
            var leave = await db.Leave.FirstOrDefaultAsync(a => a.Active && a.LeaveId == leaveId);

            var leaveDays = await db.LeaveStaffDays
                .Where(a => a.Active
                    && a.StaffId == staffId
                    && a.LeaveTypeId == leave.LeaveTypeId
                    && a.InsertedDate.Year == DateTime.Now.Year)
                .OrderByDescending(a => a.LeaveStaffDaysId).FirstOrDefaultAsync();

            int remainingLeaveDays = leaveDays != null ? leaveDays.RemainingDays : DaysForLeave((LeaveTypeEnum)leave.LeaveTypeId);
            var days = (int)await WorkingDays(leave.StartDate, leave.EndDate);

            if (remainingLeaveDays - days < 0)
            {
                return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = string.Format(Resource.NoAvailableDaysLeave, remainingLeaveDays) });
            }

            if (leaveDays != null)
            {
                leaveDays.Active = false;
                leaveDays.UpdatedDate = DateTime.Now;
                leaveDays.UpdatedFrom = user.Id;
                leaveDays.UpdatedNo = UpdateNo(leaveDays.UpdatedNo);
            }

            db.LeaveStaffDays.Add(new LeaveStaffDays
            {
                LeaveTypeId = leave.LeaveTypeId,
                StaffId = staffId,
                RemainingDays = leave.RemainingDays,
                Active = true,
                InsertedDate = DateTime.Now,
                InsertedFrom = user.Id
            });

            leave.RemainingDays = leave.RemainingDays;
            leave.UpdatedDate = DateTime.Now;
            leave.UpdatedFrom = user.Id;
            leave.UpdatedNo = UpdateNo(leave.UpdatedNo);
        }

        var leaveStatus = await db.LeaveStatus.FirstOrDefaultAsync(a => a.Active && a.LeaveId == leaveId);
        leaveStatus.Active = false;
        leaveStatus.UpdatedDate = DateTime.Now;
        leaveStatus.UpdatedFrom = user.Id;
        leaveStatus.UpdatedNo = UpdateNo(leaveStatus.UpdatedNo);

        db.LeaveStatus.Add(new LeaveStatus
        {
            LeaveId = leaveId,
            StatusTypeId = review.StatusTypeId,
            Description = review.Description,
            Active = true,
            InsertedDate = DateTime.Now,
            InsertedFrom = user.Id
        });
        await db.SaveChangesAsync();

        //var staffEmail = await db.Staff.Where(a => a.StaffId == staffId).Select(a => a.User.Email).FirstOrDefaultAsync();
        //var leaveType = await db.LeaveType.Where(a => a.LeaveTypeId == leaveId).Select(a => user.Language == LanguageEnum.Albanian ? a.NameSq : a.NameEn).FirstOrDefaultAsync();
        //var statusType = await db.StatusType.Where(a => a.StatusTypeId == review.StatusTypeId).Select(a => user.Language == LanguageEnum.Albanian ? a.NameSq : a.NameEn).FirstOrDefaultAsync();

        //await emailSender.SendEmailAsync(staffEmail, Resource.RequestForLeave, string.Format(Resource.LeaveRequestReview, leaveType, statusType));

        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.LeaveReviewedSuccessfully });
    }

    #endregion

    #region Edit

    [Authorize(Policy = "31:e"), Description("Arb Tahiri", "Form to edit a holiday.")]
    public async Task<IActionResult> _Edit(string ide)
    {
        var leave = await db.Leave
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
        return PartialView(leave);
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
        var days = (int)await WorkingDays(startDate, endDate);
        int actualDays = remainingLeaveDays - days;

        if (remainingLeaveDays - days < 0)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = string.Format(Resource.NoAvailableDaysLeave, remainingLeaveDays) });
        }

        var leave = await db.Leave.FirstOrDefaultAsync(a => a.Active && a.LeaveId == CryptoSecurity.Decrypt<int>(edit.LeaveIde));
        leave.StartDate = startDate;
        leave.EndDate = endDate;
        leave.RemainingDays = (int)(remainingLeaveDays - days);
        leave.Description = edit.Description;
        leave.UpdatedDate = DateTime.Now;
        leave.UpdatedFrom = user.Id;
        leave.UpdatedNo = UpdateNo(leave.UpdatedNo);

        await db.SaveChangesAsync();

        //var staff = await db.Staff.Where(a => a.UserId == user.Id).FirstOrDefaultAsync();
        //var managerEmail = await db.StaffDepartment.Where(a => a.EndDate >= DateTime.Now && a.StaffTypeId == (int)StaffTypeEnum.Manager).Select(a => a.Staff.User.Email).FirstOrDefaultAsync();
        //var leaveType = await db.LeaveType.Where(a => a.LeaveTypeId == edit.ALeaveTypeId).Select(a => user.Language == LanguageEnum.Albanian ? a.NameSq : a.NameEn).FirstOrDefaultAsync();

        //await emailSender.SendEmailAsync(managerEmail, Resource.RequestForLeave, string.Format(Resource.LeaveRequestChanged, leaveType, $"{staff.FirstName} {staff.LastName}", startDate.ToString("dd/MM/yyyy"), endDate.ToString("dd/MM/yyyy")));

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

        int leaveId = CryptoSecurity.Decrypt<int>(ide);

        var leave = await db.Leave.FirstOrDefaultAsync(a => a.Active && a.LeaveId == leaveId);
        leave.Active = false;
        leave.UpdatedDate = DateTime.Now;
        leave.UpdatedFrom = user.Id;
        leave.UpdatedNo = UpdateNo(leave.UpdatedNo);

        var leaveStatus = await db.LeaveStatus.FirstOrDefaultAsync(a => a.Active && a.LeaveId == leaveId);
        leaveStatus.Active = false;
        leaveStatus.UpdatedDate = DateTime.Now;
        leaveStatus.UpdatedFrom = user.Id;
        leaveStatus.UpdatedNo = UpdateNo(leaveStatus.UpdatedNo);

        db.LeaveStatus.Add(new LeaveStatus
        {
            LeaveId = leaveId,
            StatusTypeId = (int)Status.Rejected,
            Active = true,
            InsertedDate = DateTime.Now,
            InsertedFrom = user.Id
        });
        await db.SaveChangesAsync();

        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataDeletedSuccessfully });
    }

    #endregion

    #region Remote

    [Description("Arb Tahiri", "Method to check startdate and enddate.")]
    public async Task<IActionResult> CheckDate(string StartDate, string EndDate, int ALeaveTypeId)
    {
        if (string.IsNullOrEmpty(StartDate) || string.IsNullOrEmpty(EndDate))
        {
            return Json(true);
        }

        var startDate = DateTime.ParseExact(StartDate, "dd/MM/yyyy", null);
        var endDate = DateTime.ParseExact(EndDate, "dd/MM/yyyy", null);

        if (startDate >= endDate)
        {
            return Json(Resource.StartDateVSEndDate);
        }

        var leaveDays = await db.LeaveStaffDays
            .Where(a => a.Active && a.Staff.UserId == user.Id && a.LeaveTypeId == ALeaveTypeId && a.InsertedDate.Year == DateTime.Now.Year)
            .OrderByDescending(a => a.LeaveStaffDaysId).FirstOrDefaultAsync();

        int remainingLeaveDays = leaveDays != null ? leaveDays.RemainingDays : DaysForLeave((LeaveTypeEnum)ALeaveTypeId);
        int days = (int)await WorkingDays(startDate, endDate);

        if (remainingLeaveDays - days >= 0)
        {
            return Json(true);
        }
        else
        {
            return Json(string.Format(Resource.NoAvailableDaysLeave, remainingLeaveDays));
        }
    }

    #endregion

    #region Methods

    [HttpGet, Description("Arb Tahiri", "Method to get remaining days depending on selected holiday type.")]
    public async Task<IActionResult> RemainingDays(int ltypeId, string startDate, string endDate)
    {
        if (ltypeId == 0 || ltypeId == (int)LeaveTypeEnum.Unpaid)
        {
            return Json("");
        }

        DateTime? start = null, end = null;
        if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
        {
            start = DateTime.ParseExact(startDate, "dd/MM/yyyy", null);
            end = DateTime.ParseExact(endDate, "dd/MM/yyyy", null);
        }

        var leave = await db.LeaveStaffDays
            .Where(a => a.Active && a.Staff.UserId == user.Id && a.LeaveTypeId == ltypeId && a.InsertedDate.Year == DateTime.Now.Year)
            .OrderByDescending(a => a.LeaveStaffDaysId).FirstOrDefaultAsync();

        int remainingDays = leave != null ? leave.RemainingDays : DaysForLeave((LeaveTypeEnum)ltypeId);
        if (start.HasValue && end.HasValue)
        {
            remainingDays -= (int)await WorkingDays(start.Value, end.Value);
        }
        return Json(remainingDays);
    }

    #endregion
}
