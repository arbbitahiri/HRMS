using System;
using System.Collections.Generic;

namespace HRMS.Data.General
{
    public partial class StaffCollegeSubject
    {
        public int StaffCollegeSubjectId { get; set; }
        public int StaffCollegeId { get; set; }
        public int SubjectId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Active { get; set; }
        public string InsertedFrom { get; set; }
        public DateTime InsertedDate { get; set; }
        public string UpdatedFrom { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedNo { get; set; }

        public virtual AspNetUsers InsertedFromNavigation { get; set; }
        public virtual StaffCollege StaffCollege { get; set; }
        public virtual Subject Subject { get; set; }
        public virtual AspNetUsers UpdatedFromNavigation { get; set; }
    }
}
