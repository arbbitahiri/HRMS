using HRMS.Resources;
using HRMS.Utilities;
using HRMS.Utilities.Validations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Models.EvaluationManager;

public class ManageQuestion
{
    public string EvaluationQuestionnaireNumericalIde { get; set; }
    public string EvaluationQuestionnaireOptionalIde { get; set; }
    public string EvaluationQuestionnaireTopicIde { get; set; }
    public string EvaluationIde { get; set; }

    [Display(Name = "QuestionType", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public int QuestionTypeId { get; set; }

    [Display(Name = "Question", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string Question { get; set; }

    [Display(Name = "Options", ResourceType = typeof(Resource))]
    public List<Option> Options { get; set; }

    public QuestionType QuestionType { get; set; }
}

public class Option
{
    public string OptionIde { get; set; }
    public string Title { get; set; }
}
