using System.Collections.Generic;

namespace HRMS.Models.EvaluationManager;

public class QuestionOptional
{
    public string EvaluationQuestionnaireOptionalIde { get; set; }

    public string Question { get; set; }
    public List<QuestionOption> Options { get; set; }
}

public class QuestionOption
{
    public string EvaluationQuestionnaireOptionalOptionIde { get; set; }

    public string Option { get; set; }
    public bool Checked { get; set; }
}
