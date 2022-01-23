using HRMS.Data.General;
using HRMS.Models;
using HRMS.Utilities.General;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmEmailChangeModel : BaseOModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public ConfirmEmailChangeModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, HRMS_WorkContext db)
            : base(db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public ErrorVM Error { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string email, string code)
        {
            if (userId == null || email == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ChangeEmailAsync(user, email, code);
            if (!result.Succeeded)
            {
                Error = new ErrorVM { Status = Utilities.ErrorStatus.Error, Description = "Error while changing email!" };
                return RedirectToPage("Login");
            }

            // In our UI email and user name are one and the same, so when we update the email
            // we need to update the user name.
            var setUserNameResult = await _userManager.SetUserNameAsync(user, email);
            if (!setUserNameResult.Succeeded)
            {
                Error = new ErrorVM { Status = Utilities.ErrorStatus.Error, Description = "Error while changing username!" };
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            Error = new ErrorVM { Status = Utilities.ErrorStatus.Error, Description = "Thank you for confirming your email." };

            TempData.Set<ErrorVM>("Error", Error);
            return RedirectToPage("Login");
        }
    }
}
