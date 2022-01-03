using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace HRMS.Models.Staff.Document;

public class AddDocument
{
    public string StaffIde { get; set; }
    public string StaffDocumentIde { get; set; }

    public int DocumentTypeId { get; set; }
    public string Title { get; set; }
    public List<IFormFile> FormFiles { get; set; }
    public IFormFile FormFile { get; set; }
    public string Description { get; set; }

    public bool Active { get; set; }
}
