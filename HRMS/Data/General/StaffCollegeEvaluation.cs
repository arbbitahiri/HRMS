using System;
using System.Collections.Generic;

namespace HRMS.Data.General
{
    public partial class StaffCollegeEvaluation
    {
        public StaffCollegeEvaluation()
        {
            StaffCollegeEvaluationQuestionnaire = new HashSet<StaffCollegeEvaluationQuestionnaire>();
        }

        public int StaffCollegeEvaluationId { get; set; }
        public int StaffCollegeId { get; set; }
        public int EvaluationTypeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public string InsertedFrom { get; set; }
        public DateTime InsertedDate { get; set; }
        public string UpdatedFrom { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedNo { get; set; }

        public virtual EvaluationType EvaluationType { get; set; }
        public virtual AspNetUsers InsertedFromNavigation { get; set; }
        public virtual StaffCollege StaffCollege { get; set; }
        public virtual AspNetUsers UpdatedFromNavigation { get; set; }
        public virtual ICollection<StaffCollegeEvaluationQuestionnaire> StaffCollegeEvaluationQuestionnaire { get; set; }
    }
}
