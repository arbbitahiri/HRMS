using Microsoft.AspNetCore.Http;

namespace HRMS.Models.EvaluationSelf;

public class ManageDocument
{
    public string EvaluationDocumentIde { get; set; }
    public string EvaluationIde { get; set; }

    public int DocumentTypeId { get; set; }
    public string Title { get; set; }
    public IFormFile DocumentFile { get; set; }
    public string Description { get; set; }
    public bool Active { get; set; }

    public string FileSize { get; set; }
}
