using HRMS.Resources;
using HRMS.Utilities.Validations;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Areas.Evaluations.Models.StudentsStaff;

public class ManageDocument
{
    public string EvaluationIde { get; set; }
    public string EvaluationDocumentIde { get; set; }

    [Display(Name = "DocumentType", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public int DocumentTypeId { get; set; }

    [Display(Name = "Title", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string Title { get; set; }

    [Display(Name = "Document", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    [FileExtension(".xls,.xlsx,.xlsm,.doc,.docm,.docx,.pdf,.pps,.ppsx,.ppt,.pptx", ErrorMessageResourceName = "AllowedFileFormats", ErrorMessageResourceType = typeof(Resource))]
    public IFormFile DocumentFile { get; set; }

    [Display(Name = "Description", ResourceType = typeof(Resource))]
    public string Description { get; set; }
    public bool Active { get; set; }

    public string FileSize { get; set; }
}
