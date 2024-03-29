﻿using System;
using System.Collections.Generic;

namespace HRMS.Data.General
{
    public partial class Staff
    {
        public Staff()
        {
            EvaluationManager = new HashSet<EvaluationManager>();
            EvaluationSelf = new HashSet<EvaluationSelf>();
            Leave = new HashSet<Leave>();
            LeaveStaffDays = new HashSet<LeaveStaffDays>();
            StaffDepartment = new HashSet<StaffDepartment>();
            StaffDocument = new HashSet<StaffDocument>();
            StaffPayroll = new HashSet<StaffPayroll>();
            StaffQualification = new HashSet<StaffQualification>();
            StaffRegistrationStatus = new HashSet<StaffRegistrationStatus>();
        }

        public int StaffId { get; set; }
        public string UserId { get; set; }
        public string PersonalNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthdate { get; set; }
        public int Gender { get; set; }
        public string City { get; set; }
        public int CountryId { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string Nationality { get; set; }
        public string InsertedFrom { get; set; }
        public DateTime InsertedDate { get; set; }
        public string UpdatedFrom { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedNo { get; set; }

        public virtual Country Country { get; set; }
        public virtual AspNetUsers UpdatedFromNavigation { get; set; }
        public virtual AspNetUsers User { get; set; }
        public virtual ICollection<EvaluationManager> EvaluationManager { get; set; }
        public virtual ICollection<EvaluationSelf> EvaluationSelf { get; set; }
        public virtual ICollection<Leave> Leave { get; set; }
        public virtual ICollection<LeaveStaffDays> LeaveStaffDays { get; set; }
        public virtual ICollection<StaffDepartment> StaffDepartment { get; set; }
        public virtual ICollection<StaffDocument> StaffDocument { get; set; }
        public virtual ICollection<StaffPayroll> StaffPayroll { get; set; }
        public virtual ICollection<StaffQualification> StaffQualification { get; set; }
        public virtual ICollection<StaffRegistrationStatus> StaffRegistrationStatus { get; set; }
    }
}
