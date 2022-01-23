using HRMS.Data.Core;
using HRMS.Data.General;
using HRMS.Models;
using HRMS.Models.Staff;
using HRMS.Models.Staff.Department;
using HRMS.Models.Staff.Document;
using HRMS.Models.Staff.Qualification;
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
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.Controllers;
[Authorize]
public class StaffController : BaseController
{
    private readonly IWebHostEnvironment environment;
    private readonly IConfiguration configuration;
    private readonly RoleManager<ApplicationRole> roleManager;

    public StaffController(IWebHostEnvironment environment, IConfiguration configuration, RoleManager<ApplicationRole> roleManager,
        HRMS_WorkContext db, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        : base(db, signInManager, userManager)
    {
        this.environment = environment;
        this.configuration = configuration;
        this.roleManager = roleManager;
    }

    #region 1. Register and edit

    [HttpGet, Authorize(Policy = "21s:c")]
    [Description("Arb Tahiri", "Form to register or update staff. First step of registration/edition of staff.")]
    public async Task<IActionResult> Register(string ide)
    {
        if (string.IsNullOrEmpty(ide))
        {
            return View(new StaffPost() { MethodType = MethodType.Post });
        }
        else
        {
            var staff = await db.Staff.Include(a => a.User)
                .Where(a => a.PersonalNumber == CryptoSecurity.Decrypt<string>(ide))
                .Select(a => new StaffPost
                {
                    MethodType = MethodType.Put,
                    StaffIde = CryptoSecurity.Encrypt(a.StaffId),
                    PersonalNumber = CryptoSecurity.Decrypt<string>(ide),
                    Firstname = a.FirstName,
                    Lastname = a.LastName,
                    BirthDate = a.Birthdate.ToString("dd/MM/yyyy"),
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

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "21s:c")]
    [Description("Arb Tahiri", "Action to register staff.")]
    public async Task<IActionResult> Register(StaffPost staff)
    {
        if (!ModelState.IsValid)
        {
            TempData.Set("Error", new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        string staffIde = string.Empty;
        if (await db.AspNetUsers.AnyAsync(a => a.PersonalNumber == staff.PersonalNumber))
        {
            if (await db.Staff.AnyAsync(a => a.PersonalNumber == staff.PersonalNumber))
            {
                TempData.Set("Error", new ErrorVM { Status = ErrorStatus.Warning, Title = Resource.Warning, Description = Resource.StaffWithPersonalExists });
            }

            try
            {
                var newStaff = new Staff
                {
                    UserId = await db.AspNetUsers.Where(a => a.PersonalNumber == staff.PersonalNumber).Select(a => a.Id).FirstOrDefaultAsync(),
                    PersonalNumber = staff.PersonalNumber,
                    FirstName = staff.Firstname,
                    LastName = staff.Lastname,
                    Birthdate = DateTime.ParseExact(staff.BirthDate, "dd/MM/yyyy", null),
                    Gender = staff.Gender,
                    City = staff.City,
                    Country = staff.Country,
                    Address = staff.Address,
                    PostalCode = staff.PostalCode,
                    Nationality = staff.Nationality,
                    InsertedDate = DateTime.Now,
                    InsertedFrom = user.Id
                };

                db.Staff.Add(newStaff);
                db.StaffRegistrationStatus.Add(new StaffRegistrationStatus
                {
                    StaffId = newStaff.StaffId,
                    StatusTypeId = (int)StatusTypeEnum.Processing,
                    InsertedDate = DateTime.Now,
                    InsertedFrom = user.Id
                });

                await db.SaveChangesAsync();

                staffIde = CryptoSecurity.Encrypt(newStaff.StaffId);
            }
            catch (Exception ex)
            {
                await LogError(ex);
                TempData.Set("Error", new ErrorVM { Status = ErrorStatus.Error, Title = Resource.Error, Description = Resource.ErrorProcessingData });
            }
        }
        else
        {
            var firstUser = new ApplicationUser
            {
                PersonalNumber = staff.PersonalNumber,
                FirstName = staff.Firstname,
                LastName = staff.Lastname,
                Birthdate = DateTime.ParseExact(staff.BirthDate, "dd/MM/yyyy", null),
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
                    Birthdate = DateTime.ParseExact(staff.BirthDate, "dd/MM/yyyy", null),
                    Gender = staff.Gender,
                    City = staff.City,
                    Country = staff.Country,
                    Address = staff.Address,
                    PostalCode = staff.PostalCode,
                    Nationality = staff.Nationality,
                    InsertedDate = DateTime.Now,
                    InsertedFrom = user.Id
                };

                db.Staff.Add(newStaff);
                db.StaffRegistrationStatus.Add(new StaffRegistrationStatus
                {
                    StaffId = newStaff.StaffId,
                    StatusTypeId = (int)StatusTypeEnum.Processing,
                    InsertedDate = DateTime.Now,
                    InsertedFrom = user.Id
                });

                await db.SaveChangesAsync();
                staffIde = CryptoSecurity.Encrypt(newStaff.StaffId);
            }
            catch (Exception ex)
            {
                await userManager.DeleteAsync(firstUser);
                await LogError(ex);
                TempData.Set("Error", new ErrorVM { Status = ErrorStatus.Error, Title = Resource.Error, Description = Resource.ErrorProcessingData });
            }
        }

        return RedirectToAction(nameof(Qualification), new { ide = staffIde, method = staff.MethodType });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "21s:c")]
    [Description("Arb Tahiri", "Action to edit staff data.")]
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
        staff.Birthdate = DateTime.ParseExact(edit.BirthDate, "dd/MM/yyyy", null);
        staff.City = edit.City;
        staff.Country = edit.Country;
        staff.Address = edit.Address;
        staff.PostalCode = edit.PostalCode;
        staff.Nationality = edit.Nationality;
        userStaff.Email = edit.Email;
        userStaff.PhoneNumber = edit.PhoneNumber;
        staff.UpdatedDate = DateTime.Now;
        staff.UpdatedFrom = user.Id;
        staff.UpdatedNo = staff.UpdatedNo.HasValue ? ++staff.UpdatedNo : staff.UpdatedNo = 1;

        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Qualification), new { ide = edit.StaffIde, method = edit.MethodType });
    }

