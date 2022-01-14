using HRMS.Utilities;
using System;

namespace HRMS.Models.Holiday;

public class List
{
    public string HolidayRequestIde { get; set; }

    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string ProfileImage { get; set; }
    public string PersonalNumber { get; set; }
    public string HolidayType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public double TotalDays { get; set; }
    public string StatusType { get; set; }
    public StatusTypeEnum StatusTypeEnum { get; set; }
    public string Description { get; set; }

    public bool Finished { get; set; }
}
