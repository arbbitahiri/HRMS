using HRMS.Data.Core;
using HRMS.Data.General;
using HRMS.Models;
using HRMS.Models.AppSettings;
using HRMS.Models.Configuration.Subject;
using HRMS.Repository;
using HRMS.Resources;
using HRMS.Utilities;
using HRMS.Utilities.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.Controllers;
[Authorize]
public class ConfigurationController : BaseController
{
    private readonly RoleManager<ApplicationRole> roleManager;
    private readonly IFunctionRepository func;

    public ConfigurationController(HRMS_WorkContext db,
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IFunctionRepository func)
        : base(db, signInManager, userManager)
    {
        this.roleManager = roleManager;
        this.func = func;
    }

    [Description("Entry home.")]
    public IActionResult Index() => View();

    #region Application settings

    [HttpGet, Authorize(Policy = "15:r")]
    [Description("Form to display list of application settings.")]
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
    [Description("Form to edit application settings.")]
    public async Task<IActionResult> _EditAppSetings(ApplicationSettings edit)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.WARNING, Title = Resource.Warning, Description = Resource.InvalidData });
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

        return Json(new ErrorVM { Status = ErrorStatus.SUCCESS, Title = Resource.Success, Description = Resource.DataRegisteredSuccessfully });
    }

    #endregion

    #region Subjects

    #region => List

    [Authorize(Policy = "33:r"), Description("Entry form, list of subjects.")]
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

    [Authorize(Policy = "33:c"), Description("Form to add a subject.")]
    public IActionResult _CreateSubject() => PartialView();

    [HttpPost, Authorize(Policy = "33:c"), ValidateAntiForgeryToken]
    [Description("Action to add a subject.")]
    public async Task<IActionResult> CreateSubject(CreateSubject create)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.WARNING, Title = Resource.Warning, Description = Resource.InvalidData });
        }

        if (await db.Subject.AnyAsync(a => a.Active && a.Code == create.Code))
        {
            return Json(new ErrorVM { Status = ErrorStatus.WARNING, Title = Resource.Warning, Description = Resource.SubjectExistsWithCode });
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
        return Json(new ErrorVM { Status = ErrorStatus.SUCCESS, Title = Resource.Success, Description = Resource.DataRegisteredSuccessfully });
    }

    #endregion

    #region => Edit

    [HttpGet, Authorize(Policy = "33:e"), Description("Form to edit a subject.")]
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
    [Description("Action to edit a subject.")]
    public async Task<IActionResult> EditSubject(CreateSubject edit)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.WARNING, Title = Resource.Warning, Description = Resource.InvalidData });
        }

        if (await db.Subject.AnyAsync(a => a.Active && a.Code == edit.Code))
        {
            return Json(new ErrorVM { Status = ErrorStatus.WARNING, Title = Resource.Warning, Description = Resource.SubjectExistsWithCode });
        }

        var subject = await db.Subject.FirstOrDefaultAsync(a => a.SubjectId == CryptoSecurity.Decrypt<int>(edit.SubjectIde));
        subject.Code = edit.Code;
        subject.NameSq = edit.NameSq;
        subject.NameEn = edit.NameEn;
        subject.Active = edit.Active;
        subject.UpdatedDate = DateTime.Now;
        subject.UpdatedFrom = user.Id;
        subject.UpdatedNo += 1;

        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.SUCCESS, Title = Resource.Success, Description = Resource.DataUpdatedSuccessfully });
    }

    #endregion

    #region => Delete

    [HttpPost, Authorize(Policy = "33:d"), ValidateAntiForgeryToken]
    [Description("Action to delete a subject.")]
    public async Task<IActionResult> DeleteSubject(string ide)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.WARNING, Title = Resource.Warning, Description = Resource.InvalidData });
        }

        var subject = await db.Subject.FirstOrDefaultAsync(a => a.SubjectId == CryptoSecurity.Decrypt<int>(ide));
        subject.Active = false;
        subject.UpdatedDate = DateTime.Now;
        subject.UpdatedFrom = user.Id;
        subject.UpdatedNo += 1;

        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.SUCCESS, Title = Resource.Success, Description = Resource.DataDeletedSuccessfully });
    }

    #endregion

    #endregion

    #region Email

    #endregion

    #region Password

    #endregion
}
