using HRMS.Utilities;
using System.Collections.Generic;

namespace HRMS.Areas.Evaluations.Models.StudentsStaff;

public class QuestionVM
{
    public Details EvaluationDetails { get; set; }
    public List<QuestionNumerical> Numerals { get; set; }
    public List<QuestionOptional> Optionals { get; set; }
    public int TotalQuestions { get; set; }
    public MethodType Method { get; set; }
    public int MaxQuestionOptions { get; set; }
}
