using System;

namespace HRMS.Models.Application;

public class LogDetails
{
    public string LogIde { get; set; }
    public string Ip { get; set; }
    public string Controller { get; set; }
    public string Action { get; set; }
    public string Description { get; set; }
    public string Exception { get; set; }
    public string FormContent { get; set; }
    public DateTime InsertDate { get; set; }
    public string HttpMethod { get; set; }
    public string Username { get; set; }
}
