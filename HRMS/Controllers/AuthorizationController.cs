using HRMS.Data.Core;
using HRMS.Data.General;
using HRMS.Models;
using HRMS.Models.Authorization;
using HRMS.Models.Configuration.Rules;
using HRMS.Models.Menu;
using HRMS.Models.SubMenu;
using HRMS.Repository;
using HRMS.Resources;
using HRMS.Utilities;
using HRMS.Utilities.General;
using HRMS.Utilities.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HRMS.Controllers;
[Authorize]
public class AuthorizationController : BaseController
{
    private readonly RoleManager<ApplicationRole> roleManager;
    private readonly IFunctionRepository function;

    public AuthorizationController(RoleManager<ApplicationRole> roleManager, IFunctionRepository function,
        HRMS_WorkContext db, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        : base(db, signInManager, userManager)
    {
        this.roleManager = roleManager;
        this.function = function;
    }

    [Authorize("50:r"), Description("Arb Tahiri", "Entry form for authorizations.")]
    public IActionResult Index() => View();

    #region Authorization

    [Description("Arb Tahiri", "Authorization configuration.")]
    public IActionResult Authorization() => View();

    [HttpPost, Authorize(Policy = "51:c"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Form to search through authorizations.")]
    public async Task<IActionResult> Search(Search search)
    {
        var roleDetails = (await function.MenuListAccess(search.Role, user.Language))
            .Select(a => new RoleDetails
            {
                MenuIde = CryptoSecurity.Encrypt(a.MenuId),
                SubMenuIde = a.SubMenuId != 0 ? CryptoSecurity.Encrypt(a.SubMenuId) : string.Empty,
                MenuTitle = a.Menu,
                SubMenuTitle = a.SubMenu,
                Icon = a.Icon,
                HasSubMenu = a.HasSubMenu,
                HasAccess = a.HasAccess,
                ClaimPolicy = a.ClaimPolicy
            }).ToList();
        return PartialView(roleDetails);
    }

    [HttpPost, Authorize(Policy = "51:c"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Form to search through authorizations.")]
    public async Task<IActionResult> ChangeAccess(ChangeAccess change)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Title = Resource.Warning, Description = Resource.InvalidData });
        }

        Menu menu = null; SubMenu subMenu = null;
        string claimType = string.Empty, claimValue = string.Empty;
        var getRole = await roleManager.FindByIdAsync(change.Role);

        if (!string.IsNullOrEmpty(change.SubMenuIde))
        {
            subMenu = await db.SubMenu.FindAsync(CryptoSecurity.Decrypt<int>(change.SubMenuIde));
            claimType = subMenu.Claim.Split(":")[0];
            claimValue = subMenu.Claim.Split(":")[1];
        }
        else
        {
            menu = await db.Menu.FindAsync(CryptoSecurity.Decrypt<int>(change.MenuIde));
            claimType = menu.Claim.Split(":")[0];
            claimValue = menu.Claim.Split(":")[1];
        }

        if (change.Access && !await db.AspNetRoleClaims.AnyAsync(a => a.ClaimType == claimType && a.ClaimValue == claimValue && a.RoleId == change.Role))
        {
            await roleManager.AddClaimAsync(getRole, new Claim(claimType, claimValue));
            if (!string.IsNullOrEmpty(change.SubMenuIde))
            {
                subMenu.Roles += getRole.Name + ", ";
            }
            else
            {
                menu.Roles += getRole.Name + ", ";
            }
        }
        else
        {
            await roleManager.RemoveClaimAsync(getRole, new Claim(claimType, claimValue));
            if (!string.IsNullOrEmpty(change.SubMenuIde))
            {
                subMenu.Roles = subMenu.Roles.Replace(getRole.Name, string.Empty);
            }
            else
            {
                menu.Roles = menu.Roles.Replace(getRole.Name, string.Empty);
            }
        }

        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Title = Resource.Success, Description = Resource.AccessChangedSuccessfully });
    }

    #endregion

    #region Application rules

    [Description("Arb Tahiri", "Entry form in application rules")]
    public async Task<IActionResult> Rules(string role)
    {
        var rules = new List<Rule>();
        Assembly.GetEntryAssembly().GetTypes().AsEnumerable()
            .Where(type => typeof(Controller).IsAssignableFrom(type)).ToList()
            .ForEach(a =>
            {
                a.GetMethods().Where(m => m.CustomAttributes.Any()).ToList()
                .ForEach(method =>
                {
                    var rule = new Rule
                    {
                        Controller = a.Name,
                        Method = method.Name
                    };

                    method.CustomAttributes.Where(b => b.AttributeType.Name == "AuthorizeAttribute" || b.AttributeType.Name == "DescriptionAttribute").ToList()
                    .ForEach(attr =>
                    {
                        if (attr.AttributeType.Name == "AuthorizeAttribute")
                        {
                            rule.Policy = attr.NamedArguments.Select(c => c.TypedValue.Value.ToString()).FirstOrDefault();
                        }
                        else if (attr.AttributeType.Name == "DescriptionAttribute")
                        {
                            rule.Description = attr.ConstructorArguments[0].ToString();
                        }
                    });

                    if (!string.IsNullOrEmpty(rule.Policy))
                    {
                        rules.Add(rule);
                    }
                });
            });

        rules = rules.Where(a => a.Policy.Split(":").Length > 1).Distinct(new PolicyComparer()).ToList();

        var menuPolicies = await db.Menu.Where(a => !string.IsNullOrEmpty(a.Claim)).Select(a => a.Claim).ToListAsync();
        var submenuPolicies = await db.SubMenu.Where(a => !string.IsNullOrEmpty(a.Claim)).Select(a => a.Claim).ToListAsync();
        var roleClaims = await db.AspNetRoleClaims.Where(a => a.RoleId == role).Select(a => new RoleClaim { ClaimType = a.ClaimType, ClaimValue = a.ClaimValue }).ToListAsync();

        roleClaims = roleClaims.Where(a => !(menuPolicies.Any(b => b.Split(":")[0] == a.ClaimType) && menuPolicies.Any(b => b.Split(":")[1] == a.ClaimValue))).ToList();
        roleClaims = roleClaims.Where(a => !(submenuPolicies.Any(b => b.Split(":")[0] == a.ClaimType) && submenuPolicies.Any(b => b.Split(":")[1] == a.ClaimValue))).ToList();

        rules.ForEach(rule => rule.HasAccess = roleClaims.Any(a => a.ClaimType == rule.Policy.Split(":")[0] && a.ClaimValue == rule.Policy.Split(":")[1]));
        return PartialView(rules);
    }

    [HttpPost, Authorize(Policy = "12:c"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Action to change access for methods with policies")]
    public async Task<IActionResult> ChangeMethodAccess(Claims claim)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Title = Resource.Warning, Description = Resource.InvalidData });
        }

        string claimType = claim.Policy.Split(":")[0], claimValue = claim.Policy.Split(":")[1];

        if (claim.Access && !await db.AspNetRoleClaims.AnyAsync(a => a.ClaimType == claimType && a.ClaimValue == claimValue && a.RoleId == claim.Role))
        {
            await roleManager.AddClaimAsync(await roleManager.FindByIdAsync(claim.Role), new Claim(claimType, claimValue));
        }
        else
        {
            await roleManager.RemoveClaimAsync(await roleManager.FindByIdAsync(claim.Role), new Claim(claimType, claimValue));
        }
        return Json(new ErrorVM { Status = ErrorStatus.Success, Title = Resource.Success, Description = Resource.AccessChangedSuccessfully });
    }

    #endregion

    [Description("Arb Tahiri", "Form to display tabs for Menu and Submenu.")]
    public IActionResult MenuIndex() => View();

    #region Menu

    #region => List

    [Authorize(Policy = "52:r"), Description("Arb Tahiri", "Form to display list of menus.")]
    public async Task<IActionResult> Menu()
    {
        var menus = await db.Menu.Select(a => new MenuDetails
        {
            MenuIde = CryptoSecurity.Encrypt(a.MenuId),
            Title = user.Language == LanguageEnum.Albanian ? a.NameSq : a.NameEn,
            Controller = a.Controller,
            Action = a.Action,
            HasSubMenu = a.HasSubMenu,
            Icon = a.Icon
        }).ToListAsync();
        return PartialView(menus);
    }

    #endregion

    #region => Create

    [HttpGet, Authorize(Policy = "52:c"), Description("Arb Tahiri", "Form to create a new menu.")]
    public IActionResult _CreateMenu() => PartialView();

    [HttpPost, Authorize(Policy = "52:c"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Action to create a new menu.")]
    public async Task<IActionResult> CreateMenu(CreateMenu create)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Title = Resource.Warning, Description = Resource.InvalidData });
        }

        db.Menu.Add(new Menu
        {
            NameSq = create.NameSq,
            NameEn = create.NameEn,
            HasSubMenu = create.HasSubMenu,
            Active = true,
            Icon = create.Icon,
            Claim = create.ClaimPolicy,
            ClaimType = create.ClaimPolicy?.Split(":")[0],
            Controller = create.Controller,
            Action = create.Action,
            OrdinalNumber = create.OrdinalNumber,
            OpenFor = create.OpenFor,
            InsertedFrom = user.Id,
            InsertedDate = DateTime.Now
        });
        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Title = Resource.Success, Description = Resource.DataRegisteredSuccessfully });
    }

    #endregion

    #region => Edit

    [HttpGet, Authorize(Policy = "52:e"), Description("Arb Tahiri", "Form to edit a new menu.")]
    public async Task<IActionResult> _EditMenu(string ide)
    {
        var menu = await db.Menu.FindAsync(CryptoSecurity.Decrypt<int>(ide));
        var edit = new EditMenu
        {
            MenuIde = ide,
            NameSq = menu.NameSq,
            NameEn = menu.NameEn,
            HasSubMenu = menu.HasSubMenu,
            Active = menu.Active,
            Icon = menu.Icon,
            ClaimPolicy = menu.Claim,
            Controller = menu.Controller,
            Action = menu.Action,
            OrdinalNumber = menu.OrdinalNumber,
            OpenFor = menu.OpenFor
        };
        return PartialView(edit);
    }

    [HttpPost, Authorize(Policy = "52:e"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Action to edit a new menu.")]
    public async Task<IActionResult> EditMenu(EditMenu edit)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Title = Resource.Warning, Description = Resource.InvalidData });
        }

        var menu = await db.Menu.FindAsync(CryptoSecurity.Decrypt<int>(edit.MenuIde));
        menu.NameSq = edit.NameSq;
        menu.NameEn = edit.NameEn;
        menu.HasSubMenu = edit.HasSubMenu;
        menu.Active = edit.Active;
        menu.Icon = edit.Icon;
        menu.Claim = edit.ClaimPolicy;
        menu.ClaimType = edit.ClaimPolicy?.Split(":")[0];
        menu.Controller = edit.Controller;
        menu.Action = edit.Action;
        menu.OrdinalNumber = edit.OrdinalNumber;
        menu.OpenFor = edit.OpenFor;
        menu.UpdatedFrom = user.Id;
        menu.UpdatedDate = DateTime.Now;
        menu.UpdatedNo = menu.UpdatedNo.HasValue ? ++menu.UpdatedNo : menu.UpdatedNo = 1;

        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Title = Resource.Success, Description = Resource.DataUpdatedSuccessfully });
    }

    #endregion

    #region => Delete

    [HttpPost, Authorize(Policy = "52:d"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Form to edit a new menu.")]
    public async Task<IActionResult> DeleteMenu(string ide)
    {
        db.Menu.Remove(await db.Menu.FindAsync(CryptoSecurity.Decrypt<int>(ide)));
        await db.SaveChangesAsync();

        return Json(new ErrorVM { Status = ErrorStatus.Success, Title = Resource.Success, Description = Resource.DataDeletedSuccessfully });
    }

    #endregion

    #endregion

    #region SubMenu

    #region => List

    [Authorize(Policy = "53:r"), Description("Arb Tahiri", "Form to dipslay list of submenus.")]
    public async Task<IActionResult> SubMenu()
    {
        var menus = await db.SubMenu.Include(a => a.Menu)
            .Select(a => new SubMenuDetails
            {
                SubMenuIde = CryptoSecurity.Encrypt(a.SubMenuId),
                Title = user.Language == LanguageEnum.Albanian ? a.NameSq : a.NameEn,
                MenuTitle = user.Language == LanguageEnum.Albanian ? a.Menu.NameSq : a.Menu.NameEn,
                Controller = a.Controller,
                Action = a.Action,
                Icon = a.Icon
            }).ToListAsync();
        return PartialView(menus);
    }

    #endregion

    #region => Create

    [HttpGet, Authorize(Policy = "53:c"), Description("Arb Tahiri", "Form to create a new submenu.")]
    public async Task<IActionResult> _CreateSubMenu(string ide)
    {
        var menu = await db.Menu.FindAsync(CryptoSecurity.Decrypt<int>(ide));
        var create = new CreateSubMenu
        {
            MenuIde = ide,
            MenuTitle = user.Language == LanguageEnum.Albanian ? menu.NameSq : menu.NameEn
        };
        return PartialView(create);
    }

    [HttpPost, Authorize(Policy = "53:c"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Action to create a new submenu.")]
    public async Task<IActionResult> CreateSubMenu(CreateSubMenu create)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Title = Resource.Warning, Description = Resource.InvalidData });
        }

        db.SubMenu.Add(new SubMenu
        {
            MenuId = CryptoSecurity.Decrypt<int>(create.MenuIde),
            NameSq = create.NameSq,
            NameEn = create.NameEn,
            Active = true,
            Icon = create.Icon,
            Claim = create.ClaimPolicy,
            ClaimType = create.ClaimPolicy?.Split(":")[0],
            Controller = create.Controller,
            Action = create.Action,
            OrdinalNumber = create.OrdinalNumber,
            OpenFor = create.OpenFor,
            InsertedFrom = user.Id,
            InsertedDate = DateTime.Now
        });
        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Title = Resource.Success, Description = Resource.DataRegisteredSuccessfully });
    }

    #endregion

    #region => Edit

    [HttpGet, Authorize(Policy = "53:e")]
    [Description("Arb Tahiri", "Form to edit a new submenu.")]
    public async Task<IActionResult> _EditSubMenu(string ide)
    {
        var submenu = await db.SubMenu.FindAsync(CryptoSecurity.Decrypt<int>(ide));
        var edit = new EditSubMenu
        {
            SubMenuIde = ide,
            NameSq = submenu.NameSq,
            NameEn = submenu.NameEn,
            Active = submenu.Active,
            Icon = submenu.Icon,
            ClaimPolicy = submenu.Claim,
            Controller = submenu.Controller,
            Action = submenu.Action,
            OrdinalNumber = submenu.OrdinalNumber,
            OpenFor = submenu.OpenFor
        };
        return PartialView(edit);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "53:e")]
    [Description("Arb Tahiri", "Action to edit a new submenu.")]
    public async Task<IActionResult> EditSubMenu(EditSubMenu edit)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Title = Resource.Warning, Description = Resource.InvalidData });
        }

        var submenu = await db.SubMenu.FindAsync(CryptoSecurity.Decrypt<int>(edit.SubMenuIde));
        submenu.NameSq = edit.NameSq;
        submenu.NameEn = edit.NameEn;
        submenu.Active = edit.Active;
        submenu.Icon = edit.Icon;
        submenu.Claim = edit.ClaimPolicy;
        submenu.ClaimType = edit.ClaimPolicy?.Split(":")[0];
        submenu.Controller = edit.Controller;
        submenu.Action = edit.Action;
        submenu.OrdinalNumber = edit.OrdinalNumber;
        submenu.OpenFor = edit.OpenFor;
        submenu.UpdatedFrom = user.Id;
        submenu.UpdatedDate = DateTime.Now;
        submenu.UpdatedNo = submenu.UpdatedNo.HasValue ? ++submenu.UpdatedNo : submenu.UpdatedNo = 1;

        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Title = Resource.Success, Description = Resource.DataUpdatedSuccessfully });
    }

    #endregion

    #region => Delete

    [HttpPost, Authorize(Policy = "53:d"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Form to edit a new submenu.")]
    public async Task<IActionResult> DeleteSubMenu(string ide)
    {
        db.SubMenu.Remove(await db.SubMenu.FindAsync(CryptoSecurity.Decrypt<int>(ide)));
        await db.SaveChangesAsync();

        return Json(new ErrorVM { Status = ErrorStatus.Success, Title = Resource.Success, Description = Resource.DataDeletedSuccessfully });
    }

    #endregion

    #endregion
}

public class PolicyComparer : IEqualityComparer<Rule>
{
    public bool Equals(Rule x, Rule y)
    {
        return x.Policy == y.Policy;
    }

    public int GetHashCode([DisallowNull] Rule obj)
    {
        return obj.Policy.GetHashCode();
    }
}
