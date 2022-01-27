using System;
using System.Collections.Generic;

namespace HRMS.Data.General
{
    public partial class EvaluationQuestionnaireNumerical
    {
        public int EvaluationQuestionnaireNumericalId { get; set; }
        public int EvaluationId { get; set; }
        public string Question { get; set; }
        public int? Grade { get; set; }
        public bool Active { get; set; }
        public string InsertedFrom { get; set; }
        public DateTime InsertedDate { get; set; }
        public string UpdatedFrom { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedNo { get; set; }

        public virtual Evaluation Evaluation { get; set; }
        public virtual AspNetUsers InsertedFromNavigation { get; set; }
        public virtual AspNetUsers UpdatedFromNavigation { get; set; }
    }
}
