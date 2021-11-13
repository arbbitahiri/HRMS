using HRMS.Data.Core;
using HRMS.Data.General;
using HRMS.Models;
using HRMS.Models.Shared;
using HRMS.Utilities;
using HRMS.Utilities.General;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.Areas.Identity.Pages.Account.Manage
{
    public class BaseIModel : PageModel
    {
        protected readonly SignInManager<ApplicationUser> _signInManager;
        protected readonly UserManager<ApplicationUser> _userManager;
        protected HRMSContext db;
        protected ApplicationUser user;

        public BaseIModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, HRMSContext db)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            this.db = db;
        }

        public override async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            user = await _userManager.GetUserAsync(context.HttpContext.User);
            await _signInManager.RefreshSignInAsync(user);

            if (context.HttpContext.Request.RouteValues["page"].ToString() != "/Account/Manage/ChangePassword")
            {
                TempData.Set("Error", new ErrorVM { Status = ErrorStatus.Info, Description = "You must change your password." });
                context.HttpContext.Response.Redirect("/Identity/Account/Manage/ChangePassword?c=true");
            }

            ViewData["Title"] = "Manage your account.";
            ViewData["IternalUser"] = user;
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
                if (Request.Headers["x-requested-with"] == "XMLHttpRequest")
                {
                    context.Result = new BadRequestResult();
                }
                else
                {
                    context.HttpContext.Response.Redirect("/Home/Error");
                }
            }

            db.Log.Add(log);
            await db.SaveChangesAsync();
            ViewData["Error"] = TempData.Get<ErrorVM>("ErrorI");
        }
    }
}
