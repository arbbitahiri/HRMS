﻿using HRMS.Data.Core;
using HRMS.Data.General;
using HRMS.Models;
using HRMS.Models.Home;
using HRMS.Repository;
using HRMS.Resources;
using HRMS.Utilities;
using HRMS.Utilities.General;
using HRMS.Utilities.Notifications;
using HRMS.Utilities.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.Controllers;

[Authorize]
public class HomeController : BaseController
{
    private readonly IFunctionRepository function;

    public HomeController(IFunctionRepository function, IHubContext<NotificationHub> hubContext,
        HRMSContext db, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        : base(db, signInManager, userManager)
    {
        this.function = function;
    }

    [Authorize(Policy = "1h:m"), Description("Arb Tahiri", "Entry home.")]
    public IActionResult Index()
    {
        ViewData["Title"] = Resource.HomePage;

        var getRole = User.Claims.FirstOrDefault(a => a.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
        return getRole.Value switch
        {
            "Administrator" => RedirectToAction(nameof(Administrator)),
            "Lecturer" => RedirectToAction(nameof(Lecturer)),
            "Manager" => RedirectToAction(nameof(Manager)),
            _ => View()
        };
    }

    #region Search

    [Description("Arb Tahiri", "Form to search for staff in application.")]
    public async Task<IActionResult> Search(string param)
    {
        var list = (await function.SearchHome(param, user.Language))
            .Select(a => new SearchData
            {
                StaffIde = CryptoSecurity.Encrypt(a.StaffId),
                ProfileImage = a.ProfileImage,
                Name = $"{a.FirstName} {a.LastName}",
                Initials = $"{a.FirstName[..1]} {a.LastName[..1]}",
                PersonalNumber = a.PersonalNumber,
                Department = a.Department,
                StaffType = a.StaffType,
                StayDate = $"{a.StartDate:dd/MM/yyyy} - {a.EndDate:dd/MM/yyyy}"
            }).ToList();
        return PartialView("_SearchResult", list);
    }

    #endregion

    #region Dashboards

    [HttpGet, Description("Arb Tahiri", "Home page for administrator.")]
    public async Task<IActionResult> Administrator()
    {
        ViewData["Title"] = Resource.HomePage;

        var leaveStaffDays = await db.LeaveStaffDays
            .Where(a => a.Active
                && a.Staff.UserId == user.Id
                && a.InsertedDate.Year == DateTime.Now.Year
                && a.LeaveTypeId == (int)LeaveTypeEnum.Annual)
            .ToListAsync();

        int leaveCount = leaveStaffDays.Any() ? leaveStaffDays.Select(a => a.RemainingDays).FirstOrDefault() : 20;

        var dashboard = new AdministratorVM
        {
            NumberOfUsers = await db.AspNetUsers.CountAsync(),
            NumberOfStaff = await db.Staff.CountAsync(),
            NumberOfStaffSubjects = await db.StaffDepartmentSubject.CountAsync(a => a.EndDate >= DateTime.Now),
            NumberOfAvailableLeave = leaveCount,
            UserRoles = await db.AspNetRoles
                .Select(a => new User
                {
                    Role = user.Language == LanguageEnum.Albanian ? a.NameSq : a.NameEn,
                    UserCount = a.User.Count
                }).ToListAsync(),
            Logs = await db.Log
                .Where(a => a.UserId == user.Id)
                .OrderByDescending(a => a.LogId)
                .Take(100).AsSplitQuery()
                .Select(a => new LogData
                {
                    Action = a.Action,
                    Description = a.Description,
                    InsertDate = a.InsertedDate.ToString("dd/MM/yyyy HH:mm")
                }).ToListAsync()
        };
        return View(dashboard);
    }

    [HttpGet, Description("Arb Tahiri", "Home page for lecturer.")]
    public async Task<IActionResult> Lecturer()
    {
        ViewData["Title"] = Resource.HomePage;

        var leaveStaffDays = await db.LeaveStaffDays
            .Where(a => a.Active
                && a.Staff.UserId == user.Id
                && a.InsertedDate.Year == DateTime.Now.Year
                && a.LeaveTypeId == (int)LeaveTypeEnum.Annual)
            .ToListAsync();

        int leaveCount = leaveStaffDays.Any() ? leaveStaffDays.Select(a => a.RemainingDays).FirstOrDefault() : 20;

        var dashboard = new LecturerVM
        {
            NumberOfStaffSubjects = await db.StaffDepartmentSubject.CountAsync(a => a.StaffDepartment.Staff.UserId == user.Id && a.EndDate >= DateTime.Now && a.StaffDepartment.EndDate >= DateTime.Now),
            NumberOfQualifications = await db.StaffQualification.CountAsync(a => a.Staff.UserId == user.Id),
            NumberOfDocuments = await db.StaffDocument.CountAsync(a => a.Active && a.Staff.UserId == user.Id),
            NumberOfAvailableLeave = leaveCount,
            StaffDocuments = await db.StaffDocument
                .Where(a => a.Staff.UserId == user.Id)
                .GroupBy(a => a.DocumentTypeId)
                .Select(a => new Document
                {
                    DocumentType = a.Select(a => user.Language == LanguageEnum.Albanian ? a.DocumentType.NameSq : a.DocumentType.NameEn).FirstOrDefault(),
                    DocumentCount = a.Count()
                }).ToListAsync(),
            Logs = await db.Log
                .Where(a => a.UserId == user.Id)
                .OrderByDescending(a => a.LogId)
                .Take(100).AsSplitQuery()
                .Select(a => new LogData
                {
                    Action = a.Action,
                    Description = a.Description,
                    InsertDate = a.InsertedDate.ToString("dd/MM/yyyy HH:mm")
                }).ToListAsync()
        };
        return View(dashboard);
    }

    [HttpGet, Description("Arb Tahiri", "Home page for manager.")]
    public async Task<IActionResult> Manager()
    {
        ViewData["Title"] = Resource.HomePage;

        var leaveStaffDays = await db.LeaveStaffDays
            .Where(a => a.Active
                && a.Staff.UserId == user.Id
                && a.InsertedDate.Year == DateTime.Now.Year
                && a.LeaveTypeId == (int)LeaveTypeEnum.Annual)
            .ToListAsync();

        int leaveCount = leaveStaffDays.Any() ? leaveStaffDays.Select(a => a.RemainingDays).FirstOrDefault() : 20;

        var dashboard = new ManagerVM
        {
            NumberOfStaff = await db.Staff.CountAsync(),
            NumberOfStaffSubjects = await db.StaffDepartmentSubject.CountAsync(a => a.EndDate >= DateTime.Now),
            NumberOfDocuments = await db.StaffDocument.CountAsync(a => a.Active),
            NumberOfAvailableLeave = leaveCount,
            StaffDepartments = await db.StaffDepartment
                .GroupBy(a => a.DepartmentId)
                .Select(a => new DepartmentStaff
                {
                    Department = a.Select(a => user.Language == LanguageEnum.Albanian ? a.Department.NameSq : a.Department.NameEn).FirstOrDefault(),
                    StaffCount = a.Count()
                }).ToListAsync(),
            Logs = await db.Log
                .Where(a => a.UserId == user.Id)
                .OrderByDescending(a => a.LogId)
                .Take(100).AsSplitQuery()
                .Select(a => new LogData
                {
                    Action = a.Action,
                    Description = a.Description,
                    InsertDate = a.InsertedDate.ToString("dd/MM/yyyy HH:mm")
                }).ToListAsync()
        };
        return View(dashboard);
    }

    #endregion

    #region General methods

    [HttpPost, Description("Arb Tahiri", "Action to change actual role.")]
    public async Task<IActionResult> ChangeRole(string ide)
    {
        string roleId = CryptoSecurity.Decrypt<string>(ide.Replace("\\", ""));
        var realRoles = await db.RealRole.Include(t => t.Role).Where(t => t.UserId == user.Id).ToListAsync();

        if (!realRoles.Any(t => t.RoleId == roleId))
        {
            return Unauthorized();
        }

        var currentRole = User.Claims.Where(t => t.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").FirstOrDefault();
        string role = realRoles.Where(t => t.RoleId == roleId).Select(t => t.Role.Name).FirstOrDefault();

        var error = new ErrorVM { Status = ErrorStatus.Success, Description = string.Format(Resource.RoleChangedSuccess, role) };

        var result = await userManager.RemoveFromRoleAsync(user, currentRole.Value);
        if (!result.Succeeded)
        {
            error = new ErrorVM { Status = ErrorStatus.Error, Description = Resource.ThereWasAnError };
        }
        else
        {
            result = await userManager.AddToRoleAsync(user, role);
            if (!result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, currentRole.Value);
                error = new ErrorVM { Status = ErrorStatus.Error, Description = Resource.ThereWasAnError };
            }
        }
        return Json(error);
    }

    [HttpPost, Description("Arb Tahiri", "Action to change actual role.")]
    public async Task<IActionResult> ChangeMode(bool mode)
    {
        var currentUser = await db.AspNetUsers.FindAsync(user.Id);
        currentUser.Mode = (int)(mode ? TemplateMode.Dark : TemplateMode.Light);
        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success });
    }

