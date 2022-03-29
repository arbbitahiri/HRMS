using HRMS.Data;
using HRMS.Data.SqlFunctions;
using HRMS.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HRMS.Repository;

public class FunctionRepo : IFunctionRepository
{
    private readonly ApplicationDbContext _db;

    public FunctionRepo(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<MenuList>> MenuList(string role, LanguageEnum lang) =>
        await _db.Set<MenuList>().FromSqlInterpolated(sql: $"SELECT * FROM [MenuList] ({role}, {lang})").ToListAsync();

    public async Task<List<MenuListAccess>> MenuListAccess(string role, LanguageEnum lang) =>
        await _db.Set<MenuListAccess>().FromSqlInterpolated(sql: $"SELECT * FROM [MenuListAccess] ({role}, {lang})").ToListAsync();

    public async Task<List<Logs>> Logs(string roleId, string userId, DateTime startDate, DateTime endDate, string ip, string controller, string action, string httpMethod, bool error) =>
        await _db.Set<Logs>().FromSqlInterpolated(sql: $"SELECT * FROM [Logs] ({roleId}, {userId}, {startDate}, {endDate}, {ip}, {controller}, {action}, {httpMethod}, {error})").ToListAsync();

    public async Task<List<SearchHome>> SearchHome(string parameter, LanguageEnum lang) =>
        await _db.Set<SearchHome>().FromSqlInterpolated(sql: $"SELECT * FROM [SearchHome] ({parameter}, {lang})").ToListAsync();
}
