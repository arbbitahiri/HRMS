using System;
using System.Collections.Generic;

namespace HRMS.Data.General
{
    public partial class EvaluationSelfDocument
    {
        public int EvaluationSelfDocumentId { get; set; }
        public int EvaluationSelfId { get; set; }
        public int DocumentTypeId { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public bool Active { get; set; }
        public string InsertedFrom { get; set; }
        public DateTime InsertedDate { get; set; }
        public string UpdatedFrom { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedNo { get; set; }

        public virtual DocumentType DocumentType { get; set; }
        public virtual EvaluationSelf EvaluationSelf { get; set; }
        public virtual AspNetUsers InsertedFromNavigation { get; set; }
        public virtual AspNetUsers UpdatedFromNavigation { get; set; }
    }
}
