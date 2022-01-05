using HRMS.Data.Core;
using HRMS.Data.General;
using HRMS.Models;
using HRMS.Models.Staff;
using HRMS.Models.Staff.Department;
using HRMS.Models.Staff.Document;
using HRMS.Models.Staff.Qualification;
using HRMS.Resources;
using HRMS.Utilities;
using HRMS.Utilities.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
    private readonly IWebHostEnvironment environment;
    private readonly IConfiguration configuration;

    public StaffController(IWebHostEnvironment environment, IConfiguration configuration,
        HRMSContext db, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        : base(db, signInManager, userManager)
    {
        this.environment = environment;
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

    #region 1. Register and edit

    [HttpGet, Description("Form to register or update staff. First step of registration/edition of staff.")]
    public async Task<IActionResult> Register(string ide)
    {
        if (string.IsNullOrEmpty(ide))
        {
            return View(new StaffPost() { MethodType = MethodType.POST });
        }
        else
        {
            var staff = await db.Staff.Include(a => a.User)
                .Where(a => a.PersonalNumber == CryptoSecurity.Decrypt<string>(ide))
                .Select(a => new StaffPost
                {
                    MethodType = MethodType.PUT,
                    StaffIde = CryptoSecurity.Encrypt(a.StaffId),
                    PersonalNumber = CryptoSecurity.Decrypt<string>(ide),
                    Firstname = a.FirstName,
                    Lastname = a.LastName,
                    BirthDate = a.Birthdate,
                    Gender = a.Gender,
                    Email = a.User.Email,
                    PhoneNumber = a.User.PhoneNumber,
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

        string staffIde = string.Empty;
        if (await db.AspNetUsers.AnyAsync(a => a.PersonalNumber == staff.PersonalNumber))
        {
            if (await db.Staff.AnyAsync(a => a.PersonalNumber == staff.PersonalNumber))
            {
                return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = "Stafi me kete numer personal ekziston!" });
            }

            try
            {
                var newStaff = new Staff
                {
                    UserId = await db.AspNetUsers.Where(a => a.PersonalNumber == staff.PersonalNumber).Select(a => a.Id).FirstOrDefaultAsync(),
                    PersonalNumber = staff.PersonalNumber,
                    FirstName = staff.Firstname,
                    LastName = staff.Lastname,
                    Birthdate = staff.BirthDate,
                    Gender = staff.Gender,
                    City = staff.City,
                    Country = staff.Country,
                    Address = staff.Address,
                    PostalCode = staff.PostalCode,
                    Nationality = staff.Nationality,
                    InsertedDate = DateTime.Now,
                    InsertedFrom = user.Id
                };

                staffIde = CryptoSecurity.Encrypt(newStaff.StaffId);
                db.Staff.Add(newStaff);
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
                Language = LanguageEnum.Albanian,
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
                    City = staff.City,
                    Country = staff.Country,
                    Address = staff.Address,
                    PostalCode = staff.PostalCode,
                    Nationality = staff.Nationality,
                    InsertedDate = DateTime.Now,
                    InsertedFrom = user.Id
                };

                staffIde = CryptoSecurity.Encrypt(newStaff.StaffId);
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

        return RedirectToAction(nameof(Qualification), new { ide = staffIde, mt = staff.MethodType });
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
        return RedirectToAction(nameof(Qualification), new { ide = edit.StaffIde, method = edit.MethodType });
        //return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataUpdatedSuccessfully });
    }

    #endregion

    #region 2. Qualifications

    #region => List

    [HttpGet, Description("Entry form for qualification. Second step of registration/edition of staff.")]
    public async Task<IActionResult> Qualification(string ide, MethodType method)
    {
        var staff = await db.Staff.Where(a => a.StaffId == CryptoSecurity.Decrypt<int>(ide))
            .Select(a => new StaffDetails
            {
                Ide = ide,
                Firstname = a.FirstName,
                Lastname = a.LastName
            }).FirstOrDefaultAsync();

        var qualifications = await db.StaffQualification.Where(a => a.StaffId == CryptoSecurity.Decrypt<int>(ide))
            .Select(a => new Qualifications
            {
                StaffQualificationIde = CryptoSecurity.Encrypt(a.StaffQualificationId),
                ProfessionType = user.Language == LanguageEnum.Albanian ? a.ProfessionType.NameSq : a.ProfessionType.NameEn,
                EducationLevel = user.Language == LanguageEnum.Albanian ? a.EducationLevelType.NameSq : a.EducationLevelType.NameEn,
                Title = a.Title,
                FieldOfStudy = a.FieldStudy,
                CreditNumber = a.CreditNumber,
            }).ToListAsync();

        var qualificationVM = new QualificationVM
        {
            StaffDetails = staff,
            Qualifications = qualifications,
            QualificationCount = qualifications.Count,
            MethodType = method
        };
        return View(qualifications);
    }

    #endregion

    #region => Create

    [HttpGet, Description("Form to add new qualification.")]
    public IActionResult _AddQualification(string ide) => PartialView(new AddQualification { StaffIde = ide });

    [HttpPost, ValidateAntiForgeryToken]
    [Description("Action to add qualification.")]
    public async Task<IActionResult> AddQualification(AddQualification add)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        db.Add(new StaffQualification
        {
            StaffId = CryptoSecurity.Decrypt<int>(add.StaffIde),
            ProfessionTypeId = add.ProfessionTypeId,
            EducationLevelTypeId = add.EducationLevelTypeId,
            Training = add.Training,
            Title = add.Title,
            FieldStudy = add.FieldOfStudy,
            City = add.City,
            Country = add.Country,
            Address = add.Address,
            From = add.From,
            To = add.To,
            OnGoing = add.OnGoing,
            Description = add.Description,
            FinalGrade = add.FinalGrade,
            Thesis = add.Thesis,
            CreditType = add.CreditType,
            CreditNumber = add.CreditNumber,
            Validity = add.Validity,
            InsertedDate = DateTime.Now,
            InsertedFrom = user.Id
        });
        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataRegisteredSuccessfully });
    }

    #endregion

    #region => Edit

    [HttpGet, Description("Form to edit staff data.")]
    public async Task<IActionResult> _EditQualification(string ide)
    {
        var qualification = await db.StaffQualification
            .Where(a => a.StaffQualificationId == CryptoSecurity.Decrypt<int>(ide))
            .Select(a => new AddQualification
            {
                StaffQualificationIde = ide,
                ProfessionTypeId = a.ProfessionTypeId,
                EducationLevelTypeId = a.EducationLevelTypeId,
                Training = a.Training,
                Title = a.Title,
                FieldOfStudy = a.FieldStudy,
                City = a.City,
                Country = a.Country,
                Address = a.Address,
                From = a.From,
                To = a.To,
                OnGoing = a.OnGoing,
                Description = a.Description,
                FinalGrade = a.FinalGrade,
                Thesis = a.Thesis,
                CreditType = a.CreditType,
                CreditNumber = a.CreditNumber,
                Validity = a.Validity
            }).FirstOrDefaultAsync();

        return PartialView(qualification);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Description("Action to edit a qualification.")]
    public async Task<IActionResult> EditQualification(AddQualification edit)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        var qualification = await db.StaffQualification.Where(a => a.StaffQualificationId == CryptoSecurity.Decrypt<int>(edit.StaffQualificationIde)).FirstOrDefaultAsync();
        qualification.ProfessionTypeId = edit.ProfessionTypeId;
        qualification.EducationLevelTypeId = edit.EducationLevelTypeId;
        qualification.Training = edit.Training;
        qualification.Title = edit.Title;
        qualification.FieldStudy = edit.FieldOfStudy;
        qualification.City = edit.City;
        qualification.Country = edit.Country;
        qualification.Address = edit.Address;
        qualification.From = edit.From;
        qualification.To = edit.To;
        qualification.OnGoing = edit.OnGoing;
        qualification.Description = edit.Description;
        qualification.FinalGrade = edit.FinalGrade;
        qualification.Thesis = edit.Thesis;
        qualification.CreditType = edit.CreditType;
        qualification.CreditNumber = edit.CreditNumber;
        qualification.Validity = edit.Validity;
        qualification.UpdatedDate = DateTime.Now;
        qualification.UpdatedFrom = user.Id;
        qualification.UpdatedNo++;

        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataUpdatedSuccessfully });
    }

    #endregion

    #region => Delete

    [HttpPost, ValidateAntiForgeryToken]
    [Description("Action to delete a qualification.")]
    public async Task<IActionResult> DeleteQualification(string ide)
    {
        var qualification = await db.StaffQualification.Where(a => a.StaffQualificationId == CryptoSecurity.Decrypt<int>(ide)).FirstOrDefaultAsync();
        db.Remove(qualification);
        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataDeletedSuccessfully });
    }

    #endregion

    #endregion

    #region 3. Documents

    #region => List

    [HttpGet, Description("Entry form for documents. Third step of registration/editation of staff.")]
    public async Task<IActionResult> Documents(string ide, MethodType method)
    {
        var staff = await db.Staff.Where(a => a.StaffId == CryptoSecurity.Decrypt<int>(ide))
            .Select(a => new StaffDetails
            {
                Ide = ide,
                Firstname = a.FirstName,
                Lastname = a.LastName
            }).FirstOrDefaultAsync();

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
                Description = a.Description
            }).ToListAsync();

        var documentVM = new DocumentsVM
        {
            StaffDetails = staff,
            Documents = documents,
            DocumentCount = documents.Count,
            MethodType = method
        };
        return View(documentVM);
    }

    #endregion

    #region => Create

    [HttpGet, Description("Form to add documents.")]
    public IActionResult _AddDocuments(string ide) => PartialView(new AddDocument { StaffIde = ide });

    [HttpPost, ValidateAntiForgeryToken]
    [Description("Action to add documents.")]
    public async Task<IActionResult> AddDocuments(AddDocument add)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        foreach (var file in add.FormFiles)
        {
            string path = await SaveFile(environment, configuration, file, "StaffDocuments", null);

            db.Add(new StaffDocument
            {
                StaffId = CryptoSecurity.Decrypt<int>(add.StaffIde),
                DocumentTypeId = add.DocumentTypeId,
                Title = add.Title,
                FileName = file.FileName,
                Path = path,
                Description = add.Description,
                Active = true,
                InsertedDate = DateTime.Now,
                InsertedFrom = user.Id
            });
        }

        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataRegisteredSuccessfully });
    }

    #endregion

    #region => Edit

    [HttpGet, Description("Form to edit a document.")]
    public async Task<IActionResult> _EditDocument(string ide)
    {
        var document = await db.StaffDocument
            .Where(a => a.StaffDocumentId == CryptoSecurity.Decrypt<int>(ide))
            .Select(a => new AddDocument
            {
                StaffDocumentIde = ide,
                DocumentTypeId = a.DocumentTypeId,
                Title = a.Title,
                Description = a.Description,
                Active = a.Active
            }).FirstOrDefaultAsync();

        return View(document);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Description("Action to edit a document.")]
    public async Task<IActionResult> EditDocument(AddDocument edit)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        var document = await db.StaffDocument.Where(a => a.StaffDocumentId == CryptoSecurity.Decrypt<int>(edit.StaffDocumentIde)).FirstOrDefaultAsync();
        document.DocumentTypeId = edit.DocumentTypeId;
        document.Title = edit.Title;
        document.Description = edit.Description;
        document.Active = edit.Active;
        document.UpdatedDate = DateTime.Now;
        document.UpdatedFrom = user.Id;
        document.UpdatedNo++;

        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataUpdatedSuccessfully });
    }

    #endregion

    #region => Delete

    [HttpPost, ValidateAntiForgeryToken]
    [Description("Action to delete a document.")]
    public async Task<IActionResult> DeleteDocument(string ide)
    {
        var document = await db.StaffDocument.Where(a => a.StaffDocumentId == CryptoSecurity.Decrypt<int>(ide)).FirstOrDefaultAsync();
        document.Active = false;
        document.UpdatedDate = DateTime.Now;
        document.UpdatedFrom = user.Id;
        document.UpdatedNo++;

        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataDeletedSuccessfully });
    }

    #endregion

    #endregion

    #region 4. Department && Subjects

    #region => List

    [HttpGet, Description("Entry form for department. Fourth step of registration/editation of staff.")]
    public async Task<IActionResult> Departments(string ide, MethodType method)
    {
        var staff = await db.Staff
            .Where(a => a.StaffId == CryptoSecurity.Decrypt<int>(ide))
            .Select(a => new StaffDetails
            {
                Ide = ide,
                Firstname = a.FirstName,
                Lastname = a.LastName,
                //IsProfessor = a.StaffCollege.Any(a => a.StaffTypeId == (int)StaffTypeEnum.Professor)
            }).FirstOrDefaultAsync();

        var departments = await db.StaffCollege
            .Include(a => a.Department).Include(a => a.StaffType)
            .Where(a => a.StaffId == CryptoSecurity.Decrypt<int>(ide))
            .Select(a => new Departments
            {
                StaffCollegeIde = CryptoSecurity.Encrypt(a.StaffCollegeId),
                Department = user.Language == LanguageEnum.Albanian ? a.Department.NameSq : a.Department.NameEn,
                StaffType = user.Language == LanguageEnum.Albanian ? a.StaffType.NameSq : a.StaffType.NameEn,
                StartDate = a.StartDate,
                EndDate = a.EndDate
            }).ToListAsync();

        var subjects = await db.StaffCollegeSubject
            .Include(a => a.Subject)
            .Where(a => a.StaffCollege.StaffId == CryptoSecurity.Decrypt<int>(ide))
            .Select(a => new Subjects
            {
                StaffCollegeSubjectIde = CryptoSecurity.Encrypt(a.StaffCollegeSubjectId),
                Subject = user.Language == LanguageEnum.Albanian ? a.Subject.NameSq : a.Subject.NameEn,
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                InsertDate = a.InsertedDate
            }).ToListAsync();

        var departmentVM = new DepartmentVM
        {
            StaffDetails = staff,
            Departments = departments,
            Subjects = subjects,
            DepartmentCount = departments.Count,
            SubjectCount = subjects.Count,
            MethodType = method
        };
        return View(departmentVM);
    }

    #endregion

    #region Department

    #region => Create

    [HttpGet, Description("Form to add department.")]
    public IActionResult _AddDepartment(string ide) => PartialView(new AddDepartment { StaffIde = ide });

    [HttpPost, ValidateAntiForgeryToken]
    [Description("Action to add new department.")]
    public async Task<IActionResult> AddDepartment(AddDepartment add)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        var getRole = GetRoleFromStaffType(add.StaffTypeId);
        var staffCollege = await db.StaffCollege.Include(a => a.Staff).Where(a => a.StaffId == CryptoSecurity.Decrypt<int>(add.StaffIde)).ToListAsync();

        foreach (var item in staffCollege)
        {
            if (await db.StaffCollege.AnyAsync(a => a.StaffId == item.StaffId && a.StaffTypeId == item.StaffTypeId && a.EndDate >= DateTime.Now))
            {
                var role = GetRoleFromStaffType(item.StaffTypeId);
                var staffRoleName = await db.AspNetRoles.Where(a => a.Id == role).Select(a => user.Language == LanguageEnum.Albanian ? a.NameSq : a.NameEn).FirstOrDefaultAsync();
                return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = string.Format(Resource.ExistsStaffRole, item.Staff.FirstName, item.Staff.LastName, staffRoleName) }); //$"{item.Staff.FirstName} {item.Staff.LastName} ekziston si {staffRoleName}. Duhet të pasivizoni këtë për të regjistruar pastaj!"
            }
        }

        var newStaffCollege = new StaffCollege
        {
            StaffId = CryptoSecurity.Decrypt<int>(add.StaffIde),
            StaffTypeId = add.StaffTypeId,
            DepartmentId = add.DepartmentId,
            StartDate = add.StartDate,
            EndDate = add.EndDate,
            Description = add.Description,
            InsertedDate = DateTime.Now,
            InsertedFrom = user.Id
        };

        var newUserId = await db.Staff.Where(a => a.StaffId == newStaffCollege.StaffId).Select(a => a.UserId).FirstOrDefaultAsync();
        if (!await db.AspNetUserRoles.AnyAsync(a => a.UserId == newUserId))
        {
            db.AspNetUserRoles.Add(new AspNetUserRoles
            {
                UserId = newUserId,
                RoleId = getRole
            });
        }

        // TODO: RealRoles

        db.StaffCollege.Add(newStaffCollege);
        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataRegisteredSuccessfully });
    }

    #endregion

    #region => Edit

    [HttpGet, Description("Form to edit department.")]
    public async Task<IActionResult> _EditDepartment(string ide)
    {
        var department = await db.StaffCollege
            .Where(a => a.StaffCollegeId == CryptoSecurity.Decrypt<int>(ide))
            .Select(a => new AddDepartment
            {
                StaffCollegeIde = ide,
                DepartmentId = a.DepartmentId,
                StaffTypeId = a.StaffTypeId,
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                Description = a.Description
            }).FirstOrDefaultAsync();

        return PartialView(department);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Description("Action to edit department.")]
    public async Task<IActionResult> EditDepartment(AddDepartment edit)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        var department = await db.StaffCollege.Where(a => a.StaffCollegeId == CryptoSecurity.Decrypt<int>(edit.StaffCollegeIde)).FirstOrDefaultAsync();
        department.DepartmentId = edit.DepartmentId;
        department.StaffTypeId = edit.StaffTypeId;
        department.StartDate = edit.StartDate;
        department.EndDate = edit.EndDate;
        department.Description = edit.Description;
        department.UpdatedDate = DateTime.Now;
        department.UpdatedFrom = user.Id;
        department.UpdatedNo++;

        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataUpdatedSuccessfully });
    }

    #endregion

    #region => Delete

    [HttpPost, ValidateAntiForgeryToken]
    [Description("Action to delete a department.")]
    public async Task<IActionResult> DeleteDepartment(string ide)
    {
        var department = await db.StaffCollege.Where(a => a.StaffCollegeId == CryptoSecurity.Decrypt<int>(ide)).FirstOrDefaultAsync();
        department.EndDate = DateTime.Now.AddDays(-1);
        department.UpdatedDate = DateTime.Now;
        department.UpdatedFrom = user.Id;
        department.UpdatedNo++;

        var subjects = await db.StaffCollegeSubject.Where(a => a.StaffCollegeId == CryptoSecurity.Decrypt<int>(ide) && a.EndDate >= DateTime.Now).ToListAsync();
        foreach (var subject in subjects)
        {
            subject.EndDate = DateTime.Now.AddDays(-1);
            subject.Active = false;
            subject.UpdatedDate = DateTime.Now;
            subject.UpdatedFrom = user.Id;
            subject.UpdatedNo++;
        }

        // TODO: check in RealRoles

        db.AspNetUserRoles.Remove(await db.AspNetUserRoles.Where(a => a.UserId == department.Staff.UserId).FirstOrDefaultAsync());

        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataDeletedSuccessfully });
    }

    #endregion

    #endregion

    #region Subject

    #region => Create

    [HttpGet, Description("Form to add subject.")]
    public IActionResult _AddSubject(string ide) => PartialView(new AddSubject { StaffCollegeIde = ide });

    [HttpPost, ValidateAntiForgeryToken]
    [Description("Action to add subject.")]
    public async Task<IActionResult> AddSubject(AddSubject add)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        db.StaffCollegeSubject.Add(new StaffCollegeSubject
        {
            StaffCollegeId = CryptoSecurity.Decrypt<int>(add.StaffCollegeIde),
            SubjectId = add.SubjectId,
            StartDate = add.StartDate,
            EndDate = add.EndDate,
            Active = true,
            InsertedDate = DateTime.Now,
            InsertedFrom = user.Id
        });

        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataRegisteredSuccessfully });
    }

    #endregion

    #region => Edit

    [HttpGet, Description("Form to edit subject.")]
    public async Task<IActionResult> _EditSubject(string ide)
    {
        var subject = await db.StaffCollegeSubject
            .Where(a => a.StaffCollegeSubjectId == CryptoSecurity.Decrypt<int>(ide))
            .Select(a => new AddSubject
            {
                StaffCollegeSubjectIde = ide,
                SubjectId = a.SubjectId,
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                Active = a.Active
            }).FirstOrDefaultAsync();

        return PartialView(subject);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Description("Action to edit subject.")]
    public async Task<IActionResult> EditSubject(AddSubject edit)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        var subject = await db.StaffCollegeSubject.Where(a => a.StaffCollegeSubjectId == CryptoSecurity.Decrypt<int>(edit.StaffCollegeSubjectIde)).FirstOrDefaultAsync();
        subject.SubjectId = edit.SubjectId;
        subject.StartDate = edit.StartDate;
        subject.EndDate = edit.EndDate;
        subject.Active = edit.Active;
        subject.UpdatedDate = DateTime.Now;
        subject.UpdatedFrom = user.Id;
        subject.UpdatedNo++;

        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataUpdatedSuccessfully });
    }

    #endregion

    #region => Delete

    [HttpPost, ValidateAntiForgeryToken]
    [Description("Action to delete a subject.")]
    public async Task<IActionResult> DeleteSubject(string ide)
    {
        var subject = await db.StaffCollegeSubject.Where(a => a.StaffCollegeSubjectId == CryptoSecurity.Decrypt<int>(ide)).FirstOrDefaultAsync();
        subject.EndDate = DateTime.Now.AddDays(-1);
        subject.UpdatedDate = DateTime.Now;
        subject.UpdatedFrom = user.Id;
        subject.UpdatedNo++;

        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataDeletedSuccessfully });
    }

    #endregion

    #endregion

    #endregion

    #region Profile

    [HttpGet, Description("Form to view the profile of staff.")]
    public async Task<IActionResult> Profile(string ide)
    {
        if (string.IsNullOrEmpty(ide))
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        var staffDetails = await db.Staff
            .Include(a => a.User)
            .Include(a => a.StaffCollege).ThenInclude(a => a.Department)
            .Where(a => a.StaffId == CryptoSecurity.Decrypt<int>(ide))
            .Select(a => new StaffDetails
            {
                ProfileImage = a.User.ProfileImage,
                Firstname = a.FirstName,
                Lastname = a.LastName,
                PhoneNumber = a.User.PhoneNumber,
                Email = a.User.Email,
                Department = string.Join(", ", a.StaffCollege.Select(a => user.Language == LanguageEnum.Albanian ? a.Department.NameSq : a.Department.NameEn).FirstOrDefault())
            }).FirstOrDefaultAsync();

        var qualifications = await db.StaffQualification
            .Include(a => a.ProfessionType).Include(a => a.EducationLevelType)
            .Where(a => a.StaffId == CryptoSecurity.Decrypt<int>(ide))
            .Select(a => new Qualifications
            {
                StaffQualificationIde = CryptoSecurity.Encrypt(a.StaffQualificationId),
                ProfessionType = user.Language == LanguageEnum.Albanian ? a.ProfessionType.NameSq : a.ProfessionType.NameEn,
                EducationLevel = user.Language == LanguageEnum.Albanian ? a.EducationLevelType.NameSq : a.EducationLevelType.NameEn,
                Title = a.Title,
                FieldOfStudy = a.FieldStudy,
                CreditNumber = a.CreditNumber
            }).ToListAsync();

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
                Description = a.Description
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

        var profile = new ProfileVM
        {
            StaffDetails = staffDetails,
            Qualifications = qualifications,
            Documents = documents,
            Subjects = subjects,
            QualificationsCount = qualifications.Count,
            DocumentsCount = documents.Count,
            SubjectsCount = subjects.Count
        };
        return View(profile);
    }

    #endregion

    #region Remote

    [Description("Method to check birthday.")]
    public IActionResult CheckBirthdate(DateTime BirthDate)
    {
        if (BirthDate >= DateTime.Now)
        {
            return Json("Nuk lejohet data me e madhe se sot!");
        }

        if (BirthDate <= DateTime.Now.AddYears(-18))
        {
            return Json(true);
        }
        else
        {
            return Json(false);
        }
    }

    #endregion
}
