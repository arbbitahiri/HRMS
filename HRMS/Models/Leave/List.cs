using HRMS.Utilities;
using System;

namespace HRMS.Models.Leave;

public class List
{
    public string LeaveIde { get; set; }

    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string ProfileImage { get; set; }
    public string PersonalNumber { get; set; }
    public string LeaveType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime ReturnDate { get; set; }
    public DateTime InsertedDate { get; set; }
    public string ReviewedDate { get; set; }

    public bool Finished { get; set; }
}
