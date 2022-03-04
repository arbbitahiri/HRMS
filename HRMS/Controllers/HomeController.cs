﻿using HRMS.Data.Core;
using HRMS.Data.General;
using HRMS.Models;
using HRMS.Models.Home.SideProfile;
using HRMS.Resources;
using HRMS.Utilities;
using HRMS.Utilities.General;
using HRMS.Utilities.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.Controllers;
[Authorize]
public class HomeController : BaseController
{
    public HomeController(HRMSContext db,
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager)
        : base(db, signInManager, userManager)
    {
    }

    [Authorize(Policy = "1h:m"), Description("Arb Tahiri", "Entry home.")]
    public IActionResult Index()
    {
        //ViewData["Title"] = Resource.HomePage;

        //var getRole = User.Claims.FirstOrDefault(a => a.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");

        //return getRole.Value switch
        //{
        //    "Administrator" => RedirectToAction(nameof(Administrator)),
        //    "Lecturer" => RedirectToAction(nameof(Lecturer)),
        //    "Manager" => RedirectToAction(nameof(Manager)),
        //    _ => View()
        //};
        return View();
    }

    #region Dashboards

    [HttpGet, Authorize("1ha:r"), Description("Arb Tahiri", "Home page for administrator.")]
    public async Task<IActionResult> Administrator()
    {
        ViewData["Title"] = Resource.HomePage;
        return View();
    }

    [HttpGet, Authorize("1hl:r"), Description("Arb Tahiri", "Home page for lecturer.")]
    public async Task<IActionResult> Lecturer()
    {
        ViewData["Title"] = Resource.HomePage;
        return View();
    }

    [HttpGet, Authorize("1hm:r"), Description("Arb Tahiri", "Home page for manager.")]
    public async Task<IActionResult> Manager()
    {
        ViewData["Title"] = Resource.HomePage;
        return View();
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
}
