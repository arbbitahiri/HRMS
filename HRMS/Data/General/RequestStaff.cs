using System;
using System.Collections.Generic;

namespace HRMS.Data.General
{
    public partial class RequestStaff
    {
        public int RequestStaffId { get; set; }
        public int RequestId { get; set; }
        public int StaffId { get; set; }
        public string InsertedFrom { get; set; }
        public DateTime InsertedDate { get; set; }
        public string UpdatedFrom { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedNo { get; set; }
    }
}
