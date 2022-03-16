using HRMS.Data.Core;
using HRMS.Data.General;
using HRMS.Models;
using HRMS.Models.AppSettings;
using HRMS.Models.Configuration.Email;
using HRMS.Models.Configuration.Subject;
using HRMS.Repository;
using HRMS.Resources;
using HRMS.Utilities;
using HRMS.Utilities.General;
using HRMS.Utilities.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.Controllers;
[Authorize]
public class ConfigurationController : BaseController
{
    private readonly RoleManager<ApplicationRole> roleManager;
    private readonly IFunctionRepository func;

    public ConfigurationController(HRMSContext db,
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IFunctionRepository func)
        : base(db, signInManager, userManager)
    {
        this.roleManager = roleManager;
        this.func = func;
    }

    [Authorize(Policy = "1c:m"), Description("Korab Mustafa", "Entry home.")]
    public IActionResult Index() => View();

    #region Application settings

    [HttpGet, Authorize(Policy = "15:m")]
    [Description("Korab Mustafa", "Form to display list of application settings.")]
    public IActionResult AppSettings()
    {
        ViewData["Title"] = "Parametrat e aplikacionit";

        var appSettings = new List<ApplicationSettings>();

        string json = string.Empty;
        using (var streamReader = new StreamReader("appsettings.json"))
        {
            json = streamReader.ReadToEnd();
        }

        dynamic data = JsonConvert.DeserializeObject(json);

        foreach (var item in data.ConnectionStrings)
        {
            appSettings.Add(new ApplicationSettings { Key = item.Name, Region = "ConnectionString", Value = item.Value });
        }

        foreach (var item in data.AppSettings)
        {
            appSettings.Add(new ApplicationSettings { Key = item.Name, Region = "AppSettings", Value = item.Value });
        }


        foreach (var item in data.EmailConfiguration)
        {
            appSettings.Add(new ApplicationSettings { Key = item.Name, Region = "EmailConfiguration", Value = item.Value });
        }

        return View(appSettings);
    }

