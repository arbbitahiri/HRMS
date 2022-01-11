using HRMS.Data.Core;
using HRMS.Data.General;
using HRMS.Models;
using HRMS.Resources;
using HRMS.Utilities;
using HRMS.Utilities.General;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.Areas.Identity.Pages.Account.Manage;
public class ChangePasswordModel : BaseIModel
{
    public ChangePasswordModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, HRMS_WorkContext db)
        : base(signInManager, userManager, db)
    {
    }

    [BindProperty]
    public InputModel Input { get; set; }

    [TempData]
    public string StatusMessage { get; set; }

    public class InputModel
    {
        [DataType(DataType.Password)]
        [Display(Name = "CurrentPassword", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        public string OldPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "NewPassword", ResourceType = typeof(Resource))]
        [StringLength(100, ErrorMessage = "CharacterLength", MinimumLength = 6)]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "ConfirmNewPassword", ResourceType = typeof(Resource))]
        [Compare("NewPassword", ErrorMessageResourceName = "PasswordNotMatch", ErrorMessageResourceType = typeof(Resource))]
        public string ConfirmPassword { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            TempData.Set("Error", new ErrorVM { Status = ErrorStatus.Error, Title = Resource.Error, Description = Resource.UnableToLoadUser });
            return Page();
        }

        var hasPassword = await userManager.HasPasswordAsync(user);
        if (!hasPassword)
        {
            return RedirectToPage("./SetPassword");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            TempData.Set("Error", new ErrorVM { Status = ErrorStatus.Error, Title = Resource.Error, Description = Resource.UnableToLoadUser });
            return Page();
        }

        var changePasswordResult = await userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
        if (!changePasswordResult.Succeeded)
        {
            TempData.Set("Error", new ErrorVM { Status = ErrorStatus.Warning, RawContent = true, Description = "<ul>" + string.Join("", changePasswordResult.Errors.Select(a => "<li>" + a.Description + "</li>").ToArray()) + "</ul>" });
            return Page();
        }

        await userManager.UpdateAsync(user);
        await signInManager.RefreshSignInAsync(user);

        TempData.Set("Error", new ErrorVM { Status = ErrorStatus.Success, Title = Resource.Success, Description = Resource.UpdatedPassword });
        Response.Redirect("/Home/Index");

        return RedirectToPage();
    }
}
