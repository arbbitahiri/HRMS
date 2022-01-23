using System;
using System.Collections.Generic;

namespace HRMS.Data.General
{
    public partial class EvaluationStudents
    {
        public EvaluationStudents()
        {
            EvaluationStudentsDocument = new HashSet<EvaluationStudentsDocument>();
            EvaluationStudentsQuestionnaire = new HashSet<EvaluationStudentsQuestionnaire>();
        }

        public int EvaluationStudentsId { get; set; }
        public int EvalutaionId { get; set; }
        public int StaffDepartmentSubjectId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string InsertedFrom { get; set; }
        public DateTime InsertedDate { get; set; }
        public string UpdatedFrom { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedNo { get; set; }

        public virtual Evaluation Evalutaion { get; set; }
        public virtual AspNetUsers InsertedFromNavigation { get; set; }
        public virtual StaffDepartmentSubject StaffDepartmentSubject { get; set; }
        public virtual AspNetUsers UpdatedFromNavigation { get; set; }
        public virtual ICollection<EvaluationStudentsDocument> EvaluationStudentsDocument { get; set; }
        public virtual ICollection<EvaluationStudentsQuestionnaire> EvaluationStudentsQuestionnaire { get; set; }
    }
}
