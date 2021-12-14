using System;
using System.Collections.Generic;

namespace HRMS.Data.General
{
    public partial class StaffCollegeEvaluationQuestionnaire
    {
        public StaffCollegeEvaluationQuestionnaire()
        {
            StaffCollegeEvaluationQuestionnaireRate = new HashSet<StaffCollegeEvaluationQuestionnaireRate>();
        }

        public int StaffCollegeEvaluationQuestionnaireId { get; set; }
        public int StaffCollegeEvaluationId { get; set; }
        public string Title { get; set; }
        public int? RateTypeId { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public string InsertedFrom { get; set; }
        public DateTime InsertedDate { get; set; }
        public string UpdatedFrom { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedNo { get; set; }

        public virtual AspNetUsers InsertedFromNavigation { get; set; }
        public virtual RateType RateType { get; set; }
        public virtual StaffCollegeEvaluation StaffCollegeEvaluation { get; set; }
        public virtual AspNetUsers UpdatedFromNavigation { get; set; }
        public virtual ICollection<StaffCollegeEvaluationQuestionnaireRate> StaffCollegeEvaluationQuestionnaireRate { get; set; }
    }
}
