using HRMS.Data.Core;
using HRMS.Data.General;
using HRMS.Models;
using HRMS.Models.Staff;
using HRMS.Models.Staff.Department;
using HRMS.Models.Staff.Document;
using HRMS.Resources;
using HRMS.Utilities;
using HRMS.Utilities.General;
using HRMS.Utilities.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.Controllers;

public class StaffProfileController : BaseController
{
    private readonly IWebHostEnvironment environment;

    public StaffProfileController(IWebHostEnvironment environment,
        HRMSContext db, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        : base(db, signInManager, userManager)
    {
        this.environment = environment;
    }

    #region Main form

    [HttpGet, Authorize(Policy = "25:m")]
    [Description("Arb Tahiri", "Form to view the profile of staff.")]
    public async Task<IActionResult> Index(string ide)
    {
        var staffId = !string.IsNullOrEmpty(ide) ? CryptoSecurity.Decrypt<int>(ide) : await db.Staff.Where(a => a.UserId == user.Id).Select(a => a.StaffId).FirstOrDefaultAsync();

        var staffDetails = await db.Staff
            .Where(a => a.StaffId == staffId)
            .Select(a => new StaffDetails
            {
                Ide = CryptoSecurity.Encrypt(a.StaffId),
                ProfileImage = a.User.ProfileImage,
                Firstname = a.FirstName,
                Lastname = a.LastName,
                PhoneNumber = a.User.PhoneNumber,
                Email = a.User.Email,
                StaffType = string.Join(", ", a.StaffDepartment.Select(a => user.Language == LanguageEnum.Albanian ? a.StaffType.NameSq : a.StaffType.NameEn).ToList()),
                Department = string.Join(", ", a.StaffDepartment.Select(a => user.Language == LanguageEnum.Albanian ? a.Department.NameSq : a.Department.NameEn).ToList()),
                City = a.City,
                Country = user.Language == LanguageEnum.Albanian ? a.Country.NameSq : a.Country.NameEn,
                ZIP = a.PostalCode
            }).FirstOrDefaultAsync();

        var profile = new ProfileVM
        {
            StaffDetails = staffDetails,
            QualificationsCount = await db.StaffQualification.CountAsync(a => a.StaffId == staffId),
            DocumentsCount = await db.StaffDocument.CountAsync(a => a.StaffId == staffId),
            SubjectsCount = await db.StaffDepartmentSubject.CountAsync(a => a.StaffDepartment.StaffId == staffId)
        };
        return View(profile);
    }

    #endregion

    #region 1. Department

    [Authorize(Policy = "25:r"), Description("Arb Tahiri", "Form to display list of departments.")]
    public async Task<IActionResult> _ProfileDepartment(string ide)
    {
        var departments = await db.StaffDepartment
            .Include(a => a.Department).Include(a => a.StaffType)
            .Where(a => a.StaffId == CryptoSecurity.Decrypt<int>(ide))
            .Select(a => new Departments
            {
                StaffDepartmentIde = CryptoSecurity.Encrypt(a.StaffDepartmentId),
                Department = user.Language == LanguageEnum.Albanian ? $"{a.Department.Code} - {a.Department.NameSq}" : $"{a.Department.Code} - {a.Department.NameEn}",
                StaffType = user.Language == LanguageEnum.Albanian ? a.StaffType.NameSq : a.StaffType.NameEn,
                JobType = user.Language == LanguageEnum.Albanian ? a.JobType.NameSq : a.JobType.NameEn,
                BruttoSalary = $"{a.GrossSalary:#.##} €",
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                IsLecturer = a.StaffTypeId == (int)StaffTypeEnum.Lecturer
            }).ToListAsync();

        var departmentVM = new DepartmentVM
        {
            StaffIde = ide,
            Departments = departments
        };
        return PartialView(departmentVM);
    }

    #endregion

    #region 2. Qualification

    [Authorize(Policy = "25:r"), Description("Arb Tahiri", "Form to display list of qualifications.")]
    public async Task<IActionResult> _ProfileQualification(string ide)
    {
        var departments = await db.StaffQualification
            .Include(a => a.ProfessionType).Include(a => a.EducationLevelType)
            .Where(a => a.StaffId == CryptoSecurity.Decrypt<int>(ide))
            .Select(a => new Qualifications
            {
                StaffQualificationIde = CryptoSecurity.Encrypt(a.StaffQualificationId),
                ProfessionType = user.Language == LanguageEnum.Albanian ? $"{a.ProfessionType.Code} - {a.ProfessionType.NameSq}" : $"{a.ProfessionType.Code} - {a.ProfessionType.NameEn}",
                EducationLevel = user.Language == LanguageEnum.Albanian ? a.EducationLevelType.NameSq : a.EducationLevelType.NameEn,
                Title = a.Title,
                FieldOfStudy = a.FieldStudy,
                CreditNumber = a.CreditNumber,
                CreditType = a.CreditType
            }).ToListAsync();

        var qualificationVM = new QualificationVM
        {
            StaffIde = ide,
            Qualifications = departments
        };
        return PartialView(qualificationVM);
    }

    #endregion

    #region 3. Document

    [Authorize(Policy = "25:r"), Description("Arb Tahiri", "Form to display list of documents.")]
    public async Task<IActionResult> _ProfileDocument(string ide)
    {
        var documents = await db.StaffDocument
            .Include(a => a.DocumentType)
            .Where(a => a.StaffId == CryptoSecurity.Decrypt<int>(ide))
            .Select(a => new Documents
            {
                StaffDocumentIde = CryptoSecurity.Encrypt(a.StaffDocumentId),
                Title = a.Title,
                Path = a.Path,
                PathExtension = Path.GetExtension(a.Path),
                DocumentType = user.Language == LanguageEnum.Albanian ? a.DocumentType.NameSq : a.DocumentType.NameEn,
                Description = a.Description,
                Active = a.Active
            }).ToListAsync();

        var documentVM = new DocumentsVM
        {
            StaffIde = ide,
            Documents = documents,
        };
        return PartialView(documentVM);
    }

    #endregion

    #region 4. Subject

    [Authorize(Policy = "25:r"), Description("Arb Tahiri", "Form to display list of subjects.")]
    public async Task<IActionResult> _ProfileSubject(string ide)
    {
        var subjects = await db.StaffDepartmentSubject
            .Include(a => a.Subject)
            .Where(a => a.StaffDepartment.StaffId == CryptoSecurity.Decrypt<int>(ide))
            .Select(a => new Subjects
            {
                StaffDepartmentSubjectIde = CryptoSecurity.Encrypt(a.StaffDepartmentSubjectId),
                Subject = user.Language == LanguageEnum.Albanian ? a.Subject.NameSq : a.Subject.NameEn,
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                InsertDate = a.InsertedDate
            }).ToListAsync();
        return PartialView(subjects);
    }

    #endregion

    #region Change image

    [HttpPost, Authorize(Policy = "25:r"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Action to change profile photo.")]
    public async Task<IActionResult> ChangeImage(IFormFile Image, string ide)
    {
        if (string.IsNullOrEmpty(ide))
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        string userId = await db.Staff.Where(a => a.StaffId == CryptoSecurity.Decrypt<int>(ide)).Select(a => a.UserId).FirstOrDefaultAsync();
        var aspUser = await db.AspNetUsers.FirstOrDefaultAsync(a => a.Id == userId);

        string filePath = Image != null ? await SaveImage(environment, Image, "Users") : null;
        aspUser.ProfileImage = filePath;
        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataRegisteredSuccessfully, Icon = filePath });
    }

    #endregion
}
