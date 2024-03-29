﻿using HRMS.Data;
using HRMS.Data.Core;
using HRMS.Data.General;
using HRMS.Models;
using HRMS.Models.Administration;
using HRMS.Models.Administration.Role;
using HRMS.Resources;
using HRMS.Utilities;
using HRMS.Utilities.General;
using HRMS.Utilities.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.Controllers;
[Authorize]
public class AdministrationController : BaseController
{
    private readonly IWebHostEnvironment environment;
    private readonly IConfiguration configuration;
    private readonly ApplicationDbContext appDb;
    private readonly RoleManager<ApplicationRole> roleManager;

    public AdministrationController(IWebHostEnvironment environment, IConfiguration configuration,
        ApplicationDbContext appDb, RoleManager<ApplicationRole> roleManager,
        HRMSContext db, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        : base(db, signInManager, userManager)
    {
        this.environment = environment;
        this.configuration = configuration;
        this.appDb = appDb;
        this.roleManager = roleManager;
    }

    #region Users

    #region => List

    [HttpGet, Authorize(Policy = "41:m"), Description("Arb Tahiri", "Entry home.")]
    public IActionResult Index() => View();

    [HttpPost, Authorize(Policy = "41:r"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Search for users.")]
    public async Task<IActionResult> Search(Search search)
    {
        var users = await db.AspNetUsers
            .Where(a => /*((search.Roles ?? new List<string>()).Any() ? search.Roles.Contains(a.R.Select(a => a.Id).FirstOrDefault()) : true)*/
                /*&&*/ (string.IsNullOrEmpty(search.PersonalNumber) || search.PersonalNumber.Contains(a.PersonalNumber))
                && (string.IsNullOrEmpty(search.Firstname) || search.Firstname.Contains(a.FirstName))
                && (string.IsNullOrEmpty(search.Lastname) || search.Lastname.Contains(a.LastName))
                && (string.IsNullOrEmpty(search.Email) || search.Email.Contains(a.Email)))
            .AsSplitQuery()
            .Select(a => new UserVM
            {
                UserId = CryptoSecurity.Encrypt(a.Id),
                ProfileImage = a.ProfileImage,
                PersonalNumber = a.PersonalNumber,
                Firstname = a.FirstName,
                Lastname = a.LastName,
                Name = $"{a.FirstName} {a.LastName}",
                Email = a.Email,
                PhoneNumber = a.PhoneNumber,
                //Roles = string.Join(", ", a.Role.Select(a => user.Language == LanguageEnum.Albanian ? a.NameSq : a.NameEn).ToArray()),
                LockoutEnd = a.LockoutEnd
            }).ToListAsync();
        return Json(users);
    }

    #endregion

    #region => Create

    [HttpGet, Authorize(Policy = "41:c"), Description("Arb Tahiri", "Form to create a user.")]
    public IActionResult Create() => View();

    [HttpPost, Authorize(Policy = "41:c"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Action to create a user.")]
    public async Task<IActionResult> Create(Create create)
    {
        if (!ModelState.IsValid)
        {
            TempData.Set("Error", new ErrorVM { Status = ErrorStatus.Warning, Title = Resource.Warning, Description = Resource.InvalidData, Dismissible = true });
            return View(create);
        }

        if (await appDb.Users.AnyAsync(a => a.Email == create.Email || a.UserName == create.Username))
        {
            TempData.Set("Error", new ErrorVM { Status = ErrorStatus.Warning, Title = Resource.Warning, Description = Resource.UserHasAccount, Dismissible = true });
            return View(create);
        }

        string filePath = create.ProfileImage != null ? await SaveImage(environment, create.ProfileImage, "Users") : null;

        var firstUser = new ApplicationUser
        {
            PersonalNumber = create.PersonalNumber,
            FirstName = create.Firstname,
            LastName = create.Lastname,
            Birthdate = DateTime.ParseExact(create.Birthdate, "dd/MM/yyyy", null),
            Email = create.Email,
            EmailConfirmed = true,
            PhoneNumber = create.PhoneNumber,
            UserName = create.Email,
            ProfileImage = filePath,
            Language = create.Language,
            Mode = TemplateMode.Light,
            LockoutEnd = DateTime.Now.AddYears(99),
            LockoutEnabled = true,
            InsertedDate = DateTime.Now,
            InsertedFrom = user.Id
        };

        string errors = string.Empty;

        var result = await userManager.CreateAsync(firstUser, create.Password);
        if (!result.Succeeded)
        {
            foreach (var identityError in result.Errors)
            {
                errors += $"· {identityError.Description} ";
            }
            TempData.Set("Error", new ErrorVM { Status = ErrorStatus.Warning, Title = Resource.Warning, Description = errors });
            return View(create);
        }

        if (create.Roles != null)
        {
            result = await userManager.AddToRolesAsync(firstUser, db.AspNetRoles.Where(a => create.Roles.Contains(a.Id)).Select(a => a.Name).ToList());
            if (!result.Succeeded)
            {
                TempData.Set("Error", new ErrorVM { Status = ErrorStatus.Error, Title = Resource.Error, RawContent = true, Description = "<ul>" + string.Join("", result.Errors.Select(a => "<li>" + a.Description + "</li>").ToArray()) + $"<li>{Resource.RolesAddThroughList}</li>" + "</ul>" });
                return RedirectToAction(nameof(Index));
            }
        }

        TempData.Set("Error", new ErrorVM { Status = ErrorStatus.Success, Title = Resource.Success, Description = Resource.AccountCreatedSuccessfully, Dismissible = true });
        return RedirectToAction(nameof(Index));
    }

    #endregion

    #region => Edit

    [HttpGet, Authorize(Policy = "41:e"), Description("Arb Tahiri", "Form to edit a user.")]
    public async Task<IActionResult> _Edit(string uIde)
    {
        var user = await userManager.FindByIdAsync(CryptoSecurity.Decrypt<string>(uIde));
        var edit = new Edit
        {
            UserId = CryptoSecurity.Encrypt(user.Id),
            ImagePath = user.ProfileImage,
            PersonalNumber = user.PersonalNumber,
            Username = user.UserName,
            Firstname = user.FirstName,
            Lastname = user.LastName,
            Birthdate = user.Birthdate.ToString("dd/MM/yyyy"),
            PhoneNumber = user.PhoneNumber,
            Email = user.Email
        };
        return View(edit);
    }

    [HttpPost, Authorize(Policy = "41:e"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Action to edit a user.")]
    public async Task<IActionResult> Edit(Edit edit)
    {
        if (!ModelState.IsValid)
        {
            TempData.Set("Error", new ErrorVM { Status = ErrorStatus.Warning, Title = Resource.Warning, Description = Resource.InvalidData, Dismissible = true });
        }

        if (await appDb.Users.AnyAsync(a => a.Email == edit.Email || a.UserName == edit.Username))
        {
            TempData.Set("Error", new ErrorVM { Status = ErrorStatus.Warning, Title = Resource.Warning, Description = Resource.UserHasAccount, Dismissible = true });
        }

        var user = await userManager.FindByIdAsync(CryptoSecurity.Decrypt<string>(edit.UserId));
        user.PhoneNumber = edit.PhoneNumber;
        user.ProfileImage = edit.ProfileImage != null ? await SaveImage(environment, edit.ProfileImage, "Users") : null;
        user.Birthdate = DateTime.ParseExact(edit.Birthdate, "dd/MM/yyyy", null);
        user.PersonalNumber = edit.PersonalNumber;
        user.FirstName = edit.Firstname;
        user.LastName = edit.Lastname;

        if (user.UserName != edit.Username)
        {
            var result = await userManager.SetUserNameAsync(user, edit.Username);
            if (!result.Succeeded)
            {
                TempData.Set("Error", new ErrorVM { Status = ErrorStatus.Error, Title = Resource.Error, RawContent = true, Description = "<ul>" + string.Join("", result.Errors.Select(a => "<li>" + a.Description + "</li>").ToArray()) + "</ul>" });
            }
        }

        if (user.Email != edit.Email)
        {
            var result = await userManager.SetEmailAsync(user, edit.Email);
            if (!result.Succeeded)
            {
                TempData.Set("Error", new ErrorVM { Status = ErrorStatus.Error, Title = Resource.Error, RawContent = true, Description = "<ul>" + string.Join("", result.Errors.Select(a => "<li>" + a.Description + "</li>").ToArray()) + "</ul>" });
            }
        }

        var update = await userManager.UpdateAsync(user);
        if (!update.Succeeded)
        {
            TempData.Set("Error", new ErrorVM { Status = ErrorStatus.Error, Title = Resource.Error, RawContent = true, Description = "<ul>" + string.Join("", update.Errors.Select(a => "<li>" + a.Description + "</li>").ToArray()) + "</ul>" });
        }

        TempData.Set("Error", new ErrorVM { Status = ErrorStatus.Success, Title = Resource.Success, Description = Resource.DataUpdatedSuccessfully, Dismissible = true });
        return RedirectToAction(nameof(Index));
    }

    #endregion

    #region => Delete

    [HttpPost, Authorize(Policy = "41:d"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Action to delete a user.")]
    public async Task<IActionResult> Delete(string uId)
    {
        await userManager.SetLockoutEndDateAsync(await userManager.FindByIdAsync(CryptoSecurity.Decrypt<string>(uId)), DateTime.Now.AddYears(99));
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataDeletedSuccessfully });
    }

    #endregion

    #endregion

    #region Check for users

    [Description("Arb Tahiri", "Check if email already exists.")]
    public async Task<IActionResult> CheckEmail(string Email, string UserId)
    {
        if (await db.AspNetUsers.AnyAsync(a => a.Email == Email && (string.IsNullOrEmpty(UserId) || a.Id != CryptoSecurity.Decrypt<string>(UserId))))
        {
            return Json(Resource.EmailExists);
        }
        return Json(true);
    }

    [HttpPost, Description("Arb Tahiri", "Check if username already exists.")]
    public async Task<IActionResult> CheckUsername(string uIde, string Username)
    {
        if (await db.AspNetUsers.AnyAsync(a => a.Id != CryptoSecurity.Decrypt<string>(uIde) && a.UserName == Username))
        {
            return Json(Resource.UsernameExists);
        }
        return Json(true);
    }

    #endregion

    #region Manage users

    [HttpGet, Authorize(Policy = "41sp:c"), Description("Arb Tahiri", "Form to set password for user.")]
    public async Task<IActionResult> _SetPassword(string uIde)
    {
        var user = await appDb.Users.Where(a => a.Id == CryptoSecurity.Decrypt<string>(uIde))
            .Select(a => new SetPassword
            {
                UserId = uIde,
                Name = $"{a.FirstName} {a.LastName} ({a.Email})"
            }).FirstOrDefaultAsync();
        return PartialView(user);
    }

    [HttpPost, Authorize(Policy = "41sp:c"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Action to set password for user.")]
    public async Task<IActionResult> _SetPassword(SetPassword set)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        var user = await userManager.FindByIdAsync(CryptoSecurity.Decrypt<string>(set.UserId));
        await userManager.RemovePasswordAsync(user);
        var result = await userManager.AddPasswordAsync(user, set.NewPassword);
        if (!result.Succeeded)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = $"{Resource.PasswordNotAdded}: " + string.Join(", ", result.Errors.Select(t => t.Description).ToArray()) });
        }
        await userManager.UpdateAsync(user);
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = $"{Resource.PasswordUpdatedSuccess}" });
    }

    [HttpPost, Authorize(Policy = "41l:c"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Action to lock the account.")]
    public async Task<IActionResult> Lock(string uIde)
    {
        var result = await userManager.SetLockoutEndDateAsync(await userManager.FindByIdAsync(CryptoSecurity.Decrypt<string>(uIde)), DateTime.Now.AddYears(99));
        if (!result.Succeeded)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = $"{Resource.AccountNotLocked}: " + string.Join(", ", result.Errors.Select(t => t.Description).ToArray()).ToLower() });
        }
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.AccountLockedSuccess });
    }

