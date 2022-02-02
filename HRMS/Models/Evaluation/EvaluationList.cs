using System;

namespace HRMS.Models.Evaluation;

public class EvaluationList
{
    public string EvaluationIde { get; set; }
    public string EvaluationManagerIde { get; set; }
    public string EvaluationStudentIde { get; set; }
    public string EvaluationSelfIde { get; set; }

    public string Title { get; set; }
    public string Description { get; set; }
    public string StatusType { get; set; }
    public int Questions { get; set; }
    public int Answers { get; set; }
    public DateTime InsertedDate { get; set; }

    public int Students { get; set; }
}
