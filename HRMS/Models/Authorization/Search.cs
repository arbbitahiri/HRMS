using System.ComponentModel.DataAnnotations;

namespace HRMS.Models.Authorization;
public class Search
{
    [Required]
    public string Role { get; set; }
}
