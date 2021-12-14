using System;
using System.Collections.Generic;

namespace HRMS.Data.General
{
    public partial class AppSettings
    {
        public int HistoryAppSettingsId { get; set; }
        public string OldVersion { get; set; }
        public string UpdatedVersion { get; set; }
        public string InsertedFrom { get; set; }
        public DateTime IndertedDate { get; set; }
    }
}
