using HRMS.Data.Core;
using HRMS.Data.General;
using HRMS.Models;
using HRMS.Models.Staff;
using HRMS.Resources;
using HRMS.Utilities;
using HRMS.Utilities.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.Controllers;
[Authorize]
public class StaffController : BaseController
{
    private readonly IConfiguration configuration;

    public StaffController(IConfiguration configuration, HRMSContext db, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        : base(db, signInManager, userManager)
    {
        this.configuration = configuration;
    }

    #region List

    [HttpGet, Description("Entry home. Search for staff.")]
    public IActionResult Index() => View();

    [HttpPost, ValidateAntiForgeryToken]
    [Description("Form to view searched list of staff.")]
    public async Task<IActionResult> Search(Search search)
    {
        var list = await db.Staff
            .Include(a => a.StaffCollege).ThenInclude(a => a.Department)
            .Include(a => a.User)
            .Where(a => a.StaffCollege.Any(b => b.DepartmentId == (search.Department ?? b.DepartmentId))
                && a.StaffCollege.Any(b => b.StaffTypeId == (search.Department ?? b.StaffTypeId))
                && (string.IsNullOrEmpty(search.PersonalNumber) || a.PersonalNumber.Contains(search.PersonalNumber))
                && (string.IsNullOrEmpty(search.Firstname) || a.FirstName.Contains(search.Firstname))
                && (string.IsNullOrEmpty(search.Lastname) || a.LastName.Contains(search.Lastname))
                && a.StaffCollege.Any(a => a.EndDate >= DateTime.Now))
            .AsSplitQuery()
            .Select(a => new StaffDetails
            {
                Ide = CryptoSecurity.Encrypt(a.PersonalNumber),
                Firstname = a.FirstName,
                Lastname = a.LastName,
                PersonalNumber = a.PersonalNumber,
                Gender = a.Gender == ((int)GenderEnum.Male) ? Resource.Male : Resource.Female,
                Department = user.Language == LanguageEnum.Albanian ? a.StaffCollege.Select(a => a.Department.NameSq).FirstOrDefault() : a.StaffCollege.Select(a => a.Department.NameEn).FirstOrDefault(),
                ProfileImage = a.User.ProfileImage,
                Email = a.User.Email,
                PhoneNumber = a.User.PhoneNumber,
                StaffType = string.Join(", ", a.StaffCollege.Select(a => a.StaffType).FirstOrDefault())
            }).ToListAsync();
        return Json(list);
    }

    #endregion

    #region Register and edit

    [HttpGet, Description("Form to register or update staff.")]
    public async Task<IActionResult> Register(string personalNumber)
    {
        if (string.IsNullOrEmpty(personalNumber))
        {
            return View(new StaffPost() { MethodType = MethodType.POST });
        }
        else
        {
            var staff = await db.Staff.Include(a => a.User)
                .Where(a => a.PersonalNumber == CryptoSecurity.Decrypt<string>(personalNumber))
                .Select(a => new StaffPost
                {
                    MethodType = MethodType.PUT,
                    StaffIde = CryptoSecurity.Encrypt(a.StaffId),
                    PersonalNumber = CryptoSecurity.Decrypt<string>(personalNumber),
                    Firstname = a.FirstName,
                    Lastname = a.LastName,
                    BirthDate = a.Birthdate,
                    Gender = a.Gender,
                    Email = a.User.Email,
                    PhoneNumber = a.User.PhoneNumber,
                    BirthPlace = a.BirthPlace,
                    City = a.City,
                    Country = a.Country,
                    Address = a.Address,
                    PostalCode = a.PostalCode,
                    Nationality = a.Nationality
                }).FirstOrDefaultAsync();
            return View(staff);
        }
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Description("Action to register staff.")]
    public async Task<IActionResult> Register(StaffPost staff)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        if (await db.AspNetUsers.AnyAsync(a => a.PersonalNumber == staff.PersonalNumber))
        {
            if (await db.Staff.AnyAsync(a => a.PersonalNumber == staff.PersonalNumber))
            {
                return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = "Stafi me kete numer personal ekziston!" });
            }

