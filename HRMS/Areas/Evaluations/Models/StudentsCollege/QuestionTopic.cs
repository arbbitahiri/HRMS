using HRMS.Resources;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Areas.Evaluations.Models.StudentsCollege;

public class QuestionTopic
{
    public string EvaluationQuestionnaireOptionalIde { get; set; }

    [Display(Name = "Question", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string Question { get; set; }
    public List<TopicAnswer> Answers { get; set; }
}

public class TopicAnswer
{
    public int TopicId { get; set; }
    public string TopicIde { get; set; }

    [Display(Name = "Answer", ResourceType = typeof(Resource))]
    public string Answer { get; set; }
}
