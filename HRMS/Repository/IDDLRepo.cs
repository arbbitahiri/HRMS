using HRMS.Utilities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HRMS.Repository;
public interface IDDLRepo
{
    Task<List<SelectListItem>> Roles(LanguageEnum lang);
}
