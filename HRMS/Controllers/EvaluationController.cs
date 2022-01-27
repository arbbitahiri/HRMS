using HRMS.Data.Core;
using HRMS.Data.General;
using HRMS.Models;
using HRMS.Models.Evaluation;
using HRMS.Resources;
using HRMS.Utilities;
using HRMS.Utilities.General;
using HRMS.Utilities.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.Controllers;

[Authorize]
public class EvaluationController : BaseController
{
    public EvaluationController(HRMS_WorkContext db, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        : base(db, signInManager, userManager)
    {
    }

    [Authorize(Policy = "70:r"), Description("Arb Tahiri", "Form to display list of evaluations.")]
    public IActionResult Index() => View();

    [Authorize(Policy = "70:r"), Description("Arb Tahiri", "Form to search for evaluation types.")]
    public IActionResult _Search() => View();
}
