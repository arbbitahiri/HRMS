using HRMS.Data.General;
using Microsoft.AspNetCore.Authorization;

namespace HRMS.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LockoutModel : BaseOModel
    {
        public LockoutModel(HRMSContext db) : base (db)
        {

        }

        public void OnGet()
        {

        }
    }
}
