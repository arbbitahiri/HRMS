﻿using HRMS.Data.Core;
using HRMS.Data.General;
using HRMS.Models;
using HRMS.Models.Notification;
using HRMS.Models.Shared;
using HRMS.Resources;
using HRMS.Utilities;
using HRMS.Utilities.General;
using HRMS.Utilities.Notifications;
using ImageMagick;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace HRMS.Controllers;

[Authorize]
public class BaseController : Controller
{
    protected readonly SignInManager<ApplicationUser> signInManager;
    protected readonly UserManager<ApplicationUser> userManager;
    protected HRMSContext db;
    protected ApplicationUser user;

    public BaseController(HRMSContext db, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
    {
        this.db = db;
        this.signInManager = signInManager;
        this.userManager = userManager;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        user = await userManager.GetUserAsync(context.HttpContext.User);
        await signInManager.RefreshSignInAsync(user);

        ViewData["InternalUser"] = user;
        ViewData["User"] = new UserModel
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.UserName,
            PhoneNumber = user.PhoneNumber,
            FirstName = user.FirstName,
            LastName = user.LastName,
            ImageProfile = user.ProfileImage,
            Mode = user.Mode,
            Language = user.Language,
            Notification = user.AllowNotification,
            PersonalNumber = user.PersonalNumber,
            DepartmentId = user.DepartmentId
        };

        var con = context.ActionDescriptor as ControllerActionDescriptor;
        DescriptionAttribute description = ((DescriptionAttribute[])con.MethodInfo.GetCustomAttributes(typeof(DescriptionAttribute), true))
            .FirstOrDefault();

        var log = new Log
        {
            UserId = (User.Identity.IsAuthenticated ? user.Id : null),
            Ip = context.HttpContext.Connection.RemoteIpAddress.ToString(),
            Controller = context.HttpContext.Request.RouteValues["controller"].ToString(),
            Action = context.HttpContext.Request.RouteValues["action"].ToString(),
            Developer = description.Developer,
            Description = description.Description,
            HttpMethod = context.HttpContext.Request.Method,
            Url = context.HttpContext.Request.GetDisplayUrl(),
            Error = false,
            InsertedDate = DateTime.Now
        };

        if (context.HttpContext.Request.HasFormContentType)
        {
            IFormCollection form = await context.HttpContext.Request.ReadFormAsync();
            log.FormContent = JsonConvert.SerializeObject(form);
        }

        db.Log.Add(log);
        await db.SaveChangesAsync();

        await next();

        ViewData["Error"] = TempData.Get<ErrorVM>("Error");
    }

    protected async Task LogError(Exception ex)
    {
        var log = new Log
        {
            Error = false,
            UserId = user.Id,
            Ip = HttpContext.Connection.RemoteIpAddress.ToString(),
            Controller = HttpContext.Request.RouteValues["controller"].ToString(),
            Action = HttpContext.Request.RouteValues["action"].ToString(),
            HttpMethod = HttpContext.Request.Method,
            Url = HttpContext.Request.GetDisplayUrl(),
            InsertedDate = DateTime.Now
        };

        if (HttpContext.Request.HasFormContentType)
        {
            IFormCollection form = await HttpContext.Request.ReadFormAsync();
            log.FormContent = JsonConvert.SerializeObject(form);
        }

        log.Exception = JsonConvert.SerializeObject(ex);
        log.Error = true;
        db.Log.Add(log);
        await db.SaveChangesAsync();
    }

