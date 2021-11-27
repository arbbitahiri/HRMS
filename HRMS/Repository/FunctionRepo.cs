using HRMS.Data;
using HRMS.Data.SqlFunctions;
using HRMS.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HRMS.Repository;
public class FunctionRepo : IFunctionRepo
{
    private readonly ApplicationDbContext _db;

    public FunctionRepo(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<MenuList>> MenuList(string role, LanguageEnum lang) =>
        await _db.Set<MenuList>().FromSqlInterpolated(sql: $"SELECT * FROM MenuList ({role}, {lang})").ToListAsync();

    public async Task<List<MenuListAccess>> MenuListAccess(string role, LanguageEnum lang) =>
        await _db.Set<MenuListAccess>().FromSqlInterpolated(sql: $"SELECT * FROM MenuListAccess ({role}, {lang})").ToListAsync();
}
