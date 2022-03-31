using HRMS.Utilities;
using System;

namespace HRMS.Models.Notification;

public class NotificationList
{
    public string NotificationIde { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Url { get; set; }
    public DateTime Date { get; set; }
    public string Icon { get; set; }
    public bool Read { get; set; }
    public NotificationTypeEnum NotificationType { get; set; }
    public string DaysAgo { get; set; }
    public string InsertedFrom { get; set; }
    public string MarkAsRead { get; set; }
    public string LoadMore { get; set; }
    public int Language { get; set; }
}
