using System;
using System.Collections.Generic;

namespace HRMS.Data.General
{
    public partial class Document
    {
        public Document()
        {
            StaffDocument = new HashSet<StaffDocument>();
        }

        public int DocumentId { get; set; }
        public int DocumentTypeId { get; set; }
        public string Title { get; set; }
        public string FileName { get; set; }
        public string Path { get; set; }
        public bool Active { get; set; }
        public string InsertedFrom { get; set; }
        public DateTime InsertedDate { get; set; }
        public string UpdatedFrom { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedNo { get; set; }

        public virtual DocumentType DocumentType { get; set; }
        public virtual AspNetUsers InsertedFromNavigation { get; set; }
        public virtual AspNetUsers UpdatedFromNavigation { get; set; }
        public virtual ICollection<StaffDocument> StaffDocument { get; set; }
    }
}
