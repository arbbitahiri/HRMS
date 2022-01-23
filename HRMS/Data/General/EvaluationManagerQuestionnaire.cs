using System;
using System.Collections.Generic;

namespace HRMS.Data.General
{
    public partial class EvaluationManagerQuestionnaire
    {
        public int EvaluationManagerQuestionnaireId { get; set; }
        public int EvaluationManagerId { get; set; }
        public string Question { get; set; }
        public decimal? Grade { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public string InsertedFrom { get; set; }
        public DateTime InsertedDate { get; set; }
        public string UpdatedFrom { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedNo { get; set; }

        public virtual EvaluationManager EvaluationManager { get; set; }
        public virtual AspNetUsers InsertedFromNavigation { get; set; }
        public virtual AspNetUsers UpdatedFromNavigation { get; set; }
    }
}
