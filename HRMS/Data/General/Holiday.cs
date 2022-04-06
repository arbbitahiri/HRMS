using System;
using System.Collections.Generic;

namespace HRMS.Data.General
{
    public partial class Holiday
    {
        public int HolidayId { get; set; }
        public int HolidayTypeId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int RepeatTypeId { get; set; }
        public bool Active { get; set; }
        public string InsertedFrom { get; set; }
        public DateTime InsertedDate { get; set; }
        public string UpdatedFrom { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedNo { get; set; }

        public virtual HolidayType HolidayType { get; set; }
        public virtual AspNetUsers InsertedFromNavigation { get; set; }
        public virtual RepeatType RepeatType { get; set; }
        public virtual AspNetUsers UpdatedFromNavigation { get; set; }
    }
}
