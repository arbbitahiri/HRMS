﻿using HRMS.Data.Core;
using HRMS.Data.General;
using HRMS.Models;
using HRMS.Models.AppSettings;
using HRMS.Models.Authorization;
using HRMS.Models.Configuration.Subject;
using HRMS.Models.Menu;
using HRMS.Models.SubMenu;
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
using System.Security.Claims;
using System.Threading.Tasks;

namespace HRMS.Controllers;
[Authorize]
public class ConfigurationController : BaseController
{
    private readonly RoleManager<ApplicationRole> roleManager;
    private readonly IFunctionRepo func;

    public ConfigurationController(HRMS_WorkContext db,
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IFunctionRepo func)
        : base(db, signInManager, userManager)
    {
        this.roleManager = roleManager;
        this.func = func;
    }

    [Authorize(Policy = "1c:r"), Description("Entry home.")]
    public IActionResult Index() => View();

    #region Authorization - 11a:r

    [Authorize(Policy = "11a:r"), Description("Authorization configuration.")]
    public IActionResult Authorization() => View();

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "11a:r")]
    [Description("Form to search through authorizations.")]
    public async Task<IActionResult> Search(Search search)
    {
        var roleDetails = (await func.MenuListAccess(search.Role, user.Language))
            .Select(a => new RoleDetails
            {
                MenuIde = CryptoSecurity.Encrypt(a.MenuId),
                SubMenuIde = a.SubMenuId != 0 ? CryptoSecurity.Encrypt(a.SubMenuId) : string.Empty,
                MenuTitle = a.Menu,
                SubMenuTitle = a.SubMenu,
                Icon = a.Icon,
                HasSubMenu = a.HasSubMenu,
                HasAccess = a.HasAccess,
                ClaimPolicy = a.ClaimPolicy
            }).ToList();
        return Json(roleDetails);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "11a:r")]
    [Description("Form to search through authorizations.")]
    public async Task<IActionResult> ChangeAccess(ChangeAccess change)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Title = Resource.Warning, Description = Resource.InvalidData });
        }

        Menu menu = null; SubMenu subMenu = null;
        string claimType = string.Empty, claimValue = string.Empty;
        var getRole = await roleManager.FindByIdAsync(change.Role);

        if (!string.IsNullOrEmpty(change.SubMenuIde))
        {
            subMenu = await db.SubMenu.FindAsync(CryptoSecurity.Decrypt<int>(change.SubMenuIde));
            claimType = subMenu.Claim.Split(":")[0];
            claimValue = subMenu.Claim.Split(":")[1];
        }
        else
        {
            menu = await db.Menu.FindAsync(CryptoSecurity.Decrypt<int>(change.MenuIde));
            claimType = menu.Claim.Split(":")[0];
            claimValue = menu.Claim.Split(":")[1];
        }

        if (change.Access && !await db.AspNetRoleClaims.AnyAsync(a => a.ClaimType == claimType && a.ClaimValue == claimValue && a.RoleId == change.Role))
        {
            await roleManager.AddClaimAsync(getRole, new Claim(claimType, claimValue));
            if (!string.IsNullOrEmpty(change.SubMenuIde))
            {
                subMenu.Roles += getRole.Name + ", ";
            }
            else
            {
                menu.Roles += getRole.Name + ", ";
            }
        }
        else
        {
            await roleManager.RemoveClaimAsync(getRole, new Claim(claimType, claimValue));
            if (!string.IsNullOrEmpty(change.SubMenuIde))
            {
                subMenu.Roles = subMenu.Roles.Replace(getRole.Name, string.Empty);
            }
            else
            {
                menu.Roles = menu.Roles.Replace(getRole.Name, string.Empty);
            }
        }

        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Title = Resource.Success, Description = Resource.AccessChangedSuccessfully });
    }

    #endregion

    [Authorize(Policy = "11m:r"), Description("Form to display tabs for Menu and Submenu.")]
    public IActionResult MenuIndex() => View();

    #region Menu

    [Authorize(Policy = "11m:r"), Description("Menu configuration.")]
    public async Task<IActionResult> Menu()
    {
        var menus = await db.Menu.Select(a => new MenuDetails
        {
            MenuIde = CryptoSecurity.Encrypt(a.MenuId),
            Title = user.Language == LanguageEnum.Albanian ? a.NameSq : a.NameEn,
            Controller = a.Controller,
            Action = a.Action,
            HasSubMenu = a.HasSubMenu,
            Icon = a.Icon
        }).ToListAsync();
        return PartialView(menus);
    }

    #region => Create

    [HttpGet, Authorize(Policy = "11m:r")]
    [Description("Form to create a new menu.")]
    public IActionResult _CreateMenu() => PartialView();

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "11m:r")]
    [Description("Action to create a new menu.")]
    public async Task<IActionResult> CreateMenu(CreateMenu create)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Title = Resource.Warning, Description = Resource.InvalidData });
        }

        db.Menu.Add(new Menu
        {
            NameSq = create.NameSq,
            NameEn = create.NameEn,
            HasSubMenu = create.HasSubMenu,
            Active = true,
            Icon = create.Icon,
            Claim = create.ClaimPolicy,
            ClaimType = create.ClaimPolicy?.Split(":")[0],
            Controller = create.Controller,
            Action = create.Action,
            OrdinalNumber = create.OrdinalNumber,
            OpenFor = create.OpenFor,
            InsertedFrom = user.Id,
            InsertedDate = DateTime.Now
        });
        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Title = Resource.Success, Description = Resource.DataRegisteredSuccessfully });
    }

    #endregion

    #region => Edit

    [HttpGet, Authorize(Policy = "11m:r")]
    [Description("Form to edit a new menu.")]
    public async Task<IActionResult> _EditMenu(string ide)
    {
        var menu = await db.Menu.FindAsync(CryptoSecurity.Decrypt<int>(ide));
        var edit = new EditMenu
        {
            MenuIde = ide,
            NameSq = menu.NameSq,
            NameEn = menu.NameEn,
            HasSubMenu = menu.HasSubMenu,
            Active = menu.Active,
            Icon = menu.Icon,
            ClaimPolicy = menu.Claim,
            Controller = menu.Controller,
            Action = menu.Action,
            OrdinalNumber = menu.OrdinalNumber,
            OpenFor = menu.OpenFor
        };
        return PartialView(edit);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "11m:r")]
    [Description("Action to edit a new menu.")]
    public async Task<IActionResult> EditMenu(EditMenu edit)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Title = Resource.Warning, Description = Resource.InvalidData });
        }

        var menu = await db.Menu.FindAsync(CryptoSecurity.Decrypt<int>(edit.MenuIde));
        menu.NameSq = edit.NameSq;
        menu.NameEn = edit.NameEn;
        menu.HasSubMenu = edit.HasSubMenu;
        menu.Active = edit.Active;
        menu.Icon = edit.Icon;
        menu.Claim = edit.ClaimPolicy;
        menu.ClaimType = edit.ClaimPolicy?.Split(":")[0];
        menu.Controller = edit.Controller;
        menu.Action = edit.Action;
        menu.OrdinalNumber = edit.OrdinalNumber;
        menu.OpenFor = edit.OpenFor;
        menu.UpdatedFrom = user.Id;
        menu.UpdatedDate = DateTime.Now;
        menu.UpdatedNo++;

        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Title = Resource.Success, Description = Resource.DataUpdatedSuccessfully });
    }

    #endregion

    #region => Delete

    [HttpPost, Authorize(Policy = "11m:r"), ValidateAntiForgeryToken]
    [Description("Form to edit a new menu.")]
    public async Task<IActionResult> DeleteMenu(string ide)
    {
        db.Menu.Remove(await db.Menu.FindAsync(CryptoSecurity.Decrypt<int>(ide)));
        await db.SaveChangesAsync();

        return Json(new ErrorVM { Status = ErrorStatus.Success, Title = Resource.Success, Description = Resource.DataDeletedSuccessfully });
    }

    #endregion

    #endregion

    #region SubMenu

    [Authorize(Policy = "11m:r"), Description("SubMenu configuration.")]
    public async Task<IActionResult> SubMenu()
    {
        var menus = await db.SubMenu.Include(a => a.Menu)
            .Select(a => new SubMenuDetails
            {
                SubMenuIde = CryptoSecurity.Encrypt(a.SubMenuId),
                Title = user.Language == LanguageEnum.Albanian ? a.NameSq : a.NameEn,
                MenuTitle = user.Language == LanguageEnum.Albanian ? a.Menu.NameSq : a.Menu.NameEn,
                Controller = a.Controller,
                Action = a.Action,
                Icon = a.Icon
            }).ToListAsync();
        return PartialView(menus);
    }

    #region => Create

    [HttpGet, Authorize(Policy = "11m:r")]
    [Description("Form to create a new submenu.")]
    public async Task<IActionResult> _CreateSubMenu(string ide)
    {
        var menu = await db.Menu.FindAsync(CryptoSecurity.Decrypt<int>(ide));
        var create = new CreateSubMenu
        {
            MenuIde = ide,
            MenuTitle = user.Language == LanguageEnum.Albanian ? menu.NameSq : menu.NameEn
        };
        return PartialView(create);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "11m:r")]
    [Description("Action to create a new submenu.")]
    public async Task<IActionResult> CreateSubMenu(CreateSubMenu create)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Title = Resource.Warning, Description = Resource.InvalidData });
        }

        db.SubMenu.Add(new SubMenu
        {
            MenuId = CryptoSecurity.Decrypt<int>(create.MenuIde),
            NameSq = create.NameSq,
            NameEn = create.NameEn,
            Active = true,
            Icon = create.Icon,
            Claim = create.ClaimPolicy,
            ClaimType = create.ClaimPolicy?.Split(":")[0],
            Controller = create.Controller,
            Action = create.Action,
            OrdinalNumber = create.OrdinalNumber,
            OpenFor = create.OpenFor,
            InsertedFrom = user.Id,
            InsertedDate = DateTime.Now
        });
        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Title = Resource.Success, Description = Resource.DataRegisteredSuccessfully });
    }

    #endregion

    #region => Edit

    [HttpGet, Authorize(Policy = "11m:r")]
    [Description("Form to edit a new submenu.")]
    public async Task<IActionResult> _EditSubMenu(string ide)
    {
        var submenu = await db.SubMenu.FindAsync(CryptoSecurity.Decrypt<int>(ide));
        var edit = new EditSubMenu
        {
            SubMenuIde = ide,
            NameSq = submenu.NameSq,
            NameEn = submenu.NameEn,
            Active = submenu.Active,
            Icon = submenu.Icon,
            ClaimPolicy = submenu.Claim,
            Controller = submenu.Controller,
            Action = submenu.Action,
            OrdinalNumber = submenu.OrdinalNumber,
            OpenFor = submenu.OpenFor
        };
        return PartialView(edit);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "11m:r")]
    [Description("Action to edit a new submenu.")]
    public async Task<IActionResult> EditSubMenu(EditSubMenu edit)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Title = Resource.Warning, Description = Resource.InvalidData });
        }

        var submenu = await db.SubMenu.FindAsync(CryptoSecurity.Decrypt<int>(edit.SubMenuIde));
        submenu.NameSq = edit.NameSq;
        submenu.NameEn = edit.NameEn;
        submenu.Active = edit.Active;
        submenu.Icon = edit.Icon;
        submenu.Claim = edit.ClaimPolicy;
        submenu.ClaimType = edit.ClaimPolicy?.Split(":")[0];
        submenu.Controller = edit.Controller;
        submenu.Action = edit.Action;
        submenu.OrdinalNumber = edit.OrdinalNumber;
        submenu.OpenFor = edit.OpenFor;
        submenu.UpdatedFrom = user.Id;
        submenu.UpdatedDate = DateTime.Now;
        submenu.UpdatedNo++;

        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Title = Resource.Success, Description = Resource.DataUpdatedSuccessfully });
    }

    #endregion

    #region => Delete

    [HttpPost, Authorize(Policy = "11m:r"), ValidateAntiForgeryToken]
    [Description("Form to edit a new submenu.")]
    public async Task<IActionResult> DeleteSubMenu(string ide)
    {
        db.SubMenu.Remove(await db.SubMenu.FindAsync(CryptoSecurity.Decrypt<int>(ide)));
        await db.SaveChangesAsync();

        return Json(new ErrorVM { Status = ErrorStatus.Success, Title = Resource.Success, Description = Resource.DataDeletedSuccessfully });
    }

    #endregion

    #endregion

    #region Application settings

    [HttpGet, Authorize(Policy = "11as:r")]
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

    [HttpPost, Authorize(Policy = "11as:r")]
    [Description("Form to edit application settings.")]
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

    [Authorize(Policy = "22s:r"), Description("Entry form, list of subjects.")]
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

    [Authorize(Policy = "22s:r"), Description("Form to add a subject.")]
    public IActionResult _CreateSubject() => PartialView();

    [HttpPost, Authorize(Policy = "22s:r"), ValidateAntiForgeryToken]
    [Description("Action to add a subject.")]
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

    [HttpGet, Authorize(Policy = "22s:r"), Description("Form to edit a subject.")]
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

    [HttpPost, Authorize(Policy = "22s:r"), ValidateAntiForgeryToken]
    [Description("Action to edit a subject.")]
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
        subject.UpdatedNo++;

        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Title = Resource.Success, Description = Resource.DataUpdatedSuccessfully });
    }

    #endregion

    #region => Delete

    [HttpPost, Authorize(Policy = "22s:r"), ValidateAntiForgeryToken]
    [Description("Action to delete a subject.")]
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
        subject.UpdatedNo++;

        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Title = Resource.Success, Description = Resource.DataDeletedSuccessfully });
    }

    #endregion

    #endregion
}
