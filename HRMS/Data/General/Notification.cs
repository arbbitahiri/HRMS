using System;
using System.Collections.Generic;

namespace HRMS.Data.General
{
    public partial class Notification
    {
        public int NotificationId { get; set; }
        public int Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string Receiver { get; set; }
        public string Icon { get; set; }
        public bool Read { get; set; }
        public bool Deleted { get; set; }
        public string InsertedFrom { get; set; }
        public DateTime InsertedDate { get; set; }

        public virtual AspNetUsers InsertedFromNavigation { get; set; }
        public virtual AspNetUsers ReceiverNavigation { get; set; }
    }
}
