using HRMS.Utilities;
using System.Collections.Generic;

namespace HRMS.Models.Staff.Document;

public class DocumentsVM
{
    public StaffDetails StaffDetails { get; set; }
    public List<Documents> Documents { get; set; }

    public int DocumentCount { get; set; }
    public MethodType MethodType { get; set; }
}
