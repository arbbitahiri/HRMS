﻿using System;
using System.Collections.Generic;

namespace HRMS.Data.General
{
    public partial class HolidayType
    {
        public HolidayType()
        {
            Holiday = new HashSet<Holiday>();
        }

        public int HolidayTypeId { get; set; }
        public string NameSq { get; set; }
        public string NameEn { get; set; }
        public string DescriptionSq { get; set; }
        public string DescriptionEn { get; set; }
        public bool Active { get; set; }
        public string InsertedFrom { get; set; }
        public DateTime InsertedDate { get; set; }
        public string UpdatedFrom { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedNo { get; set; }

        public virtual AspNetUsers InsertedFromNavigation { get; set; }
        public virtual AspNetUsers UpdatedFromNavigation { get; set; }
        public virtual ICollection<Holiday> Holiday { get; set; }
    }
}
