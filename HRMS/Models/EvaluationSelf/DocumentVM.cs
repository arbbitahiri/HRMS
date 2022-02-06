using HRMS.Utilities;
using System.Collections.Generic;

namespace HRMS.Models.EvaluationSelf;

public class DocumentVM
{
    public Details EvaluationDetails { get; set; }
    public List<Document> Documents { get; set; }

    public int DocumentCount { get; set; }
    public MethodType Method { get; set; }
}
