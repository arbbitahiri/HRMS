using HRMS.Data.Core;
using HRMS.Data.General;
using HRMS.Utilities;
using HRMS.Utilities.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.Areas.Identity.Pages.Account.Manage;

public class NotificationsModel : BaseIModel
{
    public NotificationsModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, HRMSContext db)
        : base(signInManager, userManager, db)
    {
    }

    public List<NotificationModel> Notifications { get; set; }

    public async Task OnGetAsync()
    {
        Notifications = await db.Notification
            .Where(a => a.Receiver == user.Id && !a.Deleted)
            .Select(a => new NotificationModel
            {
                NotificationIde = CryptoSecurity.Encrypt(a.NotificationId),
                Title = a.Title,
                Description = a.Description,
                Url = a.Url,
                Icon = a.Icon,
                Read = a.Read,
                Date = a.InsertedDate.ToString("dd/MM/yyyy HH:mm"),
                NotificationType = (NotificationTypeEnum)a.Type
            }).ToListAsync();
    }
}

public class NotificationModel
{
    public string NotificationIde { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Url { get; set; }
    public string Icon { get; set; }
    public bool Read { get; set; }
    public string Date { get; set; }
    public NotificationTypeEnum NotificationType { get; set; }
}
