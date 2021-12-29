using HRMS.Data.Core;
using HRMS.Data.General;
using HRMS.Models.Staff;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Threading.Tasks;

namespace HRMS.Controllers;
public class StaffController : BaseController
{
    public StaffController(HRMSContext db, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        : base(db, signInManager, userManager)
    {
    }

    [HttpGet, Description("Entry home. Search for staff.")]
    public IActionResult Index() => View();

    //[HttpPost, Description("List of staff")]
    //public async Task<IActionResult> Search(Search search)
    //{
    //    var list = await db.Staff

    //}
}
