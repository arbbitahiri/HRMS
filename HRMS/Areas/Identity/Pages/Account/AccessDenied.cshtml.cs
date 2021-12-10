using HRMS.Areas.Identity.Pages.Account.Manage;
using HRMS.Data.Core;
using HRMS.Data.General;
using Microsoft.AspNetCore.Identity;

namespace HRMS.Areas.Identity.Pages.Account;
public class AccessDeniedModel : BaseIModel
{
    public AccessDeniedModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, HRMSContext db)
        : base(signInManager, userManager, db)
    {
    }

    public void OnGet()
    {

    }
}


