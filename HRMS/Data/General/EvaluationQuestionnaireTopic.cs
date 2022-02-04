using System;
using System.Collections.Generic;

namespace HRMS.Data.General
{
    public partial class EvaluationQuestionnaireTopic
    {
        public int EvaluationQuestionnaireTopicId { get; set; }
        public int EvaluationId { get; set; }
        public string QuestionSq { get; set; }
        public string QuestionEn { get; set; }
        public string Answer { get; set; }
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
