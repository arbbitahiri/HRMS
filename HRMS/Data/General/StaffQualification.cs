using System;
using System.Collections.Generic;

namespace HRMS.Data.General
{
    public partial class StaffQualification
    {
        public int StaffQualificationId { get; set; }
        public int StaffId { get; set; }
        public int ProffessionTypeId { get; set; }
        public int EducationLevelTypeId { get; set; }
        public bool Training { get; set; }
        public string Title { get; set; }
        public string FieldStudy { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public DateTime From { get; set; }
        public DateTime? To { get; set; }
        public bool OnGoing { get; set; }
        public string Description { get; set; }
        public decimal? FinalGrade { get; set; }
        public string Thesis { get; set; }
        public string CreditType { get; set; }
        public int? CreditNumber { get; set; }
        public DateTime? Validity { get; set; }
        public string InsertedFrom { get; set; }
        public DateTime InsertedDate { get; set; }
        public string UpdatedFrom { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedNo { get; set; }

        public virtual EducationLevelType EducationLevelType { get; set; }
        public virtual AspNetUsers InsertedFromNavigation { get; set; }
        public virtual ProfessionType ProffessionType { get; set; }
        public virtual Staff Staff { get; set; }
        public virtual AspNetUsers UpdatedFromNavigation { get; set; }
    }
}
