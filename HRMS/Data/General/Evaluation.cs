using System;
using System.Collections.Generic;

namespace HRMS.Data.General
{
    public partial class Evaluation
    {
        public Evaluation()
        {
            EvaluationManager = new HashSet<EvaluationManager>();
            EvaluationSelf = new HashSet<EvaluationSelf>();
            EvaluationStatus = new HashSet<EvaluationStatus>();
            EvaluationStudents = new HashSet<EvaluationStudents>();
        }

        public int EvaluationId { get; set; }
        public int EvaluationTypeId { get; set; }
        public string InsertedFrom { get; set; }
        public DateTime InsertedDate { get; set; }
        public string UpdatedFrom { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedNo { get; set; }

        public virtual EvaluationType EvaluationType { get; set; }
        public virtual AspNetUsers InsertedFromNavigation { get; set; }
        public virtual AspNetUsers UpdatedFromNavigation { get; set; }
        public virtual ICollection<EvaluationManager> EvaluationManager { get; set; }
        public virtual ICollection<EvaluationSelf> EvaluationSelf { get; set; }
        public virtual ICollection<EvaluationStatus> EvaluationStatus { get; set; }
        public virtual ICollection<EvaluationStudents> EvaluationStudents { get; set; }
    }
}
