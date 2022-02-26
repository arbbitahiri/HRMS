using HRMS.Resources;
using HRMS.Utilities;
using System;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Areas.Evaluations.Models.StudentsStaff;

public class Register
{
    public MethodType MethodType { get; set; }

    public string EvaluationIde { get; set; }
    public string EvaluationType { get; set; }
    public DateTime InsertedDate { get; set; }

    [Display(Name = "NumberOfStudents", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    [Range(0, int.MaxValue, ErrorMessageResourceName = "GreaterThanOne", ErrorMessageResourceType = typeof(Resource))]
    public int NumberOfStudents { get; set; }

    [Display(Name = "Subject", ResourceType = typeof(Resource))]
    public int SubjectId { get; set; }

    [Display(Name = "Staff", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public int StaffDepartmentSubjectId { get; set; }

    [Display(Name = "Title", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string Title { get; set; }

    [Display(Name = "Description", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string Description { get; set; }
}
