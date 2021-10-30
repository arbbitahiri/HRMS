﻿using HRMS.Data.Core;
using HRMS.Data.General;
using HRMS.Models;
using HRMS.Models.Shared;
using HRMS.Utilities;
using HRMS.Utilities.General;
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
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace HRMS.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        protected readonly SignInManager<ApplicationUser> _signInManager;
        protected readonly UserManager<ApplicationUser> _userManager;
        protected readonly HRMSContext db;
        protected ApplicationUser user;
        protected UserModel userModel;

        public BaseController(HRMSContext _db, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            db = _db;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            user = await _userManager.GetUserAsync(context.HttpContext.User);
            await _signInManager.RefreshSignInAsync(user);

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
                PersonalNumber = user.PersonalNumber
            };

            var log = new Log();
            var con = context.ActionDescriptor as ControllerActionDescriptor;
            DescriptionAttribute description = ((DescriptionAttribute[])con.MethodInfo.GetCustomAttributes(typeof(DescriptionAttribute), true)).FirstOrDefault();

            log.UserId = (User.Identity.IsAuthenticated ? user.Id : null);
            log.Ip = context.HttpContext.Connection.RemoteIpAddress.ToString();
            log.Controller = context.HttpContext.Request.RouteValues["controller"].ToString();
            log.Action = context.HttpContext.Request.RouteValues["action"].ToString();
            log.Description = description.Description;
            log.HttpMethod = context.HttpContext.Request.Method;
            log.Url = context.HttpContext.Request.GetDisplayUrl();
            log.Error = false;
            log.InsertedDate = DateTime.Now;

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
                Url = HttpContext.Request.GetDisplayUrl()
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

        [Description("Change language.")]
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
                await _userManager.UpdateAsync(user);
            }
            return LocalRedirect(returnUrl);
        }

        [HttpPost]
        [Description("Change notification mode.")]
        public async Task<IActionResult> ChangeNotificationMode(bool mode)
        {
            var currentUser = await db.AspNetUsers.FindAsync(user.Id);
            currentUser.AllowNotification = mode;
            await db.SaveChangesAsync();
            return Json(new ErrorVM { Status = ErrorStatus.Success, Description = "Notification mode changed successfully!" });
        }

        [Description("Error status message.")]
        public IActionResult _StatusMessage(ErrorVM error) => PartialView(nameof(_StatusMessage), error);

        protected LanguageEnum GetLanguage(string culture) =>
            culture switch
            {
                "sq-AL" => LanguageEnum.Albanian,
                "en-GB" => LanguageEnum.English,
                _ => LanguageEnum.Albanian,
            };

        protected async Task<string> SaveFile(IWebHostEnvironment environment, IConfiguration configuration, IFormFile file, string folder, int type = 512)
        {
            int maxKB = int.Parse(configuration["AppSettings:FileMaxKB"]);
            string[] imageFormats = configuration["AppSettings:ImagesFormat"].Split(",");

            if (file != null && file.Length > 0 && maxKB * 1024 >= file.Length)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var uploads = Path.Combine(environment.WebRootPath, $"Uploads/{folder}");
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }

                var filePath = Path.Combine(uploads, fileName);
                if (imageFormats.Contains(Path.GetExtension(file.FileName).ToUpper()))
                {
                    await ResizeImage(file, filePath, type);
                }
                else
                {
                    using var fileStream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(fileStream);
                }
                return $"{folder}/{fileName}";
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

        protected string GeneratePassword(IConfiguration configuration, string firstName, string lastName)
        {
            string password = "@";
            if (bool.Parse(configuration["SecurityConfiguration:Password:RequiredUppercase"]))
            {
                password += firstName[..1].ToUpper();
            }
            if (bool.Parse(configuration["SecurityConfiguration:Password:RequiredLowercase"]))
            {
                password += lastName[..1].ToUpper();
            }
            password += ".#";
            return password;
        }
    }
}
