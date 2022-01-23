using HRMS.Data.Core;
using HRMS.Data.General;
using HRMS.Models.Evaluation;
using HRMS.Utilities;
using HRMS.Utilities.General;
using HRMS.Utilities.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

    [Description("Arb Tahiri", "Form to display evaluation types.")]
    public IActionResult Index() => View();

    #region List

    [Description("Arb Tahiri", "Form to search for evaluation types.")]
    public IActionResult _Search() => View();

    //[HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "70:r")]
    //[Description("Arb Tahiri", "List of the search of evaluation types.")]
    //public async Task<IActionResult> Search(Search search)
    //{

    //}

    #endregion
}
