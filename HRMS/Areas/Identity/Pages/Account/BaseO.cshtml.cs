using HRMS.Data.Core;
using HRMS.Data.General;
using HRMS.Models;
using HRMS.Models.Shared;
using HRMS.Utilities;
using HRMS.Utilities.General;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace HRMS.Areas.Identity.Pages.Account;
public class BaseOModel : PageModel
{
    protected HRMS_WorkContext db;
    protected ApplicationUser user;

    public BaseOModel(HRMS_WorkContext db)
    {
        this.db = db;
    }

    public override async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
    {
        string cultureName = Thread.CurrentThread.CurrentUICulture.Name;
        user = new ApplicationUser
        {
            ProfileImage = "~/images/user-default.png",
            Language = cultureName == "sq-AL" ? LanguageEnum.Albanian : LanguageEnum.English,
        };

        ViewData["InternalUser"] = user;
        ViewData["User"] = new UserModel
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.UserName,
            PhoneNumber = user.PhoneNumber,
            FirstName = user.FirstName ?? "",
            LastName = user.LastName ?? "",
            ImageProfile = user.ProfileImage,
            Mode = user.Mode,
            Language = user.Language,
            Notification = user.AllowNotification,
            PersonalNumber = user.PersonalNumber
        };

        var log = new Log
        {
            Ip = context.HttpContext.Connection.RemoteIpAddress.ToString(),
            Controller = context.HttpContext.Request.RouteValues["area"].ToString(),
            Action = context.HttpContext.Request.RouteValues["page"].ToString(),
            HttpMethod = context.HttpContext.Request.Method,
            Url = context.HttpContext.Request.GetDisplayUrl(),
            Error = false,
            InsertedDate = DateTime.Now
        };

        if (context.HttpContext.Request.HasFormContentType)
        {
            IFormCollection form = await context.HttpContext.Request.ReadFormAsync();
            log.FormContent = JsonConvert.SerializeObject(form.Where(t => t.Key != "Input.Password"));
        }

        var contextExecuted = await next();
        if (contextExecuted.Exception is Exception ex)
        {
            log.Exception = JsonConvert.SerializeObject(ex);
            log.Error = true;

            contextExecuted.ExceptionHandled = true;
            contextExecuted.Result = RedirectToAction("Error");
        }

        db.Log.Add(log);
        await db.SaveChangesAsync();
        ViewData["Error"] = TempData.Get<ErrorVM>("ErrorI");
    }

    public IActionResult OnGetChangeLang(string culture, string returnUrl = "/")
    {
        var cultureInfo = new CultureInfo(culture);
        Thread.CurrentThread.CurrentUICulture = cultureInfo;
        Thread.CurrentThread.CurrentCulture = cultureInfo;
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
        );
        return LocalRedirect(returnUrl);
    }

    protected async Task<string> SaveFile(IWebHostEnvironment environment, IConfiguration configuration, IFormFile file, string folder)
    {
        int maxKB = int.Parse(configuration["AppSettings:FileMaxKB"]);

        if (file != null && file.Length > 0 && maxKB * 1024 >= file.Length)
        {
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var uploads = Path.Combine(environment.WebRootPath, $"Uploads/{folder}");
            var filePath = Path.Combine(uploads, fileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return $"/Uploads/{folder}/{fileName}";
        }
        else
        {
            return null;
        }
    }

    protected LanguageEnum GetLanguage(string culture) =>
        culture switch
        {
            "sq-AL" => LanguageEnum.Albanian,
            "en-GB" => LanguageEnum.English,
            _ => LanguageEnum.Albanian,
        };

    protected void SendEmailAsync(IConfiguration configuration, string email, string subject, string htmlMessage, string name, bool addHeader = true)
    {
        var smtpClient = new SmtpClient();
        var networkCredential = new NetworkCredential(configuration["EmailConfiguration:Email"], configuration["EmailConfiguration:Password"]);
        var mailMessage = new MailMessage();
        var mailAddress = new MailAddress(configuration["EmailConfiguration:Email"]);
        smtpClient.Host = configuration["EmailConfiguration:Host"];
        smtpClient.UseDefaultCredentials = false;
        smtpClient.Credentials = networkCredential;
        smtpClient.EnableSsl = true;
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
            catch
            {

                throw;
            }
        });
        thread.Start();
    }

    private string BodyContent(string content, string name = "", string title = null, string description = null, bool addHeader = true)
    {
        string email = $"<div>{content}</div>";
        return email;
    }
}
