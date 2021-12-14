﻿using System;
using System.Collections.Generic;

namespace HRMS.Data.General
{
    public partial class StaffCollege
    {
        public StaffCollege()
        {
            HolidayRequest = new HashSet<HolidayRequest>();
            StaffCollegeEvaluation = new HashSet<StaffCollegeEvaluation>();
            StaffCollegeSubject = new HashSet<StaffCollegeSubject>();
        }

        public int StaffCollegeId { get; set; }
        public int DepartmentId { get; set; }
        public int StaffId { get; set; }
        public int StaffTypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public string InsertedFrom { get; set; }
        public DateTime InsertedDate { get; set; }
        public string UpdatedFrom { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedNo { get; set; }

        public virtual Department Department { get; set; }
        public virtual AspNetUsers InsertedFromNavigation { get; set; }
        public virtual Staff Staff { get; set; }
        public virtual StaffType StaffType { get; set; }
        public virtual AspNetUsers UpdatedFromNavigation { get; set; }
        public virtual ICollection<HolidayRequest> HolidayRequest { get; set; }
        public virtual ICollection<StaffCollegeEvaluation> StaffCollegeEvaluation { get; set; }
        public virtual ICollection<StaffCollegeSubject> StaffCollegeSubject { get; set; }
    }
}
