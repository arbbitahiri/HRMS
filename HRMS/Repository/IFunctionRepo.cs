using HRMS.Data.SqlFunctions;
using HRMS.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HRMS.Repository;
public interface IFunctionRepo
{
    Task<List<MenuList>> MenuList(string role, LanguageEnum lang);

    Task<List<MenuListAccess>> MenuListAccess(string role, LanguageEnum lang);
}
