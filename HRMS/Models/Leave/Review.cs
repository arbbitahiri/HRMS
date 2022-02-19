using HRMS.Resources;
using HRMS.Utilities;
using System;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Models.Leave;

public class Review
{
    public string LeaveIde { get; set; }
    public string StaffIde { get; set; }

    public string StaffName { get; set; }
    public string LeaveType { get; set; }
    public LeaveTypeEnum LeaveTypeEnum { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    [Display(Name = "StatusType", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public int StatusTypeId { get; set; }

    [Display(Name = "Description", ResourceType = typeof(Resource))]
    public string Description { get; set; }
}
