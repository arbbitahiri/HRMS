using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HRMS.Data.Core;
using HRMS.Data.General;
using HRMS.Models;
using HRMS.Resources;
using HRMS.Utilities;
using HRMS.Utilities.General;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;

namespace HRMS.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResetPasswordModel : BaseOModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public ResetPasswordModel(UserManager<ApplicationUser> userManager, IConfiguration configuration, HRMSContext db) : base(db)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string Language { get; set; }

        public class InputModel
        {
            [Display(Name = "Email", ResourceType = typeof(Resource))]
            [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
            [EmailAddress]
            [StringLength(32, MinimumLength = 2)]
            public string Email { get; set; }

            [Display(Name = "Password", ResourceType = typeof(Resource))]
            [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
            [StringLength(32, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
            [DataType(DataType.Password)]
            [RegularExpression(@"((?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[~!@#$%^&*()_+?><{}]).{8,32})")]
            public string Password { get; set; }

            [Display(Name = "ConfirmPassword", ResourceType = typeof(Resource))]
            [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            public string Code { get; set; }
        }

        public IActionResult OnGet(string code = null)
        {
            ViewData["Title"] = "Reset password";
            Language = Thread.CurrentThread.CurrentCulture.Name;

            if (code == null)
            {
                return BadRequest("A code must be supplied for password reset.");
            }
            else
            {
                Input = new InputModel
                {
                    Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
                };
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ViewData["Title"] = "Reset password";
            Language = Thread.CurrentThread.CurrentCulture.Name;

            string errors = "";
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                TempData.Set<ErrorVM>("ErrorI", new ErrorVM { Status = ErrorStatus.Error, Description = "Error while reseting password!", Title = "Error!" });
                return RedirectToPage("./Login");
            }

            var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password);
            if (result.Succeeded)
            {
                TempData.Set<ErrorVM>("ErrorI", new ErrorVM { Status = ErrorStatus.Success, Description = "Password has been reset!", Title = "Success!" });
                return RedirectToPage("./Login");
            }

            foreach (var error in result.Errors)
            {
                errors += $"{error.Description}. ";
            }
            TempData.Set<ErrorVM>("ErrorI", new ErrorVM { Status = ErrorStatus.Warning, Description = errors, Title = "Warning!" });
            return Page();
        }
    }
}