    #endregion

    #region 2. Qualifications

    #region => List

    [HttpGet, Authorize(Policy = "21sq:r")]
    [Description("Arb Tahiri", "Entry form for qualification. Second step of registration/edition of staff.")]
    public async Task<IActionResult> Qualification(string ide, MethodType method)
    {
        var staff = await db.Staff.Where(a => a.StaffId == CryptoSecurity.Decrypt<int>(ide))
            .Select(a => new StaffDetails
            {
                Ide = ide,
                Firstname = a.FirstName,
                Lastname = a.LastName
            }).FirstOrDefaultAsync();

        var qualifications = await db.StaffQualification
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
            StaffDetails = staff,
            Qualifications = qualifications,
            QualificationCount = qualifications.Count,
            MethodType = method
        };
        return View(qualificationVM);
    }

    #endregion

    #region => Create

    [HttpGet, Authorize(Policy = "21sq:c")]
    [Description("Arb Tahiri", "Form to add new qualification.")]
    public IActionResult _AddQualification(string ide) => PartialView(new AddQualification { StaffIde = ide });

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "21sq:c")]
    [Description("Arb Tahiri", "Action to add qualification.")]
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
            From = DateTime.ParseExact(add.From, "dd/MM/yyyy", null),
            To = !string.IsNullOrEmpty(add.To) ? DateTime.ParseExact(add.To, "dd/MM/yyyy", null) : null,
            OnGoing = add.OnGoing,
            Description = add.Description,
            FinalGrade = add.FinalGrade,
            Thesis = add.Thesis,
            CreditType = add.CreditType,
            CreditNumber = add.CreditNumber,
            Validity = !string.IsNullOrEmpty(add.Validity) ? DateTime.ParseExact(add.Validity, "dd/MM/yyyy", null) : null,
            InsertedDate = DateTime.Now,
            InsertedFrom = user.Id
        });
        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataRegisteredSuccessfully });
    }

    #endregion

    #region => Edit

    [HttpGet, Authorize(Policy = "21sq:e")]
    [Description("Arb Tahiri", "Form to edit staff data.")]
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
                From = a.From.ToString("dd/MM/yyyy"),
                To = a.To.HasValue ? a.To.Value.ToString("dd/MM/yyyy") : "",
                OnGoing = a.OnGoing,
                Description = a.Description,
                FinalGrade = a.FinalGrade,
                Thesis = a.Thesis,
                CreditType = a.CreditType,
                CreditNumber = a.CreditNumber,
                Validity = a.Validity.HasValue ? a.Validity.Value.ToString("dd/MM/yyyy") : ""
            }).FirstOrDefaultAsync();

        return PartialView(qualification);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "21sq:e")]
    [Description("Arb Tahiri", "Action to edit a qualification.")]
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
        qualification.From = DateTime.ParseExact(edit.From, "dd/MM/yyyy", null);
        qualification.To = !string.IsNullOrEmpty(edit.To) ? DateTime.ParseExact(edit.To, "dd/MM/yyyy", null) : null;
        qualification.OnGoing = edit.OnGoing;
        qualification.Description = edit.Description;
        qualification.FinalGrade = edit.FinalGrade;
        qualification.Thesis = edit.Thesis;
        qualification.CreditType = edit.CreditType;
        qualification.CreditNumber = edit.CreditNumber;
        qualification.Validity = !string.IsNullOrEmpty(edit.To) ? DateTime.ParseExact(edit.Validity, "dd/MM/yyyy", null) : null;
        qualification.UpdatedDate = DateTime.Now;
        qualification.UpdatedFrom = user.Id;
        qualification.UpdatedNo = qualification.UpdatedNo.HasValue ? ++qualification.UpdatedNo : qualification.UpdatedNo = 1;

        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataUpdatedSuccessfully });
    }

    #endregion

    #region => Details

    [HttpGet, Authorize(Policy = "21sq:r"), Description("Arb Tahiri", "Form to display qualification's details.")]
    public async Task<IActionResult> _DetailsQualification(string ide)
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
                FieldOfStudy = a.FieldStudy ?? "///",
                City = a.City,
                Country = a.Country,
                Address = a.Address,
                From = a.From.ToString("dd/MM/yyyy"),
                To = a.To.HasValue ? a.To.Value.ToString("dd/MM/yyyy") : "///",
                OnGoing = a.OnGoing,
                Description = a.Description ?? "///",
                FinalGrade = a.FinalGrade,
                Thesis = a.Thesis ?? "///",
                CreditType = a.CreditType ?? "///",
                CreditNumber = a.CreditNumber,
                Validity = a.Validity.HasValue ? a.Validity.Value.ToString("dd/MM/yyyy") : "///"
            }).FirstOrDefaultAsync();

        return PartialView(qualification);
    }

    #endregion

    #region => Delete

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "21sq:d")]
    [Description("Arb Tahiri", "Action to delete a qualification.")]
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

    [HttpGet, Authorize(Policy = "21sd:r")]
    [Description("Arb Tahiri", "Entry form for documents. Third step of registration/editation of staff.")]
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
                Description = a.Description,
                Active = a.Active
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

    [HttpGet, Authorize(Policy = "21sd:c")]
    [Description("Arb Tahiri", "Form to add documents.")]
    public IActionResult _AddDocument(string ide) => PartialView(new AddDocument { StaffIde = ide, FileSize = $"{Convert.ToDecimal(configuration["AppSettings:FileMaxKB"]) / 1024:N1}MB" });

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "21sd:c")]
    [Description("Arb Tahiri", "Action to add documents.")]
    public async Task<IActionResult> AddDocument(AddDocument add)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        string path = await SaveFile(environment, configuration, add.FormFile, "StaffDocuments", null);

        db.Add(new StaffDocument
        {
            StaffId = CryptoSecurity.Decrypt<int>(add.StaffIde),
            DocumentTypeId = add.DocumentTypeId,
            Title = add.Title,
            Path = path,
            Description = add.Description,
            Active = true,
            InsertedDate = DateTime.Now,
            InsertedFrom = user.Id
        });

        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataRegisteredSuccessfully });
    }

    #endregion

    #region => Edit

    [HttpGet, Authorize(Policy = "21sd:e")]
    [Description("Arb Tahiri", "Form to edit a document.")]
    public async Task<IActionResult> _EditDocument(string ide)
    {
        var document = await db.StaffDocument
            .Where(a => a.StaffDocumentId == CryptoSecurity.Decrypt<int>(ide))
            .Select(a => new EditDocument
            {
                StaffDocumentIde = ide,
                DocumentTypeId = a.DocumentTypeId,
                Title = a.Title,
                Description = a.Description,
                Active = a.Active
            }).FirstOrDefaultAsync();

        return View(document);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "21sd:e")]
    [Description("Arb Tahiri", "Action to edit a document.")]
    public async Task<IActionResult> EditDocument(EditDocument edit)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        var document = await db.StaffDocument.FirstOrDefaultAsync(a => a.StaffDocumentId == CryptoSecurity.Decrypt<int>(edit.StaffDocumentIde));
        document.DocumentTypeId = edit.DocumentTypeId;
        document.Title = edit.Title;
        document.Description = edit.Description;
        document.Active = edit.Active;
        document.UpdatedDate = DateTime.Now;
        document.UpdatedFrom = user.Id;
        document.UpdatedNo = document.UpdatedNo.HasValue ? ++document.UpdatedNo : document.UpdatedNo = 1;

        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataUpdatedSuccessfully });
    }

    #endregion

    #region => Delete

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "21sd:d")]
    [Description("Arb Tahiri", "Action to delete a document.")]
    public async Task<IActionResult> DeleteDocument(string ide)
    {
        var document = await db.StaffDocument.FirstOrDefaultAsync(a => a.StaffDocumentId == CryptoSecurity.Decrypt<int>(ide));
        document.Active = false;
        document.UpdatedDate = DateTime.Now;
        document.UpdatedFrom = user.Id;
        document.UpdatedNo = document.UpdatedNo.HasValue ? ++document.UpdatedNo : document.UpdatedNo = 1;

        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataDeletedSuccessfully });
    }

    #endregion

    #endregion

    #region 4. Department && Subjects

    #region => List

    [HttpGet, Authorize(Policy = "21sds:r")]
    [Description("Arb Tahiri", "Entry form for department. Fourth step of registration/editation of staff.")]
    public async Task<IActionResult> Departments(string ide, MethodType method)
    {
        var staff = await db.Staff
            .Where(a => a.StaffId == CryptoSecurity.Decrypt<int>(ide))
            .Select(a => new StaffDetails
            {
                Ide = ide,
                Firstname = a.FirstName,
                Lastname = a.LastName,
                IsProfessor = a.StaffDepartment.Any(a => a.StaffTypeId == (int)StaffTypeEnum.Lecturer)
            }).FirstOrDefaultAsync();

        var departments = await db.StaffDepartment
            .Include(a => a.Department).Include(a => a.StaffType)
            .Where(a => a.StaffId == CryptoSecurity.Decrypt<int>(ide))
            .Select(a => new Departments
            {
                StaffDepartmentIde = CryptoSecurity.Encrypt(a.StaffDepartmentId),
                Department = user.Language == LanguageEnum.Albanian ? $"{a.Department.Code} - {a.Department.NameSq}" : $"{a.Department.Code} - {a.Department.NameEn}",
                StaffType = user.Language == LanguageEnum.Albanian ? a.StaffType.NameSq : a.StaffType.NameEn,
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                Description = a.Description
            }).ToListAsync();

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

    #region => Department

    #region ==> Create

    [HttpGet, Authorize(Policy = "21sdp:c")]
    [Description("Arb Tahiri", "Form to add department.")]
    public async Task<IActionResult> _AddDepartment(string ide)
    {
        var staffDetails = await db.Staff.Where(a => a.StaffId == CryptoSecurity.Decrypt<int>(ide))
            .Select(a => new AddDepartment
            {
                StaffIde = ide,
                Outsider = a.Country.ToLower().Contains("kosov")
            }).FirstOrDefaultAsync();
        return PartialView(staffDetails);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "21sdp:c")]
    [Description("Arb Tahiri", "Action to add new department.")]
    public async Task<IActionResult> AddDepartment(AddDepartment add)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        var getRole = GetRoleFromStaffType(add.StaffTypeId);
        var staffDepartment = await db.StaffDepartment.Include(a => a.Staff).Where(a => a.StaffId == CryptoSecurity.Decrypt<int>(add.StaffIde)).ToListAsync();

        foreach (var item in staffDepartment)
        {
            if (await db.StaffDepartment.AnyAsync(a => a.StaffId == item.StaffId && a.StaffTypeId == item.StaffTypeId && a.EndDate >= DateTime.Now))
            {
                var role = GetRoleFromStaffType(item.StaffTypeId);
                var staffRoleName = await db.AspNetRoles.Where(a => a.Id == role).Select(a => user.Language == LanguageEnum.Albanian ? a.NameSq : a.NameEn).FirstOrDefaultAsync();
                return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = string.Format(Resource.ExistsStaffRole, item.Staff.FirstName, item.Staff.LastName, staffRoleName) }); //$"{item.Staff.FirstName} {item.Staff.LastName} ekziston si {staffRoleName}. Duhet të pasivizoni këtë për të regjistruar pastaj!"
            }
        }

        var userId = await db.Staff.Where(a => a.StaffId == CryptoSecurity.Decrypt<int>(add.StaffIde)).Select(a => a.UserId).FirstOrDefaultAsync();
        var newUser = await userManager.FindByIdAsync(userId);
        var getRoles = await userManager.GetRolesAsync(newUser);

        var newStaffDepartment = new StaffDepartment
        {
            StaffId = CryptoSecurity.Decrypt<int>(add.StaffIde),
            StaffTypeId = add.StaffTypeId,
            DepartmentId = add.DepartmentId,
            StartDate = DateTime.ParseExact(add.StartDate, "dd/MM/yyyy", null),
            EndDate = DateTime.ParseExact(add.EndDate, "dd/MM/yyyy", null),
            BruttoSalary = add.Salary,
            EmployeeContribution = add.EmployeeContribution,
            EmployerContribution = add.EmployerContribution,
            Description = add.Description,
            InsertedDate = DateTime.Now,
            InsertedFrom = user.Id
        };

        if (!getRoles.Any())
        {
            var result = await userManager.AddToRolesAsync(newUser, db.AspNetRoles.Where(a => getRoles.Contains(a.Id)).Select(a => a.Name).ToList());
            if (!result.Succeeded)
            {
                TempData.Set("Error", new ErrorVM { Status = ErrorStatus.Error, Title = Resource.Error, RawContent = true, Description = "<ul>" + string.Join("", result.Errors.Select(a => "<li>" + a.Description + "</li>").ToArray()) + $"<li>{Resource.RolesAddThroughList}</li>" + "</ul>" });
            }
        }

        if (!getRoles.Any(a => a == getRole))
        {
            db.RealRole.Add(new RealRole
            {
                UserId = userId,
                RoleId = getRole,
                InsertedDate = DateTime.Now,
                InsertedFrom = user.Id
            });
        }

        db.StaffDepartment.Add(newStaffDepartment);
        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataRegisteredSuccessfully });
    }

    #endregion

    #region ==> Edit

    [HttpGet, Authorize(Policy = "21sdp:e")]
    [Description("Arb Tahiri", "Form to edit department.")]
    public async Task<IActionResult> _EditDepartment(string ide)
    {
        var department = await db.StaffDepartment
            .Where(a => a.StaffDepartmentId == CryptoSecurity.Decrypt<int>(ide))
            .Select(a => new AddDepartment
            {
                StaffDepartmentIde = ide,
                DepartmentId = a.DepartmentId,
                StaffTypeId = a.StaffTypeId,
                StartDate = a.StartDate.ToString("dd/MM/yyyy"),
                EndDate = a.EndDate.ToString("dd/MM/yyyy"),
                Salary = a.BruttoSalary,
                EmployeeContribution = a.EmployeeContribution,
                EmployerContribution = a.EmployerContribution,
                Description = a.Description,
                Outsider = a.Staff.Country.ToLower().Contains("kosov")
            }).FirstOrDefaultAsync();

        return PartialView(department);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "21sdp:e")]
    [Description("Arb Tahiri", "Action to edit department.")]
    public async Task<IActionResult> EditDepartment(AddDepartment edit)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        var department = await db.StaffDepartment.Where(a => a.StaffDepartmentId == CryptoSecurity.Decrypt<int>(edit.StaffDepartmentIde)).FirstOrDefaultAsync();
        department.DepartmentId = edit.DepartmentId;
        department.StaffTypeId = edit.StaffTypeId;
        department.StartDate = DateTime.ParseExact(edit.StartDate, "dd/MM/yyyy", null);
        department.EndDate = DateTime.ParseExact(edit.EndDate, "dd/MM/yyyy", null);
        department.BruttoSalary = edit.Salary;
        department.EmployeeContribution = edit.EmployeeContribution;
        department.EmployerContribution = edit.EmployerContribution;
        department.UpdatedDate = DateTime.Now;
        department.UpdatedFrom = user.Id;
        department.UpdatedNo = department.UpdatedNo.HasValue ? ++department.UpdatedNo : department.UpdatedNo = 1;

        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataUpdatedSuccessfully });
    }

    #endregion

    #region ==> Delete

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "21sdp:d")]
    [Description("Arb Tahiri", "Action to delete a department.")]
    public async Task<IActionResult> DeleteDepartment(string ide)
    {
        var department = await db.StaffDepartment.Include(a => a.Staff).FirstOrDefaultAsync(a => a.StaffDepartmentId == CryptoSecurity.Decrypt<int>(ide));
        department.EndDate = DateTime.Now.AddDays(-1);
        department.UpdatedDate = DateTime.Now;
        department.UpdatedFrom = user.Id;
        department.UpdatedNo = department.UpdatedNo.HasValue ? ++department.UpdatedNo : department.UpdatedNo = 1;

        var subjects = await db.StaffDepartmentSubject.Where(a => a.StaffDepartmentId == CryptoSecurity.Decrypt<int>(ide) && a.EndDate >= DateTime.Now).ToListAsync();
        foreach (var subject in subjects)
        {
            subject.EndDate = DateTime.Now.AddDays(-1);
            subject.Active = false;
            subject.UpdatedDate = DateTime.Now;
            subject.UpdatedFrom = user.Id;
            subject.UpdatedNo = department.UpdatedNo.HasValue ? ++department.UpdatedNo : department.UpdatedNo = 1;
        }

        var userToRemove = await userManager.FindByIdAsync(department.Staff.UserId);
        var rolesToRemove = await userManager.GetRolesAsync(userToRemove);

        var realRoles = await db.RealRole.FirstOrDefaultAsync(a => a.UserId == department.Staff.UserId);
        if (realRoles != null)
        {
            db.RealRole.Remove(realRoles);
            await db.SaveChangesAsync();

            var anotherRealRoles = await db.RealRole.Where(a => a.UserId == department.Staff.UserId).ToListAsync();
            if (anotherRealRoles.Count == 0)
            {
                var result = await userManager.RemoveFromRolesAsync(userToRemove, db.AspNetRoles.Where(a => rolesToRemove.Contains(a.Id)).Select(a => a.Name).ToList());
                if (!result.Succeeded)
                {
                    TempData.Set("Error", new ErrorVM { Status = ErrorStatus.Error, Title = Resource.Error, RawContent = true, Description = "<ul>" + string.Join("", result.Errors.Select(a => "<li>" + a.Description + "</li>").ToArray()) + "</ul>" });
                }
            }
            else
            {
                var result = await userManager.AddToRoleAsync(userToRemove, db.AspNetRoles.Where(a => a.Id == anotherRealRoles.Select(a => a.RoleId).FirstOrDefault()).Select(a => a.Name).FirstOrDefault());
                if (!result.Succeeded)
                {
                    TempData.Set("Error", new ErrorVM { Status = ErrorStatus.Error, Title = Resource.Error, RawContent = true, Description = "<ul>" + string.Join("", result.Errors.Select(a => "<li>" + a.Description + "</li>").ToArray()) + "</ul>" });
                }
            }
        }

        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataDeletedSuccessfully });
    }

    #endregion

    #endregion

    #region => Subject

    #region ==> Create

    [HttpGet, Authorize(Policy = "21ssb:c")]
    [Description("Arb Tahiri", "Form to add subject.")]
    public async Task<IActionResult> _AddSubject(string ide) => PartialView(new AddSubject { StaffDepartmentIde = ide, DepartmentEndDate = await db.StaffDepartment.Where(a => a.StaffDepartmentId == CryptoSecurity.Decrypt<int>(ide)).Select(a => a.EndDate).FirstOrDefaultAsync() });

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "21ssb:c")]
    [Description("Arb Tahiri", "Action to add subject.")]
    public async Task<IActionResult> AddSubject(AddSubject add)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        db.StaffDepartmentSubject.Add(new StaffDepartmentSubject
        {
            StaffDepartmentId = CryptoSecurity.Decrypt<int>(add.StaffDepartmentIde),
            SubjectId = add.SubjectId,
            StartDate = DateTime.ParseExact(add.StartDate, "dd/MM/yyyy", null),
            EndDate = DateTime.ParseExact(add.EndDate, "dd/MM/yyyy", null),
            Active = true,
            InsertedDate = DateTime.Now,
            InsertedFrom = user.Id
        });

        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataRegisteredSuccessfully });
    }

    #endregion

    #region ==> Edit

    [HttpGet, Authorize(Policy = "21ssb:e")]
    [Description("Arb Tahiri", "Form to edit subject.")]
    public async Task<IActionResult> _EditSubject(string ide)
    {
        var subject = await db.StaffDepartmentSubject
            .Where(a => a.StaffDepartmentSubjectId == CryptoSecurity.Decrypt<int>(ide))
            .Select(a => new AddSubject
            {
                StaffDepartmentSubjectIde = ide,
                SubjectId = a.SubjectId,
                StartDate = a.StartDate.ToString("dd/MM/yyyy"),
                EndDate = a.EndDate.ToString("dd/MM/yyyy"),
                Active = a.Active
            }).FirstOrDefaultAsync();

        return PartialView(subject);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "21ssb:e")]
    [Description("Arb Tahiri", "Action to edit subject.")]
    public async Task<IActionResult> EditSubject(AddSubject edit)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        var subject = await db.StaffDepartmentSubject.FirstOrDefaultAsync(a => a.StaffDepartmentSubjectId == CryptoSecurity.Decrypt<int>(edit.StaffDepartmentSubjectIde));
        subject.SubjectId = edit.SubjectId;
        subject.StartDate = DateTime.ParseExact(edit.StartDate, "dd/MM/yyyy", null);
        subject.EndDate = DateTime.ParseExact(edit.EndDate, "dd/MM/yyyy", null);
        subject.Active = edit.Active;
        subject.UpdatedDate = DateTime.Now;
        subject.UpdatedFrom = user.Id;
        subject.UpdatedNo = subject.UpdatedNo.HasValue ? ++subject.UpdatedNo : subject.UpdatedNo = 1;

        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataUpdatedSuccessfully });
    }

    #endregion

    #region ==> Delete

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "21ssb:d")]
    [Description("Arb Tahiri", "Action to delete a subject.")]
    public async Task<IActionResult> DeleteSubject(string ide)
    {
        var subject = await db.StaffDepartmentSubject.FirstOrDefaultAsync(a => a.StaffDepartmentSubjectId == CryptoSecurity.Decrypt<int>(ide));
        subject.EndDate = DateTime.Now.AddDays(-1);
        subject.UpdatedDate = DateTime.Now;
        subject.UpdatedFrom = user.Id;
        subject.UpdatedNo = subject.UpdatedNo.HasValue ? ++subject.UpdatedNo : subject.UpdatedNo = 1;

        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataDeletedSuccessfully });
    }

    #endregion

    #endregion

    #endregion

    #region Finish

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "21s:c")]
    [Description("Arb Tahiri", "Action to add finished status in staff registration.")]
    public async Task<IActionResult> Finish(string ide, MethodType method)
    {
        if (string.IsNullOrEmpty(ide))
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        if (method == MethodType.Post)
        {
            db.StaffRegistrationStatus.Add(new StaffRegistrationStatus
            {
                StaffId = CryptoSecurity.Decrypt<int>(ide),
                StatusTypeId = (int)StatusTypeEnum.Finished,
                InsertedDate = DateTime.Now,
                InsertedFrom = user.Id
            });
            await db.SaveChangesAsync();
        }

        TempData.Set("Error", new ErrorVM { Status = ErrorStatus.Success, Title = Resource.Success, Description = method == MethodType.Post ? Resource.DataRegisteredSuccessfully : Resource.DataUpdatedSuccessfully });
        return RedirectToAction(nameof(Index));
    }

    #endregion

    #region List

    [HttpGet, Authorize(Policy = "21s:r")]
    [Description("Arb Tahiri", "Entry home. Search for staff.")]
    public IActionResult Index() => View();

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "21s:r")]
    [Description("Arb Tahiri", "Form to list searched list of staff.")]
    public async Task<IActionResult> Search(Search search)
    {
        var list = await db.Staff
            .Include(a => a.StaffDepartment).ThenInclude(a => a.Department)
            .Include(a => a.User)
            .Where(a => (a.StaffDepartment.Any(b => b.DepartmentId == (search.Department ?? b.DepartmentId))
                && a.StaffDepartment.Any(b => b.StaffTypeId == (search.Department ?? b.StaffTypeId))
                && a.StaffDepartment.Any(a => a.EndDate >= DateTime.Now))
                && (string.IsNullOrEmpty(search.PersonalNumber) || a.PersonalNumber.Contains(search.PersonalNumber))
                && (string.IsNullOrEmpty(search.Firstname) || a.FirstName.Contains(search.Firstname))
                && (string.IsNullOrEmpty(search.Lastname) || a.LastName.Contains(search.Lastname)))
            .AsSplitQuery()
            .Select(a => new StaffDetails
            {
                Ide = CryptoSecurity.Encrypt(a.PersonalNumber),
                Firstname = a.FirstName,
                Lastname = a.LastName,
                PersonalNumber = a.PersonalNumber,
                ProfileImage = a.User.ProfileImage,
                Gender = a.Gender == ((int)GenderEnum.Male) ? Resource.Male : Resource.Female,
                Department = a.StaffDepartment.Select(a => user.Language == LanguageEnum.Albanian ? a.Department.NameSq : a.Department.NameEn).FirstOrDefault(),
                Email = a.User.Email,
                PhoneNumber = a.User.PhoneNumber,
                StaffType = string.Join(", ", a.StaffDepartment.Select(a => user.Language == LanguageEnum.Albanian ? a.StaffType.NameSq : a.StaffType.NameEn).ToList())
            }).ToListAsync();
        return Json(list);
    }

    [HttpGet, Authorize(Policy = "21s:r")]
    [Description("Arb Tahiri", "Form to display list of staff that are in process of registration.")]
    public async Task<IActionResult> InProcess()
    {
        var list = await db.Staff
            .Include(a => a.StaffDepartment).ThenInclude(a => a.Department)
            .Include(a => a.User)
            .Where(a => !a.StaffRegistrationStatus.Any(a => a.StatusTypeId == (int)StatusTypeEnum.Finished))
            .AsSplitQuery()
            .Select(a => new StaffDetails
            {
                Ide = CryptoSecurity.Encrypt(a.PersonalNumber),
                Firstname = a.FirstName,
                Lastname = a.LastName,
                PersonalNumber = a.PersonalNumber,
                ProfileImage = a.User.ProfileImage,
                Gender = a.Gender == ((int)GenderEnum.Male) ? Resource.Male : Resource.Female,
                Email = a.User.Email,
                PhoneNumber = a.User.PhoneNumber,
                InsertedDate = a.InsertedDate
            }).ToListAsync();
        return PartialView(list);
    }

    #endregion

    #region Profile

    #region Main form

    [HttpGet, Authorize(Policy = "22p:r")]
    [Description("Arb Tahiri", "Form to view the profile of staff.")]
    public async Task<IActionResult> Profile(string ide)
    {
        if (string.IsNullOrEmpty(ide))
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        var staffDetails = await db.Staff
            .Include(a => a.User)
            .Include(a => a.StaffDepartment).ThenInclude(a => a.Department)
            .Where(a => a.PersonalNumber == CryptoSecurity.Decrypt<string>(ide))
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
                Country = a.Country,
                ZIP = a.PostalCode
            }).FirstOrDefaultAsync();

        var profile = new ProfileVM
        {
            PersonalNumber = ide,
            StaffDetails = staffDetails,
            QualificationsCount = await db.StaffQualification.CountAsync(a => a.StaffId == CryptoSecurity.Decrypt<int>(staffDetails.Ide)),
            DocumentsCount = await db.StaffDocument.CountAsync(a => a.StaffId == CryptoSecurity.Decrypt<int>(staffDetails.Ide)),
            SubjectsCount = await db.StaffDepartmentSubject.CountAsync(a => a.StaffDepartment.StaffId == CryptoSecurity.Decrypt<int>(staffDetails.Ide))
        };
        return View(profile);
    }

    #endregion

    #region 1. Department

    [Authorize(Policy = "21sds:r"), Description("Arb Tahiri", "Form to display list of departments.")]
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
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                Description = a.Description
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

    [Authorize(Policy = "21sq:c"), Description("Arb Tahiri", "Form to display list of qualifications.")]
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

    [Authorize(Policy = "21sd:c"), Description("Arb Tahiri", "Form to display list of documents.")]
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

    [Authorize(Policy = "21sds:c"), Description("Arb Tahiri", "Form to display list of subjects.")]
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

    [HttpPost, Authorize(Policy = "22p:r"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Action to change profile photo.")]
    public async Task<IActionResult> ChangeImage(IFormFile Image, string PersonalNumber)
    {
        if (string.IsNullOrEmpty(PersonalNumber))
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        string userId = await db.Staff.Where(a => a.PersonalNumber == CryptoSecurity.Decrypt<string>(PersonalNumber)).Select(a => a.UserId).FirstOrDefaultAsync();
        var aspUser = await db.AspNetUsers.FirstOrDefaultAsync(a => a.Id == userId);

        string filePath = Image != null ? await SaveImage(environment, Image, "Users") : null;
        aspUser.ProfileImage = filePath;
        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataRegisteredSuccessfully, Icon = filePath });
    }

    #endregion

    #endregion

    #region Remote

    [Description("Arb Tahiri", "Method to check birthday.")]
    public IActionResult CheckBirthdate(string BirthDate)
    {
        var birthdate = DateTime.ParseExact(BirthDate, "dd/MM/yyyy", null);

        if (birthdate >= DateTime.Now)
        {
            return Json(Resource.NotAllowedGreaterDate);
        }

        if (birthdate <= DateTime.Now.AddYears(-18))
        {
            return Json(true);
        }
        else
        {
            return Json(false);
        }
    }

    [Description("Arb Tahiri", "Method to check end date of subject.")]
    public IActionResult CheckEndDate(DateTime DepartmentEndDate, string EndDate)
    {
        var endDate = DateTime.ParseExact(EndDate, "dd/MM/yyyy", null);

        if (endDate <= DepartmentEndDate)
        {
            return Json(true);
        }
        else
        {
            return Json(false);
        }
    }

    [Description("Arb Tahiri", "Method to check end date and start date.")]
    public IActionResult CheckDates(string StartDate, string EndDate)
    {
        var startDate = DateTime.ParseExact(StartDate, "dd/MM/yyyy", null);
        var endDate = DateTime.ParseExact(EndDate, "dd/MM/yyyy", null);

        if (startDate <= endDate)
        {
            return Json(true);
        }
        else
        {
            return Json(false);
        }
    }

    [Description("Arb Tahiri", "Method to check end date and start date.")]
    public IActionResult CheckDatesQualification(string From, string To)
    {
        var startDate = DateTime.ParseExact(From, "dd/MM/yyyy", null);
        var endDate = DateTime.ParseExact(To, "dd/MM/yyyy", null);

        if (startDate <= endDate)
        {
            return Json(true);
        }
        else
        {
            return Json(false);
        }
    }

    #endregion

    #region Open document

    [HttpGet, Description("Arb Tahiri", "Action to open documents.")]
    public async Task<IActionResult> OpenDocument(string ide)
    {
        var getDocument = await db.StaffDocument.FirstOrDefaultAsync(a => a.StaffDocumentId == CryptoSecurity.Decrypt<int>(ide));
        var openDocument = new OpenDocument
        {
            Path = getDocument.Path,
            Name = getDocument.Title
        };

        return View(openDocument);
    }

    #endregion
}
