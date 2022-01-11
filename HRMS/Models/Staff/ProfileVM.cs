using System.Collections.Generic;

namespace HRMS.Models.Staff;

public class ProfileVM
{
    public StaffDetails StaffDetails { get; set; }
    //public List<Qualifications> Qualifications { get; set; }
    //public List<Subjects> Subjects { get; set; }
    //public List<Documents> Documents { get; set; }

    public int QualificationsCount { get; set; }
    public int SubjectsCount { get; set; }
    public int DocumentsCount { get; set; }
}
