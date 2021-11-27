using HRMS.Utilities;

namespace HRMS.Models;
public class ErrorVM
{
    public string Title { get; set; }

    public ErrorStatus Status { get; set; }

    public string Description { get; set; }

    public string Icon { get; set; }

    public string RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
