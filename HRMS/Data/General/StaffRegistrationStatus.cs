using System;
using System.Collections.Generic;

namespace HRMS.Data.General
{
    public partial class StaffRegistrationStatus
    {
        public int StaffRegistrationStatusId { get; set; }
        public int StaffId { get; set; }
        public int StatusTypeId { get; set; }
        public bool Active { get; set; }
        public string InsertedFrom { get; set; }
        public DateTime InsertedDate { get; set; }
        public string UpdatedFrom { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedNo { get; set; }

        public virtual AspNetUsers InsertedFromNavigation { get; set; }
        public virtual Staff Staff { get; set; }
        public virtual StatusType StatusType { get; set; }
        public virtual AspNetUsers UpdatedFromNavigation { get; set; }
    }
}
