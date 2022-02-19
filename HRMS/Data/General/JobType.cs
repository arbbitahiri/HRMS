using System;
using System.Collections.Generic;

namespace HRMS.Data.General
{
    public partial class JobType
    {
        public JobType()
        {
            StaffDepartment = new HashSet<StaffDepartment>();
        }

        public int JobTypeId { get; set; }
        public string NameSq { get; set; }
        public string NameEn { get; set; }

        public virtual ICollection<StaffDepartment> StaffDepartment { get; set; }
    }
}
