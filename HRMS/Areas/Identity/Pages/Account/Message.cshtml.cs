using HRMS.Data.General;
using HRMS.Models;
using HRMS.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Areas.Identity.Pages.Account;
[AllowAnonymous]
public class MessageModel : BaseOModel
{
    public MessageModel(HRMSContext db) : base(db)
    {

    }

    [BindProperty]
    public ErrorVM Error { get; set; }

    public void OnGet(ErrorStatus status, string description)
    {
        Error = new ErrorVM { Status = status, Description = description };
    }

    public IActionResult OnPostFilter()
    {
        return Page();
    }
}
