using HRMS.Utilities;
using System;

namespace HRMS.Models.EvaluationManager;

public class Register
{
    public MethodType MethodType { get; set; }

    public string EvaluationIde { get; set; }
    public string EvaluationType { get; set; }
    public DateTime InsertedDate { get; set; }

    public int ManagerId { get; set; }
    public int StaffId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
}