    [HttpPost, Authorize(Policy = "41ul:c"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Action to unlock the account.")]
    public async Task<IActionResult> Unlock(string uIde)
    {
        var result = await userManager.SetLockoutEndDateAsync(await userManager.FindByIdAsync(CryptoSecurity.Decrypt<string>(uIde)), DateTime.Now);
        if (!result.Succeeded)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = $"{Resource.AccountNotUnlocked}: " + string.Join(", ", result.Errors.Select(t => t.Description).ToArray()).ToLower() });
        }
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.AccountUnlockedSuccess });
    }

    [HttpGet, Authorize(Policy = "41r:c"), Description("Arb Tahiri", "Form to add roles to user.")]
    public async Task<IActionResult> _AddRole(string uIde)
    {
        var user = await db.AspNetUsers.Where(a => a.Id == CryptoSecurity.Decrypt<string>(uIde))
            .Select(a => new AddRole
            {
                UserId = uIde,
                Name = $"{a.FirstName} {a.LastName} ({a.Email})"
            }).FirstOrDefaultAsync();
        return PartialView(user);
    }

    [HttpPost, Authorize(Policy = "41r:c"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Action to add roles to user.")]
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

        var userToAdd = await userManager.FindByIdAsync(CryptoSecurity.Decrypt<string>(addRole.UserId));
        var roleToAdd = await db.AspNetRoles.Where(a => addRole.Role.Contains(a.Id)).Select(a => a.NormalizedName).ToListAsync();

        foreach (var role in addRole.Role)
        {
            db.RealRole.Add(new RealRole
            {
                UserId = userToAdd.Id,
                RoleId = role,
                InsertedDate = DateTime.Now,
                InsertedFrom = user.Id
            });
        }

        if (!(await userManager.GetRolesAsync(userToAdd)).Any())
        {
            var result = await userManager.AddToRolesAsync(userToAdd, roleToAdd);
            if (!result.Succeeded)
            {
                return Json(new ErrorVM { Status = ErrorStatus.Error, Description = "<ul>" + string.Join("", result.Errors.Select(t => "<li>" + t.Description + "</li>").ToArray()) + $"<li>{Resource.RolesAddThroughList}!</li>" + "</ul>" });
            }
        }

        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.RoleAddedSuccess });
    }

    #endregion

    #region Roles

    #region => List

    [HttpGet, Authorize(Policy = "42:m")]
    [Description("Arb Tahiri", "Entry form for list of roles.")]
    public async Task<IActionResult> Roles()
    {
        var roles = await roleManager.Roles
            .Select(a => new Roles
            {
                RoleIde = CryptoSecurity.Encrypt(a.Id),
                NormalizedName = a.NormalizedName,
                NameSQ = a.NameSQ,
                NameEN = a.NameEN,
                DescriptionSQ = a.DescriptionSQ,
                DescriptionEN = a.DescriptionEN
            }).ToListAsync();

        return View(roles.OrderBy(a => a.NormalizedName));
    }

    #endregion

    #region => Create

    [Authorize(Policy = "42:c"), Description("Arb Tahiri", "Form to create a role.")]
    public IActionResult _CreateRole() => PartialView();

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "42:c")]
    [Description("Arb Tahiri", "Action to create a role.")]
    public async Task<IActionResult> CreateRole(CreateRole create)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        if (await roleManager.Roles.AnyAsync(a => a.NameSQ == create.NameSQ || a.NameEN == create.NameEN))
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.RoleExists });
        }

        await roleManager.CreateAsync(new ApplicationRole
        {
            Name = create.NameEN,
            NameSQ = create.NameSQ,
            NameEN = create.NameEN,
            DescriptionSQ = create.DescriptionSQ,
            DescriptionEN = create.DescriptionEN
        });

        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataRegisteredSuccessfully });
    }

    #endregion

    #region => Edit

    [Authorize(Policy = "42:c"), Description("Arb Tahiri", "Form to edit a role.")]
    public async Task<IActionResult> _EditRole(string rIde)
    {
        var role = await roleManager.Roles
            .Where(a => a.Id == CryptoSecurity.Decrypt<string>(rIde))
            .Select(a => new CreateRole
            {
                RoleIde = rIde,
                NameSQ = a.NameSQ,
                NameEN = a.NameEN,
                DescriptionSQ = a.DescriptionSQ,
                DescriptionEN = a.DescriptionEN
            }).FirstOrDefaultAsync();
        return PartialView(role);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "42:r")]
    [Description("Arb Tahiri", "Action to edit a role.")]
    public async Task<IActionResult> EditRole(CreateRole edit)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        if (await roleManager.Roles.CountAsync(a => a.NameSQ == edit.NameSQ || a.NameEN == edit.NameEN) > 1)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.RoleExists });
        }

        var role = await roleManager.Roles.FirstOrDefaultAsync(a => a.Id == CryptoSecurity.Decrypt<string>(edit.RoleIde));
        role.NormalizedName = edit.NameEN.ToUpper();
        role.NameSQ = edit.NameSQ;
        role.NameEN = edit.NameEN;
        role.DescriptionSQ = edit.DescriptionSQ;
        role.DescriptionEN = edit.DescriptionEN;
        await roleManager.UpdateAsync(role);

        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataUpdatedSuccessfully });
    }

    #endregion

    #region => Delete

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "42:c")]
    [Description("Arb Tahiri", "Action to delete a role.")]
    public async Task<IActionResult> DeleteRole(string rIde)
    {
        await roleManager.DeleteAsync(await roleManager.Roles.FirstOrDefaultAsync(a => a.Id == CryptoSecurity.Decrypt<string>(rIde)));

        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataDeletedSuccessfully });
    }

    #endregion

    #endregion
}
