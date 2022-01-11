using HRMS.Resources;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Models.Staff.Document;

public class EditDocument
{
    public string StaffDocumentIde { get; set; }

    [Display(Name = "DocumentType", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public int DocumentTypeId { get; set; }

    [Display(Name = "Title", ResourceType = typeof(Resource))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string Title { get; set; }

    [Display(Name = "Description", ResourceType = typeof(Resource))]
    public string Description { get; set; }

    [Display(Name = "Active", ResourceType = typeof(Resource))]
    public bool Active { get; set; }
}
