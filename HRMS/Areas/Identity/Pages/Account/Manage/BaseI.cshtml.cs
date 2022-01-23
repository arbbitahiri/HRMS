using HRMS.Data.Core;
using HRMS.Data.General;
using HRMS.Models;
using HRMS.Models.Shared;
using HRMS.Utilities;
using HRMS.Utilities.General;
using ImageMagick;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.Areas.Identity.Pages.Account.Manage;
public class BaseIModel : PageModel
{
    protected readonly SignInManager<ApplicationUser> signInManager;
    protected readonly UserManager<ApplicationUser> userManager;
    protected HRMS_WorkContext db;
    protected ApplicationUser user;

    public BaseIModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, HRMS_WorkContext db)
    {
        this.signInManager = signInManager;
        this.userManager = userManager;
        this.db = db;
    }

    public override async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
    {
        user = await userManager.GetUserAsync(context.HttpContext.User);
        await signInManager.RefreshSignInAsync(user);

        ViewData["Title"] = "Manage your account.";
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
        ViewData["ErrorIdentity"] = TempData.Get<ErrorVM>("ErrorIdentity");
    }

    protected async Task<string> SaveFile(IWebHostEnvironment environment, IConfiguration configuration, IFormFile file, string folder, ImageSizeType type = ImageSizeType.ProfilePhoto)
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
                await ResizeImage(file, filePath, (int)type);
            }
            else
            {
                using var fileStream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(fileStream);
            }
            return $"/Uploads/{folder}/{fileName}";
        }
        else
        {
            return null;
        }
    }

    protected async Task ResizeImage(IFormFile file, string filePath, int size)
    {
        var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        stream.Position = 0;

        int width, height;
        var img = Image.FromStream(stream);
        if (img.Width > img.Height)
        {
            width = size;
            height = Convert.ToInt32(img.Height * size / (double)img.Width);
        }
        else
        {
            width = Convert.ToInt32(img.Width * size / (double)img.Height);
            height = size;
        }

        stream.Position = 0;
        var resizer = new MagickImage(stream);

        resizer.Resize(width, height);
        resizer.Orientation = OrientationType.TopLeft;

        resizer.Write(filePath);
    }

    protected async Task SendToHistory(ApplicationUser user, string reason)
    {
        db.AspNetUsersH.Add(new AspNetUsersH
        {
            Id = user.Id,
            AccessFailedCount = user.AccessFailedCount,
            FirstName = user.FirstName,
            LastName = user.LastName,
            EmailConfirmed = user.EmailConfirmed,
            ConcurrencyStamp = user.ConcurrencyStamp,
            Email = user.Email,
            AllowNotification = user.AllowNotification,
            Birthdate = user.Birthdate,
            Language = (int)user.Language,
            LockoutEnabled = user.LockoutEnabled,
            LockoutEnd = user.LockoutEnd,
            Mode = (int)user.Mode,
            NormalizedEmail = user.NormalizedEmail,
            NormalizedUserName = user.NormalizedUserName,
            PasswordHash = user.PasswordHash,
            PersonalNumber = user.PersonalNumber,
            PhoneNumber = user.PhoneNumber,
            PhoneNumberConfirmed = user.PhoneNumberConfirmed,
            ProfileImage = user.ProfileImage,
            SecurityStamp = user.SecurityStamp,
            TwoFactorEnabled = user.TwoFactorEnabled,
            UserName = user.UserName,
            InsertedDate = user.InsertedDate,
            InsertedFrom = user.InsertedFrom,
            Reason = reason
        });

        await db.SaveChangesAsync();
    }
}
