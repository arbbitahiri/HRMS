using HRMS.Resources;
using HRMS.Utilities;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Areas.Evaluations.Models.StudentsCollege;

public class ManageQuestion
{
    public string EvaluationIde { get; set; }
    public string EvaluationQuestionnaireNumericalIde { get; set; }
    public string EvaluationQuestionnaireOptionalIde { get; set; }

    [Display(Name = "QuestionType", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public int QuestionTypeId { get; set; }

    [Display(Name = "QuestionSQ", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string QuestionSQ { get; set; }

    [Display(Name = "QuestionEN", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string QuestionEN { get; set; }

    public string Answer { get; set; }

    public QuestionType QuestionTypeEnum { get; set; }
}
