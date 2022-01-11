using HRMS.Data.Core;
using HRMS.Data.General;
using HRMS.Models;
using HRMS.Resources;
using HRMS.Utilities;
using HRMS.Utilities.General;
using HRMS.Utilities.Validations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace HRMS.Areas.Identity.Pages.Account.Manage;
public partial class IndexModel : BaseIModel
{
    private readonly IWebHostEnvironment hostEnvironment;
    private readonly IConfiguration configuration;

    public IndexModel(IWebHostEnvironment hostEnvironment, IConfiguration configuration,
        UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, HRMS_WorkContext db)
        : base(signInManager, userManager, db)
    {
        this.hostEnvironment = hostEnvironment;
        this.configuration = configuration;
    }

    public string Username { get; set; }

    public string Email { get; set; }

    [TempData]
    public string StatusMessage { get; set; }

    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Phone]
        [Display(Name = "PhoneNumber", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        [FileExtension(".png,.jpg,.bmp", ErrorMessageResourceName = "AllowedFormatImage", ErrorMessageResourceType = typeof(Resource))]
        [MaxFileSize(10640, ErrorMessageResourceName = "MaxImageSize", ErrorMessageResourceType = typeof(Resource))]
        public IFormFile ProfileImage { get; set; }

        public string ImagePath { get; set; }
    }

    private void LoadAsync(ApplicationUser user)
    {
        Username = user.UserName;
        Email = user.Email;

        Input = new InputModel
        {
            PhoneNumber = user.PhoneNumber,
            ImagePath = user.ProfileImage,
            FirstName = user.FirstName,
            LastName = user.LastName
        };
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            TempData.Set("Error", new ErrorVM { Status = ErrorStatus.Error, Title = Resource.Error, Description = Resource.UnableToLoadUser });
            LoadAsync(user);
            return Page();
        }

        LoadAsync(user);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            TempData.Set("Error", new ErrorVM { Status = ErrorStatus.Error, Title = Resource.Error, Description = Resource.UnableToLoadUser });
            LoadAsync(user);
            return Page();
        }

        if (!ModelState.IsValid)
        {
            LoadAsync(user);
            return Page();
        }

        // TODO: send to history, before changes

        user.PhoneNumber = Input.PhoneNumber;
        if (Input.ProfileImage != null)
        {
            user.ProfileImage = await SaveFile(hostEnvironment, configuration, Input.ProfileImage, "Users");
        }

        await userManager.UpdateAsync(user);
        await signInManager.RefreshSignInAsync(user);

        TempData.Set("Error", new ErrorVM { Status = ErrorStatus.Success, Title = Resource.Success, Description = Resource.UpdatedProfile });
        return RedirectToPage();
    }
}
