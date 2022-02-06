﻿using HRMS.Resources;
using HRMS.Utilities;
using System;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Models.EvaluationSelf;

public class Register
{
    public MethodType MethodType { get; set; }

    public string EvaluationIde { get; set; }
    public string EvaluationType { get; set; }
    public DateTime InsertedDate { get; set; }

    public string StaffName { get; set; }

    [Display(Name = "Title", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string Title { get; set; }

    [Display(Name = "Description", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string Description { get; set; }
}