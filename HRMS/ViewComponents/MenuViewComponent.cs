using HRMS.Data;
using HRMS.Data.Core;
using HRMS.Data.SqlFunctions;
using HRMS.Models.Shared;
using HRMS.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.ViewComponents;
public class MenuViewComponent : ViewComponent
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly IFunctionRepository repo;

    public MenuViewComponent(IFunctionRepository repo, UserManager<ApplicationUser> userManager)
    {
        this.repo = repo;
        this.userManager = userManager;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var menusTemp = new List<MenuList>();
        var user = (ApplicationUser)ViewData["InternalUser"];

        var roles = await userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            var getMenu = (await repo.MenuList(role, user.Language)).Where(a => !menusTemp.Any(n => n.SubMenuController == a.SubMenuController)).ToList();

            menusTemp.AddRange(getMenu);
        }

        var menus = menusTemp.OrderBy(a => a.MenuOrdinalNumber)
            .Select(a => new
            {
                a.MenuId,
                a.MenuTitle,
                a.HasSubMenu,
                a.MenuIcon,
                a.MenuController,
                a.MenuAction,
                a.MenuOpenFor
            }).Distinct().ToList()
            .Select(a => new MenuVM
            {
                Title = a.MenuTitle,
                HasSubmenu = a.HasSubMenu,
                Icon = a.MenuIcon,
                Controller = a.MenuController,
                Action = a.MenuAction,
                OpenFor = a.MenuOpenFor ?? "",
                Submenus = menusTemp.Where(b => b.MenuId == a.MenuId).OrderBy(a => a.SubMenuOrdinalNumber)
                    .Select(b => new SubmenuVM
                    {
                        SubmenuId = b.SubMenuId,
                        Title = b.SubMenuTitle,
                        Icon = b.SubMenuIcon,
                        Controller = b.SubMenuController,
                        Action = b.SubMenuAction,
                        OpenFor = b.SubMenuOpenFor
                    }).Distinct().ToList()
            }).ToList();

        return View(menus);
    }
}
