using System.Collections.Generic;

namespace HRMS.Models.EvaluationManager;

public class QuestionOptional
{
    public int OptionalId { get; set; }
    public string EvaluationQuestionnaireOptionalIde { get; set; }

    public string Question { get; set; }
    public string OtherDescription { get; set; }

    public List<QuestionOption> Options { get; set; }
}

public class QuestionOption
{
    public int OptionId { get; set; }
    public string EvaluationQuestionnaireOptionalOptionIde { get; set; }

    public string Option { get; set; }
    public bool DescNeeded { get; set; }
    public bool Checked { get; set; }
}
