using HRMS.Data.General;
using HRMS.Models.Notification;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.Utilities.Notifications;

public class NotificationUtility
{
    private readonly HRMSContext db;
    private readonly IHubContext<NotificationHub> hubContext;

    public NotificationUtility(HRMSContext db, IHubContext<NotificationHub> hubContext)
    {
        this.db = db;
        this.hubContext = hubContext;
    }

    public async Task SendNotification(string sender, List<string> receivers, NotificationSend notification)
    {
        await hubContext.Clients.Users(receivers).SendAsync("Notification", notification);
        var notifications = new List<Notification>();
        receivers.ForEach(receiver =>
        {
            notifications.Add(new Notification
            {
                Receiver = receiver,
                Title = notification.Title,
                Description = notification.Description,
                Url = notification.Url,
                Icon = notification.Icon,
                Read = false,
                Deleted = false,
                Type = (int)notification.NotificationType,
                InsertedDate = DateTime.Now,
                InsertedFrom = sender,
            });
        });

        await db.Notification.AddRangeAsync(notifications);
        await db.SaveChangesAsync();

        var unreadNotifications = await db.Notification
            .Where(a => receivers.Contains(a.Receiver))
            .Select(a => new
            {
                a.Receiver,
                a.Read
            }).ToListAsync();

        unreadNotifications.Select(a => a.Receiver).Distinct().ToList().ForEach(async receiver =>
        {
            await hubContext.Clients.User(receiver).SendAsync("UnreadNotification", unreadNotifications.Count(a => a.Receiver == receiver && !a.Read));
        });
    }
}
