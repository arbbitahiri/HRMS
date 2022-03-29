using System.Collections.Generic;

namespace HRMS.Models.Evaluation;

public class QuestionOptional
{
    public string Question { get; set; }
    public List<QuestionOption> Options { get; set; }
}

public class QuestionOption
{
    public int OptionId { get; set; }
    public string Option { get; set; }
    public string OtherDescription { get; set; }
    public bool Checked { get; set; }
}