namespace HRMS.Models.Staff;

public class Documents
{
    public string StaffDocumentIde { get; set; }
    public string Title { get; set; }
    public string Path { get; set; }
    public string PathExtension { get; set; }
    public string DocumentType { get; set; }
    public string Description { get; set; }
    public bool Active { get; set; }
}