    [Description("Arb Tahiri", "Change language.")]
    public async Task<ActionResult> ChangeLanguage(string culture, string returnUrl = "/")
    {
        var cultureInfo = new CultureInfo(culture);
        cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
        cultureInfo.NumberFormat.NumberGroupSeparator = ",";
        Thread.CurrentThread.CurrentUICulture = cultureInfo;
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                HttpOnly = true,
                Secure = true
            });

        if (User.Identity.IsAuthenticated)
        {
            user.Language = GetLanguage(culture);
            await userManager.UpdateAsync(user);
        }
        return LocalRedirect(returnUrl);
    }

    [HttpPost, Description("Arb Tahiri", "Change notification mode.")]
    public async Task<IActionResult> ChangeNotificationMode(bool mode)
    {
        var currentUser = await db.AspNetUsers.FindAsync(user.Id);
        currentUser.AllowNotification = mode;
        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = "Notification mode changed successfully!" });
    }

    [AllowAnonymous, Description("Arb Tahiri", "Error status message.")]
    public IActionResult _StatusMessage(ErrorVM error) => PartialView(nameof(_StatusMessage), error);

    public async Task<List<string>> GetUsers(string role) =>
        await db.AspNetUsers.Where(a => a.Role.Any(a => a.Name == role)).Select(a => a.Id).ToListAsync();

    protected string GetNotificationType(NotificationTypeEnum notificationType) =>
        notificationType switch
        {
            NotificationTypeEnum.Success => "success",
            NotificationTypeEnum.Info => "info",
            NotificationTypeEnum.Warning => "warning",
            NotificationTypeEnum.Error => "error",
            NotificationTypeEnum.Question => "question",
            _ => "info",
        };

    public async Task SendNotification(string title, string description, string target, string url, List<string> users, NotificationUtility notification, NotificationTypeEnum notificationType, string background, string icon) =>
        await notification.SendNotification(user.Id, users, new NotificationSend
        {
            title = title,
            description = description,
            icon = icon,
            url = url,
            target = target,
            background = background,
            notificationType = GetNotificationType(notificationType),
            NotificationType = notificationType
        });

    protected LanguageEnum GetLanguage(string culture) =>
        culture switch
        {
            "sq-AL" => LanguageEnum.Albanian,
            "en-GB" => LanguageEnum.English,
            _ => LanguageEnum.Albanian,
        };

    protected int? UpdateNo(int? updateNo) => updateNo.HasValue ? ++updateNo : 1;

    protected string GetRoleFromStaffType(int staffType) =>
        staffType switch
        {
            (int)StaffTypeEnum.Lecturer => "e062fce5-219c-43df-a6f3-7bc28506259a",
            (int)StaffTypeEnum.Administrator => "6dce687e-0a9c-4bcf-aa79-65c13a8b8db0",
            (int)StaffTypeEnum.Manager => "4d263424-bd57-4f70-9ecc-b95f287cc149",
            _ => ""
        };

    protected int DaysForLeave(LeaveTypeEnum holidayType) =>
        holidayType switch
        {
            LeaveTypeEnum.Annual => 20,
            LeaveTypeEnum.Sick => 20,
            LeaveTypeEnum.Maternity => 365,
            _ => 0
        };

    protected async Task<double> WorkingDays(DateTime startDate, DateTime endDate)
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
        workingDays -= await GetHoliday(startDate, endDate);
        return workingDays;
    }

    public async Task<int> GetHoliday(DateTime startDate, DateTime endDate)
    {
        int days = 0;
        await db.Holiday.Where(a => a.Active && a.Start.Year == startDate.Year && a.HolidayTypeId != (int)HolidayTypeEnum.Other)
            .ForEachAsync(a =>
            {
                if (startDate.Date <= a.Start.Date && a.Start.Date <= endDate.Date
                    && startDate.Date <= a.End.Date && a.End.Date <= endDate.Date)
                {
                    days++;
                }
            });
        return days;
    }

    protected async Task<string> SaveFile(IWebHostEnvironment environment, IConfiguration configuration, IFormFile file, string folder, string fileTitle, int type = 512)
    {
        int maxKB = int.Parse(configuration["AppSettings:FileMaxKB"]);
        string[] imageFormats = configuration["AppSettings:ImagesFormat"].Split(",");

        if (file != null && file.Length > 0 && maxKB * 1024 >= file.Length)
        {
            string fileName = string.IsNullOrEmpty(fileTitle) ? (Guid.NewGuid().ToString() + Path.GetExtension(file.FileName)) : fileTitle;
            string uploads = Path.Combine(environment.WebRootPath, $"Uploads/{folder}");
            if (!Directory.Exists(uploads))
            {
                Directory.CreateDirectory(uploads);
            }

            string filePath = Path.Combine(uploads, fileName);
            if (imageFormats.Contains(Path.GetExtension(file.FileName).ToUpper()))
            {
                await ResizeImage(file, filePath, type);
            }
            else
            {
                using var fileStream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(fileStream);
            }
            return $"/Uploads/{folder}/{fileName}";
        }
        else return null;
    }

    protected async Task ResizeImage(IFormFile file, string filePath, int size)
    {
        var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        stream.Position = 0;

        int width, height;
        var image = Image.FromStream(stream);
        if (image.Width > image.Height)
        {
            width = size;
            height = Convert.ToInt32(image.Height * size / (double)image.Width);
        }
        else
        {
            width = Convert.ToInt32(image.Width * size / (double)image.Height);
            height = size;
        }

        stream.Position = 0;
        var resizer = new MagickImage(stream) { Orientation = OrientationType.TopLeft };
        resizer.Resize(width, height);
        resizer.Write(filePath);
    }

    protected async Task<string> SaveImage(IWebHostEnvironment environment, IFormFile file, string folder, int type = 512)
    {
        //string fileName = Path.GetFileNameWithoutExtension(file.FileName);
        string extension = Path.GetExtension(file.FileName);

        string uploads = Path.Combine(environment.WebRootPath, $"Uploads/{folder}");
        if (!Directory.Exists(uploads))
        {
            Directory.CreateDirectory(uploads);
        }

        string fileName = Guid.NewGuid().ToString() + extension;
        string filePath = Path.Combine(uploads, fileName);
        await ResizeImage(file, filePath, type);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }
        return $"/Uploads/{folder}/{fileName}";
    }

    protected void SendEmailAsync(IConfiguration configuration, string email, string subject, string htmlMessage, string name, bool addHeader = true)
    {
        var smtpClient = new SmtpClient();
        var networkCredential = new NetworkCredential(configuration["EmailConfiguration:Email"], configuration["EmailConfiguration:Password"]);
        var mailMessage = new MailMessage();
        var mailAddress = new MailAddress(configuration["EmailConfiguration:Email"]);

        smtpClient.Host = configuration["EmailConfiguration:Host"];
        smtpClient.UseDefaultCredentials = false;
        smtpClient.Credentials = networkCredential;
        smtpClient.EnableSsl = bool.Parse(configuration["EmailConfig:SSL"]);
        smtpClient.Port = int.Parse(configuration["EmailConfiguration:Port"].ToString());
        mailMessage.From = mailAddress;
        mailMessage.Subject = subject;
        mailMessage.IsBodyHtml = true;
        mailMessage.Body = BodyContent(htmlMessage, name, null, null, addHeader);
        mailMessage.To.Add(email);
        Thread thread = new(async t =>
        {
            try
            {
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                await LogError(ex);
            }
        });
        thread.Start();
    }

    private string BodyContent(string content, string name = "", string title = null, string description = null, bool addHeader = true)
    {
        string email = $"<div>{content}</div>";
        return email;
    }

    protected string FirstTimePassword(IConfiguration configuration, string firstName, string lastName)
    {
        string password = "@1234";
        if (bool.Parse(configuration["SecurityConfiguration:Password:RequiredUppercase"]))
        {
            password += firstName[..1].ToUpper();
        }
        if (bool.Parse(configuration["SecurityConfiguration:Password:RequiredLowercase"]))
        {
            password += lastName[..1].ToLower();
        }
        password += "#";
        return password;
    }

    protected decimal CalculateTotalTax(decimal salary, decimal? employeeContribution, int jobType)
    {
        salary = employeeContribution.HasValue ? salary * (employeeContribution.Value / 100) : salary;
        if (jobType == (int)JobTypeEnum.Secondary)
        {
            return salary * (decimal)0.1;
        }

        decimal totalTax = 0, second, third, fourth;
        switch (salary)
        {
            case >= 450:
                second = 170 * (decimal)0.04;
                third = 200 * (decimal)0.08;
                fourth = (salary - 450) * (decimal)0.1;
                totalTax = second + third + fourth;
                break;
            case >= 250 and < 450:
                second = 170 * (decimal)0.04;
                third = (salary - 250) * (decimal)0.08;
                fourth = 0;
                totalTax = second + third + fourth;
                break;
            case >= 80 and < 250:
                second = (salary - 80) * (decimal)0.04;
                third = 0;
                fourth = 0;
                totalTax = second + third + fourth;
                break;
            case >= 0 and < 80:
                second = 0;
                third = 0;
                fourth = 0;
                totalTax = second + third + fourth;
                break;
        }
        return totalTax;
    }

    protected decimal CalculateNettoSalary(decimal salary, decimal? employeeContribution, int jobType)
    {
        salary = employeeContribution.HasValue ? salary * (employeeContribution.Value / 100) : salary;
        if (jobType == (int)JobTypeEnum.Secondary)
        {
            return salary - (salary * (decimal)0.1);
        }

        decimal second, third, fourth, totalTax, netto = 0;
        switch (salary)
        {
            case >= 450:
                second = 170 * (decimal)0.04;
                third = 200 * (decimal)0.08;
                fourth = (salary - 450) * (decimal)0.1;
                totalTax = second + third + fourth;
                netto = salary - totalTax;
                break;
            case >= 250 and < 450:
                second = 170 * (decimal)0.04;
                third = (salary - 250) * (decimal)0.08;
                fourth = 0;
                totalTax = second + third + fourth;
                netto = salary - totalTax;
                break;
            case >= 80 and < 250:
                second = (salary - 80) * (decimal)0.04;
                third = 0;
                fourth = 0;
                totalTax = second + third + fourth;
                netto = salary - totalTax;
                break;
            case >= 0 and < 80:
                second = 0;
                third = 0;
                fourth = 0;
                totalTax = second + third + fourth;
                netto = salary - totalTax;
                break;
        }
        return netto;
    }

    #region Select list items

    /// <summary>
    /// Method to get first 10 users with specified name
    /// </summary>
    /// <param name="name">Can be first or last name, email or username</param>
    /// <param name="role">Is the selected role in another select list</param>
    /// <returns>First 10 users with the specified condition</returns>
    [HttpPost, ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "List of users for select list")]
    public async Task<IActionResult> AspUsers(string name, string role = "")
    {
        string search = string.IsNullOrEmpty(name) ? "" : name;
        var list = await db.AspNetUsers
            .Where(a => (a.FirstName.ToLower().Contains(search.ToLower()) || a.LastName.ToLower().Contains(search.ToLower()) || a.Email.ToLower().Contains(search.ToLower()) || a.UserName.ToLower().Contains(search.ToLower() ))
                && (string.IsNullOrEmpty(role) || a.Role.Any(b => b.Id == role)))
            .Take(10)
            .Select(a => new Select2
            {
                id = a.Id,
                text = $"{a.FirstName} {a.LastName}",
                image = a.ProfileImage,
                initials = $"{a.FirstName.Substring(0, 1)} {a.LastName.Substring(0, 1)}"
            }).ToListAsync();
        return Json(list);
    }

    /// <summary>
    /// Method to get first 10 staff with specified name
    /// </summary>
    /// <param name="name">Can be first or last name, email or username</param>
    /// <returns>First 10 staff with the specified condition</returns>
    [HttpPost, ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "List of staff for select list")]
    public async Task<IActionResult> GetStaff(string name, string userId)
    {
        string search = string.IsNullOrEmpty(name) ? "" : name;
        var list = await db.Staff
            .Include(a => a.User)
            .Where(a => (a.FirstName.ToLower().Contains(search.ToLower()) || a.LastName.ToLower().Contains(search.ToLower())) && a.UserId != userId)
            .Take(10)
            .Select(a => new Select2
            {
                id = a.StaffId.ToString(),
                text = $"{a.FirstName} {a.LastName}",
                image = a.User.ProfileImage,
                initials = $"{a.FirstName.Substring(0, 1)} {a.LastName.Substring(0, 1)}"
            }).ToListAsync();
        return Json(list);
    }

    /// <summary>
    /// Method to get staff
    /// </summary>
    /// <param staffId="staffId">Staff id from db</param>
    /// <returns>The staff</returns>
    [HttpGet, Description("Arb Tahiri", "List of staff for select list")]
    public async Task<IActionResult> GetCurrentStaff(int staffId) =>
        Json(await db.Staff
            .Include(a => a.User)
            .Where(a => a.StaffId == staffId)
            .Select(a => new Select2
            {
                id = a.StaffId.ToString(),
                text = $"{a.FirstName} {a.LastName}",
                image = a.User.ProfileImage,
                initials = $"{a.FirstName.Substring(0, 1)} {a.LastName.Substring(0, 1)}"
            }).ToListAsync());

    #endregion
}
