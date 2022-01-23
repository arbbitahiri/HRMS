using System;
using System.Collections.Generic;

namespace HRMS.Data.General
{
    public partial class StatusType
    {
        public StatusType()
        {
            EvaluationStatus = new HashSet<EvaluationStatus>();
            HolidayStatus = new HashSet<HolidayStatus>();
            StaffRegistrationStatus = new HashSet<StaffRegistrationStatus>();
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
        public virtual ICollection<EvaluationStatus> EvaluationStatus { get; set; }
        public virtual ICollection<HolidayStatus> HolidayStatus { get; set; }
        public virtual ICollection<StaffRegistrationStatus> StaffRegistrationStatus { get; set; }
    }
}
