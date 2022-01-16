namespace HRMS.Models.Configuration.Rules;

public class Rule
{
    public string Controller { get; set; }
    public string Method { get; set; }
    public string Description { get; set; }
    public string Policy { get; set; }
    public bool HasAccess { get; set; }
}
