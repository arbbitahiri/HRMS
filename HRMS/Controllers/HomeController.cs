using HRMS.Data.Core;
using HRMS.Data.General;
using HRMS.Models;
using HRMS.Models.Home.SideProfile;
using HRMS.Utilities.General;
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
    public HomeController(HRMS_WorkContext db,
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager)
        : base(db, signInManager, userManager)
    {
    }

    [Description("Arb Tahiri", "Entry home.")]
    public IActionResult Index()
    {
        ViewData["Title"] = "Home";
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [Description("Arb Tahiri", "Error view")]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
