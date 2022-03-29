using HRMS.Data.SqlFunctions;
using HRMS.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HRMS.Repository;

public interface IFunctionRepository
{
    Task<List<MenuList>> MenuList(string role, LanguageEnum lang);

    Task<List<MenuListAccess>> MenuListAccess(string role, LanguageEnum lang);

    Task<List<Logs>> Logs(string roleId, string userId, DateTime startDate, DateTime endDate, string ip, string controller, string action, string httpMethod, bool error);

    Task<List<SearchHome>> SearchHome(string parameter, LanguageEnum lang);
}
