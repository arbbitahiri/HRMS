using HRMS.Resources;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Models.Holiday;

public class Review
{
    public string StaffName { get; set; }
    public string HolidayType { get; set; }

    public string HolidayIde { get; set; }

    [Display(Name = "StatusType", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public int StatusTypeId { get; set; }

    [Display(Name = "Description", ResourceType = typeof(Resource))]
    public string Description { get; set; }
}
