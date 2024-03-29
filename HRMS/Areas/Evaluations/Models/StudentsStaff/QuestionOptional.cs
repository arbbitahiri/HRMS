﻿using System.Collections.Generic;

namespace HRMS.Areas.Evaluations.Models.StudentsStaff;

public class QuestionOptional
{
    public int OptionalId { get; set; }
    public string EvaluationQuestionnaireOptionalIde { get; set; }

    public string Question { get; set; }
    public string OtherDescription { get; set; }

    public List<QuestionOption> Options { get; set; }
}

public class QuestionOption
{
    public int OptionId { get; set; }
    public string OptionIde { get; set; }

    public string Option { get; set; }
    public bool Checked { get; set; }
}
