using System;

namespace HRMS.Data.SqlFunctions;

public class Logs
{
    public int LogId { get; set; }
    public string Ip { get; set; }
    public string Controller { get; set; }
    public string Action { get; set; }
    public string Description { get; set; }
    public string Exception { get; set; }
    public string FormContent { get; set; }
    public string HttpMethod { get; set; }
    public string Username { get; set; }
    public DateTime InsertDate { get; set; }
}
