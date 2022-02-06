using HRMS.Utilities;
using System.Collections.Generic;

namespace HRMS.Models.EvaluationSelf;

public class QuestionVM
{
    public Details EvaluationDetails { get; set; }
    public List<QuestionNumerical> Numericals { get; set; }
    public List<QuestionOptional> Optionals { get; set; }
    public List<QuestionTopic> Topics { get; set; }

    public int TotalQuestions { get; set; }
    public MethodType Method { get; set; }
}
