using System;
using System.Collections.Generic;

namespace HRMS.Data.General
{
    public partial class EvaluationStudents
    {
        public int EvaluationStudentsId { get; set; }
        public int EvaluationId { get; set; }
        public int StaffDepartmentSubjectId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string InsertedFrom { get; set; }
        public DateTime InsertedDate { get; set; }
        public string UpdatedFrom { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedNo { get; set; }

        public virtual Evaluation Evaluation { get; set; }
        public virtual AspNetUsers InsertedFromNavigation { get; set; }
        public virtual StaffDepartmentSubject StaffDepartmentSubject { get; set; }
        public virtual AspNetUsers UpdatedFromNavigation { get; set; }
    }
}
