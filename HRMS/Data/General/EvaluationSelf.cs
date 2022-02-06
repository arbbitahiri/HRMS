using System;
using System.Collections.Generic;

namespace HRMS.Data.General
{
    public partial class EvaluationSelf
    {
        public int EvaluationSelfId { get; set; }
        public int EvaluationId { get; set; }
        public int StaffId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string InsertedFrom { get; set; }
        public DateTime InsertedDate { get; set; }
        public string UpdatedFrom { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedNo { get; set; }

        public virtual Evaluation Evaluation { get; set; }
        public virtual AspNetUsers InsertedFromNavigation { get; set; }
        public virtual Staff Staff { get; set; }
        public virtual AspNetUsers UpdatedFromNavigation { get; set; }
    }
}
