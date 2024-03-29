﻿using HRMS.Resources;
using System.ComponentModel.DataAnnotations;


namespace HRMS.Areas.Evaluations.Models.Self;

public class QuestionTopic
{
    public string EvaluationQuestionnaireTopicIde { get; set; }

    [Display(Name = "Question", ResourceType = typeof(Resource))]
    public string Question { get; set; }

    [Display(Name = "Answer", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string Answer { get; set; }

    [Display(Name = "InsertedDate", ResourceType = typeof(Resource))]
    public string InsertedDate { get; set; }
}
