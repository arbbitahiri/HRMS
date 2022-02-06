using HRMS.Utilities;
using System.Collections.Generic;

namespace HRMS.Areas.Evaluations.Models.Self;

public class Document
{
    public string EvaluationDocumentIde { get; set; }
    public string Title { get; set; }
    public string Path { get; set; }
    public string PathExtension { get; set; }
    public string DocumentType { get; set; }
    public string Description { get; set; }
    public bool Active { get; set; }
}

public class DocumentVM
{
    public Details EvaluationDetails { get; set; }
    public List<Document> Documents { get; set; }

    public int DocumentCount { get; set; }
    public MethodType Method { get; set; }
}
