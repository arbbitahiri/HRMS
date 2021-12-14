using System;
using System.Collections.Generic;

namespace HRMS.Data.General
{
    public partial class StatusType
    {
        public StatusType()
        {
            HolidayRequestStatus = new HashSet<HolidayRequestStatus>();
        }

        public int StatusTypeId { get; set; }
        public string NameSq { get; set; }
        public string NameEn { get; set; }
        public bool Active { get; set; }
        public string InsertedFrom { get; set; }
        public DateTime InsertedDate { get; set; }
        public string UpdatedFrom { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedNo { get; set; }

        public virtual AspNetUsers InsertedFromNavigation { get; set; }
        public virtual AspNetUsers UpdatedFromNavigation { get; set; }
        public virtual ICollection<HolidayRequestStatus> HolidayRequestStatus { get; set; }
    }
}
