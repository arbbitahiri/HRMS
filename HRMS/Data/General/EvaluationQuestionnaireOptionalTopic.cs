using System;
using System.Collections.Generic;

namespace HRMS.Data.General
{
    public partial class EvaluationQuestionnaireOptionalTopic
    {
        public int EvaluationQuestionnaireOptionalTopicId { get; set; }
        public int EvaluationQuestionnaireOptionalId { get; set; }
        public string Answer { get; set; }
        public bool Active { get; set; }
        public string InsertedFrom { get; set; }
        public DateTime InsertedDate { get; set; }
        public string UpdatedFrom { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedNo { get; set; }

        public virtual EvaluationQuestionnaireOptional EvaluationQuestionnaireOptional { get; set; }
        public virtual AspNetUsers InsertedFromNavigation { get; set; }
        public virtual AspNetUsers UpdatedFromNavigation { get; set; }
    }
}
