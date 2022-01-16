namespace HRMS.Models.Application;

public class LogSearch
{
    public string Role { get; set; }
    public string User { get; set; }
    public string Date { get; set; }
    public string Ip { get; set; }
    public string Controller { get; set; }
    public string Action { get; set; }
    public string HttpMethod { get; set; }
    public bool Error { get; set; }
}
