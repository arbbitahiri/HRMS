using System;
using System.Collections.Generic;

namespace HRMS.Data.General
{
    public partial class StaffDepartmentEvaluationQuestionnaireRate
    {
        public int StaffDepartmentEvaluationQuestionnaireRateId { get; set; }
        public int StaffDepartmentEvaluationQuestionnaireId { get; set; }
        public int RateTypeId { get; set; }
        public bool Active { get; set; }
        public string InsertedFrom { get; set; }
        public DateTime InsertedDate { get; set; }
        public string UpdatedFrom { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedNo { get; set; }

        public virtual RateType RateType { get; set; }
        public virtual StaffDepartmentEvaluationQuestionnaire StaffDepartmentEvaluationQuestionnaire { get; set; }
    }
}
