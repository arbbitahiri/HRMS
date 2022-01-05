using System;
using System.Collections.Generic;

namespace HRMS.Data.General
{
    public partial class HolidayRequest
    {
        public HolidayRequest()
        {
            HolidayRequestStatus = new HashSet<HolidayRequestStatus>();
        }

        public int HolidayRequestId { get; set; }
        public int HolidayTypeId { get; set; }
        public int StaffDepartmentId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public string InsertedFrom { get; set; }
        public DateTime InsertedDate { get; set; }
        public string UpdatedFrom { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedNo { get; set; }

        public virtual HolidayType HolidayType { get; set; }
        public virtual AspNetUsers InsertedFromNavigation { get; set; }
        public virtual StaffDepartment StaffDepartment { get; set; }
        public virtual AspNetUsers UpdatedFromNavigation { get; set; }
        public virtual ICollection<HolidayRequestStatus> HolidayRequestStatus { get; set; }
    }
}
