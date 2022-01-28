using HRMS.Resources;
using HRMS.Utilities.Validations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Models.EvaluationManager;

public class ManageQuestion
{
    public string EvaluationQuestionnaireNumericalIde { get; set; }
    public string EvaluationIde { get; set; }

    [Display(Name = "QuestionType", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public int QuestionTypeId { get; set; }

    [Display(Name = "Question", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string Question { get; set; }

    [Display(Name = "Description", ResourceType = typeof(Resource))]
    [IfRequired(nameof(QuestionTypeId), "3", ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string Description { get; set; }

    [Display(Name = "Options", ResourceType = typeof(Resource))]
    public List<string> OptionsIds { get; set; }
    public bool Active { get; set; }
}