    [Route("404"), Description("Arb Tahiri", "When page is not found.")]
    public IActionResult PageNotFound()
    {
        if (HttpContext.Items.ContainsKey("originalPath"))
        {
            _ = HttpContext.Items["originalPath"] as string;
        }
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [Description("Arb Tahiri", "Error view")]
    public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

    #endregion

    #region Notifications

    [HttpPost, Description("Arb Tahiri", "Action to mark as read a notification.")]
    public async Task<IActionResult> MarkAsReadNotification(string ide)
    {
        int id = CryptoSecurity.Decrypt<int>(ide);
        var notification = await db.Notification.FirstOrDefaultAsync(a => a.NotificationId == id);
        notification.Read = !notification.Read;
        await db.SaveChangesAsync();
        return Json(true);
    }

    [HttpPost, Description("Arb Tahiri", "Action to delete a notification.")]
    public async Task<IActionResult> DeleteNotification(string ide)
    {
        int id = CryptoSecurity.Decrypt<int>(ide);
        var notification = await db.Notification.FirstOrDefaultAsync(a => a.NotificationId == id);
        notification.Deleted = true;
        await db.SaveChangesAsync();
        return Json(true);
    }

    [HttpPost, Description("Arb Tahiri", "Action to mark as read all notifications.")]
    public async Task<IActionResult> MarkAsReadAllNotification()
    {
        await db.Notification.Where(a => a.Receiver == user.Id && !a.Read).ForEachAsync(a => a.Read = true);
        await db.SaveChangesAsync();
        return Json(true);
    }

    [HttpPost, Description("Arb Tahiri", "Action to delete all notifications.")]
    public async Task<IActionResult> DeleteAllNotification()
    {
        await db.Notification.Where(a => a.Receiver == user.Id && !a.Deleted).ForEachAsync(a => a.Deleted = true);
        await db.SaveChangesAsync();
        return Json(true);
    }

    #endregion
}
