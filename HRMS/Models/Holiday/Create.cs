using HRMS.Resources;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Models.Holiday;

public class Create
{
    public string HolidayRequestIde { get; set; }

    [Display(Name = "HolidayType", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public int AHolidayTypeId { get; set; }

    [Display(Name = "StartDate", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string StartDate { get; set; }

    [Display(Name = "EndDate", ResourceType = typeof(Resource))]
    [Remote("CheckDate", "Holiday", AdditionalFields = "StartDate,HolidayTypeId", ErrorMessageResourceName = "MustBe18YearsOld", ErrorMessageResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string EndDate { get; set; }

    [Display(Name = "Description", ResourceType = typeof(Resource))]
    public string Description { get; set; }

    public int RemainingDays { get; set; }
}
