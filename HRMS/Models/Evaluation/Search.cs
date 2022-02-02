using HRMS.Resources;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Models.Evaluation;

public class Search
{
    [Display(Name = "EvaluationType", ResourceType = typeof(Resource))]
    public int? EvaluationTypeId { get; set; }
    [Display(Name = "StatusType", ResourceType = typeof(Resource))]
    public int? StatusTypeId { get; set; }
    [Display(Name = "Staff", ResourceType = typeof(Resource))]
    public int? StaffId { get; set; }
}
