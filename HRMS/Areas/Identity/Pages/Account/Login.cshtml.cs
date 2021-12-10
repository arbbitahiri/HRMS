﻿using HRMS.Data.Core;
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
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public LoginModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, HRMSContext db)
        : base(db)
    {
        _userManager = userManager;
        _signInManager = signInManager;
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

        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        ViewData["Title"] = "Login";
        Language = Thread.CurrentThread.CurrentCulture.Name;
        if (!ModelState.IsValid)
        {
            return new JsonResult(new ErrorVM { Status = ErrorStatus.Error, Description = "" });
        }

        var error = new ErrorVM { Status = ErrorStatus.Success, Description = "" };
        var request = Request.Form;

        var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: true);
        if (result.Succeeded)
        {
            var userId = await db.AspNetUsers.Where(a => a.Email == Input.Email).Select(a => a.Id).FirstOrDefaultAsync();
            await db.SaveChangesAsync();
            return new JsonResult(error);
        }
        if (result.RequiresTwoFactor)
        {
            return new JsonResult(new ErrorVM { Status = ErrorStatus.Info, Description = "You must confirm account!", Title = "Info", Icon = "icon fas fa-lock" });
        }
        if (result.IsLockedOut)
        {
            return new JsonResult(new ErrorVM { Status = ErrorStatus.Info, Description = "Account is locked!", Title = "Info", Icon = "icon fas fa-lock" });
        }
        else
        {
            return new JsonResult(new ErrorVM { Status = ErrorStatus.Warning, Description = "Invalid login attempt", Title = "Warning" });
        }
    }
}
