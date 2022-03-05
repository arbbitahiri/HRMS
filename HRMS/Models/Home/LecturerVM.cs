using System.Collections.Generic;

namespace HRMS.Models.Home;

public class LecturerVM
{
    public int NumberOfStaffSubjects { get; set; }
    public int NumberOfQualifications { get; set; }
    public int NumberOfDocuments { get; set; }
    public int NumberOfAvailableLeave { get; set; }

    public List<Document> StaffDocuments { get; set; }
    public List<LogData> Logs { get; set; }
}

public class Document
{
    public string DocumentType { get; set; }
    public int DocumentCount { get; set; }
}
