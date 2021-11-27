using HRMS.Data.General;
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

    public async Task<List<SelectListItem>> Roles(LanguageEnum lang) =>
        await db.AspNetRoles.Select(a => new SelectListItem
        {
            Value = a.Id,
            Text = lang == LanguageEnum.Albanian ? a.NameSq : a.NameEn
        }).ToListAsync();
}
