using HRMS.Models.EvaluationManager;
using System.Collections.Generic;

namespace HRMS.Models.Evaluation;

public class DetailsVM
{
    public EvaluationDetails EvaluationDetails { get; set; }
    public List<QuestionNumerical> QuestionNumericals { get; set; }
    public List<QuestionOptional> QuestionOptionals { get; set; }
    public List<QuestionTopic> QuestionTopics { get; set; }
    public List<Document> Documents { get; set; }
}
