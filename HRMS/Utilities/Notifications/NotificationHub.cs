using HRMS.Data.General;
using HRMS.Models.Notification;
using HRMS.Resources;
using HRMS.Utilities.Security;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HRMS.Utilities.Notifications;

public class NotificationHub : Hub
{
    private readonly HRMSContext db;

    public NotificationHub(HRMSContext db)
    {
        this.db = db;
    }

    public async Task SendMessage(string user, string message) =>
        await Clients.All.SendAsync("Notification", user, message);

    public async Task UnreadNotifications()
    {
        string userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        await Clients.User(userId).SendAsync("UnreadNotification", await db.Notification.CountAsync(a => a.Receiver == userId && !a.Read && !a.Deleted));
    }

    public async Task UserNotifications()
    {
        string userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var notifications = await db.Notification
            .Where(a => a.Receiver == userId && !a.Deleted)
            .OrderByDescending(a => a.InsertedDate)
            .Select(a => new NotificationList
            {
                Title = a.Title,
                Description = a.Description,
                Url = a.Url,
                Date = a.InsertedDate,
                Icon = a.Icon,
                Read = a.Read,
                NotificationType = (NotificationTypeEnum)a.Type
            }).ToListAsync();

        notifications.ForEach(a =>
        {
            a.Icon = $"{a.Icon} {NotificationType(a.NotificationType)}";
            a.DaysAgo = DateTimeDifference(a.Date, DateTime.Now, 1);
        });

        await Clients.User(userId).SendAsync("UserLastFiveNotifications", notifications.Take(5));
    }

    public async Task Notifications(string rowNumber)
    {
        int counter = Convert.ToInt32(rowNumber);
        counter += 5;

        string userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var notifications = await db.Notification.Where(a => a.Receiver == userId).OrderByDescending(a => a.InsertedDate).ToListAsync();
        int language = await db.AspNetUsers.Where(a => a.Id == userId).Select(a => a.Language).FirstOrDefaultAsync();
        notifications.ForEach(async n =>
        {
            var recentNotifications = notifications
                .Where(a => !a.Deleted)
                .Select(a => new NotificationList
                {
                    NotificationIde = CryptoSecurity.Encrypt(a.NotificationId),
                    Title = a.Title,
                    Description = a.Description,
                    Url = a.Url,
                    Date = a.InsertedDate,
                    Icon = a.Icon,
                    Read = a.Read,
                    NotificationType = (NotificationTypeEnum)a.Type,
                    InsertedFrom = a.InsertedFrom,
                    MarkAsRead = Resource.MarkAsRead,
                    LoadMore = Resource.LoadMore,
                    Language = language
                }).Take(counter).ToList();

            recentNotifications.ForEach(r =>
            {
                r.Icon = $"{r.Icon} {NotificationType(r.NotificationType)}";
                r.DaysAgo = DateTimeDifference(r.Date, DateTime.Now, 1);
            });

            var noLoadMore = notifications.Count <= recentNotifications.Count;
            await Clients.Users(userId).SendAsync("SendNotification", recentNotifications, recentNotifications.Count, noLoadMore, Resource.LoadMore);
            await Clients.Users(userId).SendAsync("LastFiveNotifications", recentNotifications.Take(5));
        });
        await UnreadNotifications();
    }

    public string NotificationType(NotificationTypeEnum notificationType) =>
        notificationType switch
        {
            NotificationTypeEnum.Success => "text-success",
            NotificationTypeEnum.Info => "text-info",
            NotificationTypeEnum.Warning => "text-warning",
            NotificationTypeEnum.Error => "text-danger",
            _ => "text-default"
        };

    public string DateTimeDifference(DateTime start, DateTime end, int type)
    {
        var timeSpan = end.Subtract(start);
        var totalDays = (int)timeSpan.TotalDays;
        var totalHours = (int)timeSpan.TotalHours;
        var totalMinutes = (int)timeSpan.TotalMinutes;
        var totalSeconds = (int)timeSpan.TotalDays;

        var days = type == 1 ? (totalDays != 1 ? Resource.DaysAgo.ToLower() : Resource.DayAgo.ToLower()) : (totalDays != 1 ? Resource.Days.ToLower() : Resource.Day.ToLower());
        var hours = type == 1 ? (totalHours != 1 ? Resource.HoursAgo.ToLower() : Resource.HourAgo.ToLower()) : (totalHours != 1 ? Resource.Hours.ToLower() : Resource.Hour.ToLower());
        var minutes = type == 1 ? (totalMinutes != 1 ? Resource.MinutesAgo.ToLower() : Resource.MinuteAgo.ToLower()) : (totalMinutes != 1 ? Resource.Minutes.ToLower() : Resource.Minute.ToLower());
        var seconds = type == 1 ? (totalSeconds != 1 ? Resource.SecondsAgo.ToLower() : Resource.SecondAgo.ToLower()) : (totalSeconds != 1 ? Resource.Seconds.ToLower() : Resource.Second.ToLower());

        return totalDays > 0 ? $"{totalDays} {days}" : (totalHours > 0 ? $"{totalHours} {hours}" : (totalMinutes > 0 ? $"{totalMinutes} {minutes}" : (totalSeconds > 0 ? $"{totalSeconds} {seconds}" : "")));
    }

    public override async Task OnConnectedAsync()
    {
        if (Context.User.IsInRole("Administrator"))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Admin");
        }
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        return base.OnDisconnectedAsync(exception);
    }
}
