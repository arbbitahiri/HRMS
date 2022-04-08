﻿using HRMS.Utilities;

namespace HRMS.Models.Notification;

public class NotificationSend
{
    public string title { get; set; }
    public string description { get; set; }
    public string url { get; set; }
    public string icon { get; set; }
    public string target { get; set; }
    public string background { get; set; }
    public string notificationType { get; set; }
    public NotificationTypeEnum NotificationType { get; set; }
}
