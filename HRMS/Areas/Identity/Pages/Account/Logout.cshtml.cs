using HRMS.Data.General;
using HRMS.Models;
using HRMS.Utilities.General;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HRMS.Areas.Identity.Pages.Account
{
    [Authorize]
    public class LogoutModel : BaseOModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;

        public LogoutModel(SignInManager<IdentityUser> signInManager, HRMSContext db)
            : base(db)
        {
            _signInManager = signInManager;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            TempData.Set<ErrorVM>("ErrorI", new ErrorVM { Status = Utilities.ErrorStatus.Success, Description = "You are logged out." });
            return RedirectToPage("Login");
        }
    }
}