            try
            {
                db.Staff.Add(new Staff
                {
                    UserId = await db.AspNetUsers.Where(a => a.PersonalNumber == staff.PersonalNumber).Select(a => a.Id).FirstOrDefaultAsync(),
                    PersonalNumber = staff.PersonalNumber,
                    FirstName = staff.Firstname,
                    LastName = staff.Lastname,
                    Birthdate = staff.BirthDate,
                    Gender = staff.Gender,
                    BirthPlace = staff.BirthPlace,
                    City = staff.City,
                    Country = staff.Country,
                    Address = staff.Address,
                    PostalCode = staff.PostalCode,
                    Nationality = staff.Nationality,
                    InsertedDate = DateTime.Now,
                    InsertedFrom = user.Id
                });
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await LogError(ex);
                return Json(new ErrorVM { Status = ErrorStatus.Error, Description = "Ka ndodhur nje gabim gjate regjistrimit!" });
            }
        }
        else
        {
            var firstUser = new ApplicationUser
            {
                PersonalNumber = staff.PersonalNumber,
                FirstName = staff.Firstname,
                LastName = staff.Lastname,
                Birthdate = staff.BirthDate,
                Email = staff.Email,
                EmailConfirmed = true,
                PhoneNumber = staff.PhoneNumber,
                UserName = staff.Email,
                Language = staff.Language,
                Mode = TemplateMode.Light,
                InsertedDate = DateTime.Now,
                InsertedFrom = user.Id
            };

            string errors = string.Empty;
            var error = new ErrorVM { Status = ErrorStatus.Success, Description = Resource.AccountCreatedSuccessfully };

            string password = FirstTimePassword(configuration, staff.Firstname, staff.Lastname);
            var result = await userManager.CreateAsync(firstUser, password);
            if (!result.Succeeded)
            {
                foreach (var identityError in result.Errors)
                {
                    errors += $"{identityError.Description}. ";
                }
                error = new ErrorVM { Status = ErrorStatus.Warning, Description = errors };
            }

            try
            {
                var newStaff = new Staff
                {
                    UserId = firstUser.Id,
                    PersonalNumber = staff.PersonalNumber,
                    FirstName = staff.Firstname,
                    LastName = staff.Lastname,
                    Birthdate = staff.BirthDate,
                    Gender = staff.Gender,
                    BirthPlace = staff.BirthPlace,
                    City = staff.City,
                    Country = staff.Country,
                    Address = staff.Address,
                    PostalCode = staff.PostalCode,
                    Nationality = staff.Nationality,
                    InsertedDate = DateTime.Now,
                    InsertedFrom = user.Id
                };
                db.Staff.Add(newStaff);
                await db.SaveChangesAsync();
                // TODO: Send email to confirm email, if needed
            }
            catch (Exception ex)
            {
                await userManager.DeleteAsync(firstUser);
                await LogError(ex);
                return Json(new ErrorVM { Status = ErrorStatus.Error, Description = "Ka ndodhur nje gabim gjate regjistrimit!" });
            }
        }

        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataRegisteredSuccessfully });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Description("Action to edit staff data.")]
    public async Task<IActionResult> Edit(StaffPost edit)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        var staff = await db.Staff.Where(a => a.StaffId == CryptoSecurity.Decrypt<int>(edit.StaffIde)).FirstOrDefaultAsync();
        var userStaff = await db.AspNetUsers.FindAsync(staff.UserId);

        staff.PersonalNumber = edit.PersonalNumber;
        staff.FirstName = edit.Firstname;
        staff.LastName = edit.Lastname;
        staff.Birthdate = edit.BirthDate;
        staff.City = edit.City;
        staff.Country = edit.Country;
        staff.Address = edit.Address;
        staff.PostalCode = edit.PostalCode;
        staff.Nationality = edit.Nationality;
        userStaff.Email = edit.Email;
        userStaff.PhoneNumber = edit.PhoneNumber;
        staff.UpdatedDate = DateTime.Now;
        staff.UpdatedFrom = user.Id;
        staff.UpdatedNo++;

        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataUpdatedSuccessfully });
    }

    #endregion

    #region Profile

    [HttpGet, Description("Form to view the profile of staff.")]
    public async Task<IActionResult> Profile(string ide)
    {
        if (string.IsNullOrEmpty(ide))
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        var staffDetails = await db.Staff.Include(a => a.User)
            .Where(a => a.StaffId == CryptoSecurity.Decrypt<int>(ide))
            .Select(a => new StaffDetails
            {
                ProfileImage = a.User.ProfileImage,
                Firstname = a.FirstName,
                Lastname = a.LastName,
                PhoneNumber = a.User.PhoneNumber,
                Email = a.User.Email
            }).FirstOrDefaultAsync();

        var qualifications = await db.StaffQualification
            .Include(a => a.ProffessionType).Include(a => a.EducationLevelType)
            .Where(a => a.StaffId == CryptoSecurity.Decrypt<int>(ide))
            .Select(a => new Qualifications
            {
                StaffQualificationIde = CryptoSecurity.Encrypt(a.StaffQualificationId),
                ProfessionType = user.Language == LanguageEnum.Albanian ? a.ProffessionType.NameSq : a.ProffessionType.NameEn,
                EducationLevel = user.Language == LanguageEnum.Albanian ? a.EducationLevelType.NameSq : a.EducationLevelType.NameEn,
                Title = a.Title,
                FieldOfStudy = a.FieldStudy,
                City = a.City
            }).ToListAsync();

        var subjects = await db.StaffCollegeSubject
            .Include(a => a.Subject)
            .Include(a => a.StaffCollege).ThenInclude(a => a.Department)
            .Where(a => a.StaffCollege.StaffId == CryptoSecurity.Decrypt<int>(ide))
            .Select(a => new Subjects
            {
                StaffCollegeSubjectIde = CryptoSecurity.Encrypt(a.StaffCollegeSubjectId),
                Subject = user.Language == LanguageEnum.Albanian ? a.Subject.NameSq : a.Subject.NameEn,
                Department = user.Language == LanguageEnum.Albanian ? a.StaffCollege.Department.NameSq : a.StaffCollege.Department.NameEn,
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                InsertDate = a.InsertedDate
            }).ToListAsync();

        var documents = await db.StaffDocument
            .Include(a => a.Document).ThenInclude(a => a.DocumentType)
            .Where(a => a.StaffId == CryptoSecurity.Decrypt<int>(ide))
            .Select(a => new Documents
            {
                StaffDocumentIde = CryptoSecurity.Encrypt(a.StaffDocumentId),
                Title = a.Document.Title,
                Path = a.Document.Path,
                PathExtension = Path.GetExtension(a.Document.Path),
                DocumentType = user.Language == LanguageEnum.Albanian ? a.Document.DocumentType.NameSq : a.Document.DocumentType.NameEn,
                Description = a.Description
            }).ToListAsync();

        var profile = new ProfileVM
        {
            StaffDetails = staffDetails,
            Qualifications = qualifications,
            Subjects = subjects,
            Documents = documents,
            QualificationsCount = qualifications.Count,
            SubjectsCount = subjects.Count,
            DocumentsCount = documents.Count
        };
        return View(profile);
    }

    #endregion
}
