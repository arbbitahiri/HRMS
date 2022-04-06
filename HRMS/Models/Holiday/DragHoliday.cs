using System;

namespace HRMS.Models.Holiday;

public class DragHoliday
{
    public string HolidayIde { get; set; }
    public string HolidayTypeIde { get; set; }
    public DateTime Date { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
