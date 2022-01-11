using HRMS.Resources;
using HRMS.Utilities.Validations;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Models.Staff.Document;

public class AddDocument
{
    public string StaffIde { get; set; }

    [Display(Name = "DocumentType", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public int DocumentTypeId { get; set; }

    [Display(Name = "Title", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string Title { get; set; }

    [Display(Name = "Document", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    [FileExtension(".xls,.xlsx,.xlsm,.doc,.docm,.docx,.pdf,.pps,.ppsx,.ppt,.pptx", ErrorMessageResourceName = "AllowedFileFormats", ErrorMessageResourceType = typeof(Resource))]
    public IFormFile FormFile { get; set; }

    [Display(Name = "Description", ResourceType = typeof(Resource))]
    public string Description { get; set; }

    public string FileSize { get; set; }
}
