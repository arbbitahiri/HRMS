﻿using System;
using System.Collections.Generic;

#nullable disable

namespace HRMS.Data.General
{
    public partial class StatusType
    {
        public int StatusTypeId { get; set; }
        public string NameSq { get; set; }
        public string NameEn { get; set; }
        public bool Active { get; set; }
        public string InsertedFrom { get; set; }
        public DateTime InsertedDate { get; set; }
        public string UpdatedFrom { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedNo { get; set; }
    }
}