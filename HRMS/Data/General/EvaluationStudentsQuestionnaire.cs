using System;
using System.Collections.Generic;

namespace HRMS.Data.General
{
    public partial class EvaluationStudentsQuestionnaire
    {
        public int EvaluationStudentsQuestionnaireId { get; set; }
        public int EvaluationStudentsId { get; set; }
        public string Question { get; set; }
        public decimal? Grade { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public string InsertedFrom { get; set; }
        public DateTime InsertedDate { get; set; }
        public string UpdatedFrom { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedNo { get; set; }

        public virtual EvaluationStudents EvaluationStudents { get; set; }
        public virtual AspNetUsers InsertedFromNavigation { get; set; }
        public virtual AspNetUsers UpdatedFromNavigation { get; set; }
    }
}
