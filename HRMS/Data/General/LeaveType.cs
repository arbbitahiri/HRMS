using System;
using System.Collections.Generic;

namespace HRMS.Data.General
{
    public partial class LeaveType
    {
        public LeaveType()
        {
            Leave = new HashSet<Leave>();
            LeaveStaffDays = new HashSet<LeaveStaffDays>();
        }

        public int LeaveTypeId { get; set; }
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
        public virtual ICollection<Leave> Leave { get; set; }
        public virtual ICollection<LeaveStaffDays> LeaveStaffDays { get; set; }
    }
}
