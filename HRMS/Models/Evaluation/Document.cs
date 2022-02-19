namespace HRMS.Models.Evaluation;

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