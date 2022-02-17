using HRMS.Resources;
using HRMS.Utilities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Areas.Evaluations.Models.Self;

public class ManageQuestion
{
    public string EvaluationQuestionnaireNumericalIde { get; set; }
    public string EvaluationQuestionnaireOptionalIde { get; set; }
    public string EvaluationQuestionnaireTopicIde { get; set; }
    public string EvaluationIde { get; set; }

    [Display(Name = "QuestionType", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public int QuestionTypeId { get; set; }

    [Display(Name = "QuestionSQ", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string QuestionSQ { get; set; }

    [Display(Name = "QuestionEN", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string QuestionEN { get; set; }

    [Display(Name = "Options", ResourceType = typeof(Resource))]
    //required if per null list
    public List<Option> Options { get; set; }

    public string Answer { get; set; }

    public int MaxQuestionOptions { get; set; }
    public QuestionType QuestionTypeEnum { get; set; }
}

public class Option
{
    public string OptionIde { get; set; }

    [Display(Name = "TitleSQ", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string TitleSQ { get; set; }

    [Display(Name = "TitleEN", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string TitleEN { get; set; }
}
