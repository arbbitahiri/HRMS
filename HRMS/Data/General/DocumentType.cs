using System;
using System.Collections.Generic;

namespace HRMS.Data.General
{
    public partial class DocumentType
    {
        public DocumentType()
        {
            EvaluationDocument = new HashSet<EvaluationDocument>();
            StaffDocument = new HashSet<StaffDocument>();
        }

        public int DocumentTypeId { get; set; }
        public string NameSq { get; set; }
        public string NameEn { get; set; }
        public bool Active { get; set; }
        public string InsertedFrom { get; set; }
        public DateTime InsertedDate { get; set; }
        public string UpdatedFrom { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedNo { get; set; }

        public virtual AspNetUsers InsertedFromNavigation { get; set; }
        public virtual AspNetUsers UpdatedFromNavigation { get; set; }
        public virtual ICollection<EvaluationDocument> EvaluationDocument { get; set; }
        public virtual ICollection<StaffDocument> StaffDocument { get; set; }
    }
}