    [HttpPost, Authorize(Policy = "15:r")]
    [Description("Korab Mustafa", "Form to edit application settings.")]
    public async Task<IActionResult> _EditAppSetings(ApplicationSettings edit)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Title = Resource.Warning, Description = Resource.InvalidData });
        }

        string json = string.Empty;
        using (var streamReader = new StreamReader("appsettings.json"))
        {
            json = streamReader.ReadToEnd();
        }
        dynamic data = JsonConvert.DeserializeObject(json);

        db.AppSettings.Add(new AppSettings
        {
            OldVersion = data[edit.Region][edit.Key],
            UpdatedVersion = edit.Value
        });
        await db.SaveChangesAsync();
        data[edit.Region][edit.Key] = edit.Value;

        using (var streamWriter = new StreamWriter("appsettings.json", false))
        {
            await streamWriter.WriteAsync(JsonConvert.SerializeObject(data));
        }

        return Json(new ErrorVM { Status = ErrorStatus.Success, Title = Resource.Success, Description = Resource.DataRegisteredSuccessfully });
    }

    #endregion

    #region Subjects

    #region => List

    [Authorize(Policy = "33:m"), Description("Korab Mustafa", "Entry form, list of subjects.")]
    public async Task<IActionResult> SubjectIndex()
    {
        var subjects = await db.Subject
            .Select(a => new SubjectList
            {
                SubjectIde = CryptoSecurity.Encrypt(a.SubjectId),
                Code = a.Code,
                NameSq = a.NameSq,
                NameEn = a.NameEn,
                Active = a.Active
            }).ToListAsync();
        return View(subjects);
    }

    #endregion

    #region => Create

    [Authorize(Policy = "33:c"), Description("Korab Mustafa", "Form to add a subject.")]
    public IActionResult _CreateSubject() => PartialView();

    [HttpPost, Authorize(Policy = "33:c"), ValidateAntiForgeryToken]
    [Description("Korab Mustafa", "Action to add a subject.")]
    public async Task<IActionResult> CreateSubject(CreateSubject create)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Title = Resource.Warning, Description = Resource.InvalidData });
        }

        if (await db.Subject.AnyAsync(a => a.Active && a.Code == create.Code))
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Title = Resource.Warning, Description = Resource.SubjectExistsWithCode });
        }

        db.Subject.Add(new Subject
        {
            Code = create.Code,
            NameSq = create.NameSq,
            NameEn = create.NameEn,
            Active = true,
            InsertedDate = DateTime.Now,
            InsertedFrom = user.Id
        });
        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Title = Resource.Success, Description = Resource.DataRegisteredSuccessfully });
    }

    #endregion

    #region => Edit

    [HttpGet, Authorize(Policy = "33:e"), Description("Korab Mustafa", "Form to edit a subject.")]
    public async Task<IActionResult> _EditSubject(string ide)
    {
        var subject = await db.Subject
            .Where(a => a.SubjectId == CryptoSecurity.Decrypt<int>(ide))
            .Select(a => new CreateSubject
            {
                SubjectIde = ide,
                Code = a.Code,
                NameSq = a.NameSq,
                NameEn = a.NameEn,
                Active = a.Active
            }).FirstOrDefaultAsync();
        return PartialView(subject);
    }

    [HttpPost, Authorize(Policy = "33:e"), ValidateAntiForgeryToken]
    [Description("Korab Mustafa", "Action to edit a subject.")]
    public async Task<IActionResult> EditSubject(CreateSubject edit)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Title = Resource.Warning, Description = Resource.InvalidData });
        }

        if (await db.Subject.AnyAsync(a => a.Active && a.Code == edit.Code))
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Title = Resource.Warning, Description = Resource.SubjectExistsWithCode });
        }

        var subject = await db.Subject.FirstOrDefaultAsync(a => a.SubjectId == CryptoSecurity.Decrypt<int>(edit.SubjectIde));
        subject.Code = edit.Code;
        subject.NameSq = edit.NameSq;
        subject.NameEn = edit.NameEn;
        subject.Active = edit.Active;
        subject.UpdatedDate = DateTime.Now;
        subject.UpdatedFrom = user.Id;
        subject.UpdatedNo = UpdateNo(subject.UpdatedNo);

        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Title = Resource.Success, Description = Resource.DataUpdatedSuccessfully });
    }

    #endregion

    #region => Delete

    [HttpPost, Authorize(Policy = "33:d"), ValidateAntiForgeryToken]
    [Description("Korab Mustafa", "Action to delete a subject.")]
    public async Task<IActionResult> DeleteSubject(string ide)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Title = Resource.Warning, Description = Resource.InvalidData });
        }

        var subject = await db.Subject.FirstOrDefaultAsync(a => a.SubjectId == CryptoSecurity.Decrypt<int>(ide));
        subject.Active = false;
        subject.UpdatedDate = DateTime.Now;
        subject.UpdatedFrom = user.Id;
        subject.UpdatedNo = UpdateNo(subject.UpdatedNo);

        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Title = Resource.Success, Description = Resource.DataDeletedSuccessfully });
    }

    #endregion

    #endregion

    #region Email

    [HttpGet, Authorize(Policy = "1e:m"), Description("Arb Tahiri", "Form to display email.")]
    public async Task<IActionResult> Email()
    {
        ViewData["Title"] = Resource.EmailConfiguration;
        var streamReader = new StreamReader("appsettings.json");
        string json = await streamReader.ReadToEndAsync();
        streamReader.Close();
        dynamic data = JsonConvert.DeserializeObject(json);
        var email = new EmailVM
        {
            Email = data["EmailConfiguration"]["Email"],
            Password = data["EmailConfiguration"]["Password"],
            CC = data["EmailConfiguration"]["CC"],
            Host = data["EmailConfiguration"]["Host"],
            Port = data["EmailConfiguration"]["Port"],
            SSLEnable = data["EmailConfiguration"]["EnableSsl"]
        };
        return View(email);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Method to change email on appsettings.")]
    public async Task<IActionResult> Email(EmailVM em)
    {
        if (!ModelState.IsValid)
        {
            TempData.Set("Error", new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
            return RedirectToAction(nameof(Email));
        }

        var appSettings = new AppSettings
        {
            IndertedDate = DateTime.Now,
            InsertedFrom = user.Id
        };

        var streamReader = new StreamReader("appsettings.json");
        string json = await streamReader.ReadToEndAsync();
        dynamic data = JsonConvert.DeserializeObject(json);
        appSettings.OldVersion = json;
        data["EmailConfiguration"]["Email"] = em.Email;
        data["EmailConfiguration"]["Password"] = em.Password;
        data["EmailConfiguration"]["CC"] = em.CC;
        data["EmailConfiguration"]["Host"] = em.Host;
        data["EmailConfiguration"]["Port"] = em.Port;
        data["EmailConfiguration"]["EnableSsl"] = em.SSLEnable;

        var streamWriter = new StreamWriter("appsettings.json", false);
        await streamWriter.WriteAsync(JsonConvert.SerializeObject(data));
        await streamWriter.FlushAsync();
        streamWriter.Close();

        appSettings.UpdatedVersion = JsonConvert.SerializeObject(data);
        db.AppSettings.Add(appSettings);
        await db.SaveChangesAsync();

        TempData.Set("Error", new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataRegisteredSuccessfully });
        return RedirectToAction(nameof(Email));
    }

    #endregion

    #region Password

    #endregion
}
