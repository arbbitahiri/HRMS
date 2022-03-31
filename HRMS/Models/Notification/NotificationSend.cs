using HRMS.Utilities;

namespace HRMS.Models.Notification;

public class NotificationSend
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Url { get; set; }
    public string Icon { get; set; }
    public string Target { get; set; }
    public NotificationTypeEnum NotificationType { get; set; }
}
