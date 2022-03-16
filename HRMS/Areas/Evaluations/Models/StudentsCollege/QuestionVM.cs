using HRMS.Utilities;
using System.Collections.Generic;

namespace HRMS.Areas.Evaluations.Models.StudentsCollege;

public class QuestionVM
{
    public string EvaluationIde { get; set; }
    public List<QuestionNumerical> Numerals { get; set; }
    public List<QuestionTopic> OptionalTopics { get; set; }
    public int TotalQuestions { get; set; }
    public MethodType Method { get; set; }
}
