using System;
using System.Collections.Generic;

namespace HRMS.Data.General
{
    public partial class Staff
    {
        public Staff()
        {
            HolidayRequest = new HashSet<HolidayRequest>();
            StaffDepartment = new HashSet<StaffDepartment>();
            StaffDocument = new HashSet<StaffDocument>();
            StaffQualification = new HashSet<StaffQualification>();
        }

        public int StaffId { get; set; }
        public string UserId { get; set; }
        public string PersonalNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthdate { get; set; }
        public int Gender { get; set; }
        public string BirthPlace { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string Nationality { get; set; }
        public string InsertedFrom { get; set; }
        public DateTime InsertedDate { get; set; }
        public string UpdatedFrom { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedNo { get; set; }

        public virtual AspNetUsers UpdatedFromNavigation { get; set; }
        public virtual AspNetUsers User { get; set; }
        public virtual ICollection<HolidayRequest> HolidayRequest { get; set; }
        public virtual ICollection<StaffDepartment> StaffDepartment { get; set; }
        public virtual ICollection<StaffDocument> StaffDocument { get; set; }
        public virtual ICollection<StaffQualification> StaffQualification { get; set; }
    }
}
