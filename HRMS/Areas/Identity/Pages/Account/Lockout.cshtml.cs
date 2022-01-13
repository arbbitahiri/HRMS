using HRMS.Data.General;
using Microsoft.AspNetCore.Authorization;

namespace HRMS.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LockoutModel : BaseOModel
    {
        public LockoutModel(HRMS_WorkContext db) : base (db)
        {

        }

        public void OnGet()
        {

        }
    }
}
