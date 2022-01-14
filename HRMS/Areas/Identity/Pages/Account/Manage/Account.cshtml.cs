using HRMS.Data.Core;
using HRMS.Data.General;
using HRMS.Models;
using HRMS.Resources;
using HRMS.Utilities;
using HRMS.Utilities.General;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace HRMS.Areas.Identity.Pages.Account.Manage;
public class AccountModel : BaseIModel
{
    public AccountModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, HRMS_WorkContext db)
        : base(signInManager, userManager, db)
    {
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
        public string Username { get; set; }

        [EmailAddress, Display(Name = "Email", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        public string Email { get; set; }

        public LanguageEnum Language { get; set; }

        public bool AllowNotification { get; set; }
    }

    public void OnGet()
    {
        Input = new InputModel
        {
            Email = user.Email,
            Language = user.Language,
            AllowNotification = user.AllowNotification,
            Username = user.UserName
        };
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Input = new InputModel
            {
                Email = user.Email,
                Language = user.Language,
                AllowNotification = user.AllowNotification,
                Username = user.UserName
            };
            return Page();
        }

        await SendToHistory(user, "Ndryshim i të dhënave të llogarisë.");

        user.Language = Input.Language;
        user.AllowNotification = Input.AllowNotification;
        user.UserName = Input.Username;
        user.Email = Input.Email;

        await userManager.UpdateAsync(user);
        await signInManager.RefreshSignInAsync(user);

        TempData.Set("ErrorIdentity", new ErrorVM { Status = ErrorStatus.SUCCESS, Title = Resource.Success, Description = Resource.UpdatedProfile });
        return RedirectToPage();
    }
}
