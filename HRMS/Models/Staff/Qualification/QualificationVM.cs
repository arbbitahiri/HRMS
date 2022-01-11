using HRMS.Utilities;
using System.Collections.Generic;

namespace HRMS.Models.Staff;

public class QualificationVM
{
    public string StaffIde { get; set; }

    public StaffDetails StaffDetails { get; set; }
    public List<Qualifications> Qualifications { get; set; }

    public int QualificationCount { get; set; }
    public MethodType MethodType { get; set; }
}
