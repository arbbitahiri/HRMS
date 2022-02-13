namespace HRMS.Areas.Evaluations.Models.StudentsStaff;

public class QuestionNumerical
{
    public int NumericalId { get; set; }
    public string EvaluationQuestionnaireNumericalIde { get; set; }

    public string Question { get; set; }
    public int? Grade { get; set; }

    public bool Graded { get; set; }
}
