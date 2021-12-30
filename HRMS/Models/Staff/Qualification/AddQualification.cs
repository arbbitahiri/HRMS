using System;

namespace HRMS.Models.Staff.Qualification;

public class AddQualification
{
    public string StaffIde { get; set; }
    public string StaffQualificationIde { get; set; }

    public int ProfessionTypeId { get; set; }
    public int EducationLevelTypeId { get; set; }
    public bool Training { get; set; }
    public string Title { get; set; }
    public string FieldOfStudy { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public string Address { get; set; }
    public DateTime From { get; set; }
    public DateTime? To { get; set; }
    public bool OnGoing { get; set; }
    public string Description { get; set; }
    public decimal? FinalGrade { get; set; }
    public string Thesis { get; set; }
    public string CreditType { get; set; }
    public int? CreditNumber { get; set; }
    public DateTime? Validity { get; set; }
}
