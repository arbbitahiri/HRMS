using HRMS.Resources;
using HRMS.Utilities;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Models.Tables;

public class CreateData
{
    public string Ide { get; set; }

    public LookUpTable Table { get; set; }
    public string Title { get; set; }

    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string NameSQ { get; set; }

    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
    public string NameEN { get; set; }

    public string DescriptionSQ { get; set; }
    public string DescriptionEN { get; set; }

    public string OtherData { get; set; }
    public int OtherDataId { get; set; }
}
