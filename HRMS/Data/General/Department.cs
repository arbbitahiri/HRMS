using System;
using System.Collections.Generic;

namespace HRMS.Data.General
{
    public partial class Department
    {
        public Department()
        {
            AspNetUsers = new HashSet<AspNetUsers>();
            StaffDepartment = new HashSet<StaffDepartment>();
            StaffPayroll = new HashSet<StaffPayroll>();
        }

        public int DepartmentId { get; set; }
        public string Code { get; set; }
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
        public virtual ICollection<AspNetUsers> AspNetUsers { get; set; }
        public virtual ICollection<StaffDepartment> StaffDepartment { get; set; }
        public virtual ICollection<StaffPayroll> StaffPayroll { get; set; }
    }
}
