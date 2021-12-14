using System;
using System.Collections.Generic;

namespace HRMS.Data.General
{
    public partial class RateType
    {
        public RateType()
        {
            StaffCollegeEvaluationQuestionnaire = new HashSet<StaffCollegeEvaluationQuestionnaire>();
            StaffCollegeEvaluationQuestionnaireRate = new HashSet<StaffCollegeEvaluationQuestionnaireRate>();
        }

        public int RateTypeId { get; set; }
        public int RateNumber { get; set; }
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
        public virtual ICollection<StaffCollegeEvaluationQuestionnaire> StaffCollegeEvaluationQuestionnaire { get; set; }
        public virtual ICollection<StaffCollegeEvaluationQuestionnaireRate> StaffCollegeEvaluationQuestionnaireRate { get; set; }
    }
}
