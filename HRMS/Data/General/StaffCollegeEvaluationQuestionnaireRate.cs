using System;
using System.Collections.Generic;

namespace HRMS.Data.General
{
    public partial class StaffCollegeEvaluationQuestionnaireRate
    {
        public int StaffCollegeEvaluationQuestionnaireRateId { get; set; }
        public int StaffCollegeEvaluationQuestionnaireId { get; set; }
        public int RateTypeId { get; set; }
        public bool Active { get; set; }
        public string InsertedFrom { get; set; }
        public DateTime InsertedDate { get; set; }
        public string UpdatedFrom { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedNo { get; set; }

        public virtual RateType RateType { get; set; }
        public virtual StaffCollegeEvaluationQuestionnaire StaffCollegeEvaluationQuestionnaire { get; set; }
    }
}
