using System;

namespace HRMS.Models.EvaluationManager;

public class QuestionNumerical
{
    public string EvaluationQuestionnaireNumericalIde { get; set; }

    public string Title { get; set; }
    public int? Grade { get; set; }

    public bool Graded { get; set; }
}
