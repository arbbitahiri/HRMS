using System;
using System.Collections.Generic;

namespace HRMS.Data.General
{
    public partial class EvaluationQuestionnaireOptionalOption
    {
        public int EvaluationQuestionnaireOptionalOptionId { get; set; }
        public int EvaluationQuestionnaireOptionalId { get; set; }
        public string OptionTitle { get; set; }
        public bool Checked { get; set; }
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
