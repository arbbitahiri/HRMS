using HRMS.Data.Core;
using HRMS.Data.General;
using HRMS.Models.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using HRMS.Utilities;
using Microsoft.AspNetCore.Hosting;
using HRMS.Models;
using HRMS.Data;
using System;
using HRMS.Resources;
using HRMS.Utilities.General;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace HRMS.Controllers;
[Authorize]
public class AdministrationController : BaseController
{
    private readonly IWebHostEnvironment environment;
    private readonly IConfiguration configuration;
    private readonly ApplicationDbContext appDb;

    public AdministrationController(IWebHostEnvironment environment, IConfiguration configuration, ApplicationDbContext appDb,
        HRMSContext db, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        : base(db, signInManager, userManager)
    {
        this.environment = environment;
        this.configuration = configuration;
        this.appDb = appDb;
    }

    #region Users => CRUD

    #region |> List

    [HttpGet, Description("Entry home.")]
    public IActionResult Index() => View();

    [HttpPost, ValidateAntiForgeryToken]
    [Description("Search for users.")]
    public async Task<IActionResult> Search(Search search)
    {
        var users = await db.AspNetUsers
            .Include(a => a.AspNetUserRoles).ThenInclude(a => a.Role)
            .Where(a => ((search.Roles ?? new List<string>()).Any() ? search.Roles.Contains(a.AspNetUserRoles.Select(a => a.RoleId).FirstOrDefault()) : true)
                && (string.IsNullOrEmpty(search.PersonalNumber) || search.PersonalNumber.Contains(a.PersonalNumber))
                && (string.IsNullOrEmpty(search.Firstname) || search.Firstname.Contains(a.FirstName))
                && (string.IsNullOrEmpty(search.Lastname) || search.Lastname.Contains(a.LastName))
                && (string.IsNullOrEmpty(search.Email) || search.Email.Contains(a.Email)))
            .AsSplitQuery()
            .Select(a => new UserVM
            {
                UserId = a.Id,
                ProfileImage = a.ProfileImage,
                PersonalNumber = a.PersonalNumber,
                Firstname = a.FirstName,
                Lastname = a.LastName,
                Name = $"{a.FirstName} {a.LastName}",
                Email = a.Email,
                PhoneNumber = a.PhoneNumber,
                Roles = string.Join(", ", a.AspNetUserRoles.Select(a => user.Language == LanguageEnum.Albanian ? a.Role.NameSq : a.Role.NameEn).ToArray()),
                LockoutEnd = a.LockoutEnd
            }).ToListAsync();
        return Json(users);
    }

    #endregion

    #region |=> Create

    [HttpGet, Description("Form to create a user.")]
    public IActionResult Create() => View();

    [HttpPost, ValidateAntiForgeryToken]
    [Description("Action to create a user.")]
    public async Task<IActionResult> Create(Create create)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        if (await appDb.Users.AnyAsync(a => a.Email == create.Email || a.UserName == create.Username))
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.UserHasAccount });
        }

        string filePath = create.ProfileImage != null ? await SaveImage(environment, create.ProfileImage, "Users") : null;

        var newUser = new ApplicationUser
        {
            PersonalNumber = create.PersonalNumber,
            FirstName = create.Firstname,
            LastName = create.Lastname,
            Birthdate = create.Birthdate,
            Email = create.Email,
            EmailConfirmed = true,
            PhoneNumber = create.PhoneNumber,
            UserName = create.Username,
            ProfileImage = filePath,
            Language = create.Language,
            Mode = TemplateMode.Light,
            InsertedDate = DateTime.Now,
            InsertedFrom = user.Id
        };

        string errors = string.Empty;
        var error = new ErrorVM { Status = ErrorStatus.Success, Description = Resource.AccountCreatedSuccessfully };

        string password = FirstTimePassword(configuration, create.Firstname, create.Lastname);
        var result = await userManager.CreateAsync(newUser, password);
        if (!result.Succeeded)
        {
            foreach (var identityError in result.Errors)
            {
                errors += $"{identityError.Description}. ";
            }
            error = new ErrorVM { Status = ErrorStatus.Warning, Description = errors };
        }

        if (create.Roles.Any())
        {
            result = await userManager.AddToRolesAsync(newUser, db.AspNetRoles.Where(a => create.Roles.Contains(a.Id)).Select(a => a.Name).ToList());
            if (!result.Succeeded)
            {
                TempData.Set("Error", new ErrorVM { Status = ErrorStatus.Error, RawContent = true, Description = "<ul>" + string.Join("", result.Errors.Select(a => "<li>" + a.Description + "</li>").ToArray()) + $"<li>{Resource.RolesAddThroughList}</li>" + "</ul>" });
                return RedirectToAction(nameof(Index));
            }
        }

        await db.SaveChangesAsync();
        return Json(error);
    }

    #endregion

    #region |=> Edit

    [HttpGet, Description("Form to edit a user.")]
    public async Task<IActionResult> _Edit(string uId)
    {
        var user = await userManager.FindByIdAsync(uId);
        var edit = new Edit
        {
            UserId = user.Id,
            ImagePath = user.ProfileImage,
            PersonalNumber = user.PersonalNumber,
            Username = user.UserName,
            Firsname = user.FirstName,
            Lastname = user.LastName,
            Birthdate = user.Birthdate,
            PhoneNumber = user.PhoneNumber,
            Email = user.Email,
            Language = user.Language
        };
        return View(edit);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Description("Action to edit a user.")]
    public async Task<IActionResult> _Edit(Edit edit)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        if (await appDb.Users.AnyAsync(a => a.Email == edit.Email || a.UserName == edit.Username))
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.UserHasAccount });
        }

        var user = await userManager.FindByIdAsync(edit.UserId);
        user.PhoneNumber = edit.PhoneNumber;
        user.Language = edit.Language;
        user.ProfileImage = edit.ProfileImage != null ? await SaveImage(environment, edit.ProfileImage, "Users") : null;

        if (user.UserName != edit.Username)
        {
            var result = await userManager.SetUserNameAsync(user, edit.Username);
            if (!result.Succeeded)
            {
                TempData.Set("Error", new ErrorVM { Status = ErrorStatus.Error, RawContent = true, Description = "<ul>" + string.Join("", result.Errors.Select(a => "<li>" + a.Description + "</li>").ToArray()) + "</ul>" });
                return View(edit);
            }
        }

        if (user.Email != edit.Email)
        {
            var result = await userManager.SetEmailAsync(user, edit.Email);
            if (!result.Succeeded)
            {
                TempData.Set("Error", new ErrorVM { Status = ErrorStatus.Error, RawContent = true, Description = "<ul>" + string.Join("", result.Errors.Select(a => "<li>" + a.Description + "</li>").ToArray()) + "</ul>" });
                return View(edit);
            }
        }

        var update = await userManager.UpdateAsync(user);
        if (!update.Succeeded)
        {
            TempData.Set("Error", new ErrorVM { Status = ErrorStatus.Error, RawContent = true, Description = "<ul>" + string.Join("", update.Errors.Select(a => "<li>" + a.Description + "</li>").ToArray()) + "</ul>" });
            return View(edit);
        }

        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataUpdatedSuccessfully });
    }

    #endregion

    #region |=> Delete

    [HttpPost, ValidateAntiForgeryToken]
    [Description("Action to delete a user.")]
    public async Task<IActionResult> Delete(string uId)
    {
        await userManager.SetLockoutEndDateAsync(await userManager.FindByIdAsync(uId), DateTime.Now.AddYears(99));
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataDeletedSuccessfully });
    }

    #endregion

    #endregion

    #region Check for users

    [HttpPost, ValidateAntiForgeryToken]
    [Description("Check if email already exists.")]
    public async Task<IActionResult> CheckEmail(string uId, string Email)
    {
        if (await db.AspNetUsers.AnyAsync(a => a.Id != uId && a.Email == Email))
        {
            return Json(Resource.EmailExists);
        }
        return Json(true);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Description("Check if username already exists.")]
    public async Task<IActionResult> CheckUsername(string uId, string Username)
    {
        if (await db.AspNetUsers.AnyAsync(a => a.Id != uId && a.UserName == Username))
        {
            return Json(Resource.UsernameExists);
        }
        return Json(true);
    }

    #endregion

    #region Manage users

    [HttpGet, Description("Form to set password for user.")]
    public async Task<IActionResult> _SetPassword(string uId)
    {
        var user = await appDb.Users.Where(a => a.Id == uId)
            .Select(a => new SetPassword
            {
                UserId = uId,
                Name = $"{a.FirstName} {a.LastName} ({a.Email})"
            }).FirstOrDefaultAsync();
        return PartialView(user);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Description("Action to set password for user.")]
    public async Task<IActionResult> _SetPassword(SetPassword set)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        var user = await userManager.FindByIdAsync(set.UserId);
        await userManager.RemovePasswordAsync(user);
        var result = await userManager.AddPasswordAsync(user, set.NewPassword);
        if (!result.Succeeded)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = $"{Resource.PasswordNotAdded}: " + string.Join(", ", result.Errors.Select(t => t.Description).ToArray()) });
        }
        await userManager.UpdateAsync(user);
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = $"{Resource.PasswordUpdatedSuccess}" });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Description("Action to lock the account.")]
    public async Task<IActionResult> Lock(string uId)
    {
        await userManager.SetLockoutEndDateAsync(await userManager.FindByIdAsync(uId), DateTime.Now.AddYears(99));
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.AccountLockedSuccess });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Description("Action to unlock the account.")]
    public async Task<IActionResult> Unlock(string uId)
    {
        await userManager.SetLockoutEndDateAsync(await userManager.FindByIdAsync(uId), DateTime.Now);
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.AccountUnlockedSuccess });
    }

    [HttpGet, Description("Form to add roles to user.")]
    public async Task<IActionResult> _AddRole(string uId)
    {
        var user = await db.AspNetUsers.Where(a => a.Id == uId)
            .Select(a => new AddRole
            {
                UserId = uId,
                Name = $"{a.FirstName} {a.LastName} ({a.Email})"
            }).FirstOrDefaultAsync();
        return PartialView(user);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Description("Action to add roles to user.")]
    public async Task<IActionResult> _AddRole(AddRole addRole)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        var currentUser = await userManager.GetUserAsync(HttpContext.User);
        if (!await userManager.CheckPasswordAsync(currentUser, addRole.Password))
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.IncorrectPassword });
        }

        var user = await userManager.Users.FirstOrDefaultAsync(a => a.Id == addRole.UserId);

        if (!await db.AspNetUserRoles.AnyAsync(a => a.UserId == user.Id))
        {
            await userManager.AddToRoleAsync(user, addRole.Role);
        }
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.RoleAddedSuccess });
    }

    #endregion
}

