namespace HRMS.Models.Holiday;

public class ManageHoliday
{
    public string HolidayIde { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public int RepeatTypeId { get; set; }
    public string RepeatType { get; set; }
}
