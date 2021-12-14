using HRMS.Data.Core;
using HRMS.Data.General;
using HRMS.Models.Home.SideProfile;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.ViewComponents;
public class ProfileViewComponent : ViewComponent
{
    private readonly HRMSContext db;

    public ProfileViewComponent(HRMSContext db)
    {
        this.db = db;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var user = (ApplicationUser)ViewData["InternalUser"];
        var sideProfile = await db.AspNetUsers
            .Where(a => a.Id == user.Id)
            .Select(a => new SideProfile
            {
                Name = $"{a.FirstName} {a.LastName}",
                ProfileImage = a.ProfileImage ?? null,
                Username = a.UserName,
                Roles = null
            }).FirstOrDefaultAsync();

        return View(sideProfile);
    }
}
