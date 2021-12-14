using HRMS.Utilities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HRMS.Repository;
public interface IDDLRepo
{
    List<SelectListItem> Languages();

    List<SelectListItem> Genders();

    Task<List<SelectListItem>> Roles(LanguageEnum lang);
}
