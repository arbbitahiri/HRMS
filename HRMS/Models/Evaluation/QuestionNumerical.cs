namespace HRMS.Models.Evaluation;

public class QuestionNumerical
{
    public string EvaluationQuestionnaireNumericalIde { get; set; }
    public string Question { get; set; }
    public int? Grade { get; set; }
    public bool Graded { get; set; }
}