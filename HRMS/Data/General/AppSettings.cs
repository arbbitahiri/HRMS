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
        public string UpdatedFrom { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedNo { get; set; }

        public virtual AspNetUsers InsertedFromNavigation { get; set; }
        public virtual AspNetUsers UpdatedFromNavigation { get; set; }
    }
}
