using System;
using System.Collections.Generic;

namespace HRMS.Data.General
{
    public partial class HolidayStatus
    {
        public int HolidayStatusId { get; set; }
        public int HolidayId { get; set; }
        public int StatusTypeId { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public string InsertedFrom { get; set; }
        public DateTime InsertedDate { get; set; }
        public string UpdatedFrom { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedNo { get; set; }

        public virtual Holiday Holiday { get; set; }
        public virtual AspNetUsers InsertedFromNavigation { get; set; }
        public virtual StatusType StatusType { get; set; }
        public virtual AspNetUsers UpdatedFromNavigation { get; set; }
    }
}
