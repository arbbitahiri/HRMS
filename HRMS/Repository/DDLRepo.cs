using HRMS.Data.General;
using HRMS.Resources;
using HRMS.Utilities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.Repository;
public class DDLRepo : IDDLRepo
{
    private readonly HRMSContext db;

    public DDLRepo(HRMSContext db)
    {
        this.db = db;
    }

    public List<SelectListItem> Languages() => new()
        {
            new SelectListItem { Text = Resource.Albanian, Value = LanguageEnum.Albanian.ToString() },
            new SelectListItem { Text = Resource.English, Value = LanguageEnum.English.ToString() }
        };

    public List<SelectListItem> Genders() => new()
        {
            new SelectListItem { Value = ((int)GenderEnum.Male).ToString(), Text = Resource.Male },
            new SelectListItem { Value = ((int)GenderEnum.Female).ToString(), Text = Resource.Female },
        };

    public async Task<List<SelectListItem>> Roles(LanguageEnum lang) =>
        await db.AspNetRoles.Select(a => new SelectListItem
        {
            Value = a.Id,
            Text = lang == LanguageEnum.Albanian ? a.NameSq : a.NameEn
        }).OrderBy(a => a.Text).ToListAsync();
}
