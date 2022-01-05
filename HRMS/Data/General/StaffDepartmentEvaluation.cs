using System;
using System.Collections.Generic;

namespace HRMS.Data.General
{
    public partial class StaffDepartmentEvaluation
    {
        public StaffDepartmentEvaluation()
        {
            StaffDepartmentEvaluationQuestionnaire = new HashSet<StaffDepartmentEvaluationQuestionnaire>();
        }

        public int StaffDepartmentEvaluationId { get; set; }
        public int StaffDepartmentId { get; set; }
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
        public virtual StaffDepartment StaffDepartment { get; set; }
        public virtual AspNetUsers UpdatedFromNavigation { get; set; }
        public virtual ICollection<StaffDepartmentEvaluationQuestionnaire> StaffDepartmentEvaluationQuestionnaire { get; set; }
    }
}
