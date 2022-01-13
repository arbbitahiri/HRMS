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

    Task<List<SelectListItem>> Departments(LanguageEnum lang);

    Task<List<SelectListItem>> StaffTypes(LanguageEnum lang);

    Task<List<SelectListItem>> ProfessionTypes(LanguageEnum lang);

    Task<List<SelectListItem>> EducationLevelTypes(LanguageEnum lang);

    Task<List<SelectListItem>> DocumentTypes(LanguageEnum lang);

    Task<List<SelectListItem>> Subjects(LanguageEnum lang);

    Task<List<SelectListItem>> HolidayTypes(LanguageEnum lang);

    Task<List<SelectListItem>> StatusTypes(LanguageEnum lang);
}
