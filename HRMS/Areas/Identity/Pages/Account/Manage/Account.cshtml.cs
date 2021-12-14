using HRMS.Data.Core;
using HRMS.Data.General;
using HRMS.Models;
using HRMS.Resources;
using HRMS.Utilities;
using HRMS.Utilities.General;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HRMS.Areas.Identity.Pages.Account.Manage;

public class AccountModel : BaseIModel
{
    public AccountModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, HRMSContext db)
        : base(signInManager, userManager, db)
    {
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
        public string Username { get; set; }

        public string Email { get; set; }

        public LanguageEnum Language { get; set; }

        public bool TemplateMode { get; set; }

        public bool AllowNotification { get; set; }
    }

    public void OnGet()
    {
        Input = new InputModel
        {
            Email = user.Email,
            Language = user.Language,
            AllowNotification = user.AllowNotification,
            TemplateMode = user.Mode == TemplateMode.Dark,
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
                TemplateMode = user.Mode == TemplateMode.Dark,
                Username = user.UserName
            };
            return Page();
        }

        // TODO: send to history, before changes

        user.Language = Input.Language;
        user.AllowNotification = Input.AllowNotification;
        user.Mode = Input.TemplateMode ? TemplateMode.Dark : TemplateMode.Light;
        await userManager.UpdateAsync(user);
        await signInManager.RefreshSignInAsync(user);
        TempData.Set("Error", new ErrorVM { Status = ErrorStatus.Success, Title = Resource.Success, Description = Resource.UpdatedProfile });
        return RedirectToPage();
    }
}
