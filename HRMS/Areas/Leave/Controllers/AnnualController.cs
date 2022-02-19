using HRMS.Controllers;
using HRMS.Data.Core;
using HRMS.Data.General;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Areas.Leave.Controllers;

public class AnnualController : BaseController
{
    public AnnualController(HRMSContext db, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        : base(db, signInManager, userManager)
    {
    }

    public IActionResult Index()
    {
        return View();
    }
}
