using HRMS.Data.Core;
using HRMS.Data.General;
using HRMS.Models;
using HRMS.Resources;
using HRMS.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HRMS.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class LoginModel : BaseOModel
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;

    public LoginModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, HRMSContext db)
        : base(db)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
    }

    [BindProperty]
    public InputModel Input { get; set; }
    public IList<AuthenticationScheme> ExternalLogins { get; set; }
    public string ReturnUrl { get; set; }
    public string Language { get; set; }

    public class InputModel
    {
        [Display(Name = "Email", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        public string Email { get; set; }

        [Display(Name = "Password", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public string returnUrl { get; set; }
    }

    public async Task OnGetAsync(string returnUrl = null)
    {
        ViewData["Title"] = "Login";
        Language = Thread.CurrentThread.CurrentCulture.Name;
        Input = new InputModel
        {
            returnUrl = returnUrl
        };

        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        ViewData["Title"] = Resource.Login;
        Language = Thread.CurrentThread.CurrentCulture.Name;
        if (!ModelState.IsValid)
        {
            return new JsonResult(new ErrorVM { Status = ErrorStatus.Error, Description = "" });
        }

        var error = new ErrorVM { Status = ErrorStatus.Success, Description = "" };
        var request = Request.Form;

        var userName = Input.Email;
        if (userName.IndexOf('@') > -1)
        {
            var user = await userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                return new JsonResult(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidLogin, Title = Resource.Warning });
            }

            userName = user.UserName;
        }

        var result = await signInManager.PasswordSignInAsync(userName, Input.Password, Input.RememberMe, lockoutOnFailure: true);
        if (result.Succeeded)
        {
            var userId = await db.AspNetUsers.Where(a => a.Email == Input.Email).Select(a => a.Id).FirstOrDefaultAsync();
            await db.SaveChangesAsync();
            return new JsonResult(error);
        }
        if (result.RequiresTwoFactor)
        {
            return new JsonResult(new ErrorVM { Status = ErrorStatus.Info, Description = Resource.YouMustConfirmEmail, Title = Resource.Info, Icon = "icon fas fa-lock" });
        }
        if (result.IsLockedOut)
        {
            return new JsonResult(new ErrorVM { Status = ErrorStatus.Info, Description = Resource.AccountLocked, Title = Resource.Info, Icon = "icon fas fa-lock" });
        }
        else
        {
            return new JsonResult(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidLogin, Title = Resource.Warning });
        }
    }
}
