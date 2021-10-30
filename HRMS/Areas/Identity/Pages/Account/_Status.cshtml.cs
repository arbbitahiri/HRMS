using HRMS.Data.General;
using HRMS.Models;
using HRMS.Utilities;

namespace HRMS.Areas.Identity.Pages.Account
{
    public class StatusModel : BaseOModel
    {
        public StatusModel(HRMSContext db) : base(db)
        {

        }

        public ErrorStatus ErrorNumber { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }

        public void OnGet(ErrorVM Error)
        {
            ErrorNumber = Error.Status;
            Description = Error.Description;
            Title = Error.Title;
        }
    }
}
