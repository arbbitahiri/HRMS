using HRMS.Data.Core;
using HRMS.Data.General;
using HRMS.Models;
using HRMS.Resources;
using HRMS.Utilities;
using HRMS.Utilities.General;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace HRMS.Areas.Identity.Pages.Account
{
    public class TimeoutModel : BaseOModel
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signManger;
        private IConfiguration _configuration;

        public TimeoutModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signManger, IConfiguration configuration, HRMS_WorkContext db)
            : base(db)
        {
            _userManager = userManager;
            _signManger = signManger;
            _configuration = configuration;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Display(Name = "Email", ResourceType = typeof(Resource))]
            [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
            public string Email { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Password", ResourceType = typeof(Resource))]
            [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
            public string Password { get; set; }
        }

        public string ReturnUrl { get; set; }
        public string Language { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Image { get; set; }

        public ErrorVM Error { get; set; }

        public async Task OnGetAsync(string returnUrl = null)
        {
            Error = TempData.Get<ErrorVM>("Error");
            Language = Thread.CurrentThread.CurrentCulture.Name;

            ApplicationUser user = await _userManager.GetUserAsync(this.HttpContext.User);
            if (user != null)
            {
                Name = $"{user.FirstName} {user.LastName}";
                Email = user.Email;
                Image = user.ProfileImage;
                await _signManger.SignOutAsync();
            }
            Input = new InputModel();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Language = Thread.CurrentThread.CurrentCulture.Name;

            if (!ModelState.IsValid)
            {
                TempData.Set("ErrorI", new ErrorVM { Status = ErrorStatus.Warning, Description = "Invalid data!" });
                return Page();
            }

            var result = await _signManger.PasswordSignInAsync(Input.Email, Input.Password, false, lockoutOnFailure: true);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            if (result.IsNotAllowed)
            {
                TempData.Set("ErrorI", new ErrorVM { Status = ErrorStatus.Warning, Description = "You must confirm your account.", Title = "Warning" });
                return Page();
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToPage("./LoginWith2fa", new { RememberMe = false });
            }
            if (result.IsLockedOut)
            {
                TempData.Set("ErrorI", new ErrorVM { Status = ErrorStatus.Info, Description = "Account is locked!", Title = "Info", Icon = "icon fas fa-lock" });
                return Page();
            }
            else
            {
                TempData.Set("ErrorI", new ErrorVM { Status = ErrorStatus.Warning, Description = "Invalid login attempt", Title = "Warning" });
                return Page();
            }
        }
    }
}
