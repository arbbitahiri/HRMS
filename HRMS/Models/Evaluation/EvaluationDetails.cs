using HRMS.Utilities;
using System;

namespace HRMS.Models.Evaluation;

public class EvaluationDetails
{
    public string EvaluationIde { get; set; }

    public string EvaluationType { get; set; }
    public DateTime InsertedDate { get; set; }
    public MethodType MethodType { get; set; }
    public EvaluationTypeEnum EvaluationTypeEnum { get; set; }
    public string Manager { get; set; }
    public string Staff { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int Students { get; set; }
    public string Subject { get; set; }
    public int Questions { get; set; }
    public int Answers { get; set; }
}
