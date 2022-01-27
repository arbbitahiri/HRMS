using HRMS.Data.Core;
using HRMS.Data.General;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Controllers;

public class EvaluationStudentsController : BaseController
{
    public EvaluationStudentsController(HRMS_WorkContext db, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        : base(db, signInManager, userManager)
    {
    }

    public IActionResult Index()
    {
        return View();
    }
}
