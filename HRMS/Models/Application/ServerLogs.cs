using System;
using System.Diagnostics;

namespace HRMS.Models.Application;

public class ServerLogs
{
    public EventLogEntryType EventLogEntryType { get; set; }
    public string EntryType { get; set; }
    public string Machine { get; set; }
    public string Source { get; set; }
    public DateTime Time { get; set; }
    public string Username { get; set; }
    public string Message { get; set; }
}
