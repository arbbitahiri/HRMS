using HRMS.Data.Core;
using HRMS.Data.General;
using HRMS.Models;
using HRMS.Models.Evaluation;
using HRMS.Resources;
using HRMS.Utilities;
using HRMS.Utilities.General;
using HRMS.Utilities.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.Controllers;

[Authorize]
public class EvaluationController : BaseController
{
    public EvaluationController(HRMSContext db, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        : base(db, signInManager, userManager)
    {
    }

    [Authorize(Policy = "71:m"), Description("Korab Mustafa", "Form to display list of evaluation types.")]
    public async Task<IActionResult> Index()
    {
        var availableEvaluation = !User.IsInRole("Manager") ? new int[] { (int)EvaluationTypeEnum.Self } : new int[] { (int)EvaluationTypeEnum.Manager, (int)EvaluationTypeEnum.StudentCollege, (int)EvaluationTypeEnum.StudentStaff, (int)EvaluationTypeEnum.Self };
        var list = await db.EvaluationType
            .Where(a => a.Active && availableEvaluation.Contains(a.EvaluationTypeId))
            .Select(a => new EvaluationTypes
            {
                EvaluationType = (EvaluationTypeEnum)a.EvaluationTypeId,
                Title = user.Language == LanguageEnum.Albanian ? a.NameSq : a.NameEn
            }).ToListAsync();
        return View(list);
    }

    [Authorize(Policy = "70:m"), Description("Korab Mustafa", "Form to display list of evaluation data.")]
    public IActionResult Search() => View();

    #region Search for evaluations

    [HttpPost, Authorize(Policy = "70:r"), ValidateAntiForgeryToken]
    [Description("Korab Mustafa", "Form to search for manager evaluations.")]
    public async Task<IActionResult> _SearchManager(Search search)
    {
        var role = User.Claims.Where(t => t.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Select(a => a.Value).FirstOrDefault();

        var list = await db.EvaluationManager
            .Include(a => a.Evaluation).ThenInclude(a => a.EvaluationStatus).ThenInclude(a => a.StatusType)
            .Include(a => a.Evaluation).ThenInclude(a => a.EvaluationType)
            .Where(a => a.Evaluation.EvaluationTypeId == (search.EvaluationTypeId ?? a.Evaluation.EvaluationTypeId)
                && a.Evaluation.EvaluationStatus.Any(a => a.Active && a.StatusTypeId == (search.StatusTypeId ?? a.StatusTypeId))
                && (role == "Manager" ? (a.StaffId == (search.StaffId ?? a.StaffId)) : (a.Staff.UserId == user.Id)))
            .AsSplitQuery()
            .Select(a => new EvaluationList
            {
                EvaluationIde = CryptoSecurity.Encrypt(a.EvaluationId),
                EvaluationManagerIde = CryptoSecurity.Encrypt(a.EvaluationManagerId),
                Title = a.Title,
                Description = a.Description,
                StatusType = a.Evaluation.EvaluationStatus.Where(a => a.Active).Select(a => user.Language == LanguageEnum.Albanian ? a.StatusType.NameSq : a.StatusType.NameEn).FirstOrDefault(),
                Questions = a.Evaluation.EvaluationQuestionnaireNumerical.Count(a => a.Active) + a.Evaluation.EvaluationQuestionnaireOptional.Count(a => a.Active && a.EvaluationQuestionnaireOptionalOption.Any(b => b.Active)) + a.Evaluation.EvaluationQuestionnaireTopic.Count(a => a.Active),
                Answers = a.Evaluation.EvaluationQuestionnaireNumerical.Count(a => a.Active && a.Grade.HasValue) + a.Evaluation.EvaluationQuestionnaireOptional.Count(a => a.Active && a.EvaluationQuestionnaireOptionalOption.Any(a => a.Active && a.Checked)) + a.Evaluation.EvaluationQuestionnaireTopic.Count(a => a.Active && !string.IsNullOrEmpty(a.Answer)),
                Finished = (int)Status.Finished == a.Evaluation.EvaluationStatus.Where(a => a.Active).Select(a => a.StatusTypeId).FirstOrDefault(),
                Deleted = (int)Status.Deleted == a.Evaluation.EvaluationStatus.Where(a => a.Active).Select(a => a.StatusTypeId).FirstOrDefault(),
                InsertedDate = a.InsertedDate
            }).ToListAsync();
        return PartialView(list);
    }

    [HttpPost, Authorize(Policy = "70:r"), ValidateAntiForgeryToken]
    [Description("Korab Mustafa", "Form to search for students evaluation.")]
    public async Task<IActionResult> _SearchStudentCollege(Search search)
    {
        var list = await db.EvaluationStudentsCollege
            .Include(a => a.Evaluation).ThenInclude(a => a.EvaluationStatus).ThenInclude(a => a.StatusType)
            .Include(a => a.Evaluation).ThenInclude(a => a.EvaluationType)
            .Where(a => a.Evaluation.EvaluationTypeId == (search.EvaluationTypeId ?? a.Evaluation.EvaluationTypeId)
                && a.Evaluation.EvaluationStatus.Any(a => a.Active && a.StatusTypeId == (search.StatusTypeId ?? a.StatusTypeId)))
            .AsSplitQuery()
            .Select(a => new EvaluationList
            {
                EvaluationIde = CryptoSecurity.Encrypt(a.EvaluationId),
                EvaluationStudentsCollegeIde = CryptoSecurity.Encrypt(a.EvaluationStudentsCollegeId),
                NumberOfStudents = a.StudentsNo,
                Title = a.Title,
                StatusType = a.Evaluation.EvaluationStatus.Where(a => a.Active).Select(a => user.Language == LanguageEnum.Albanian ? a.StatusType.NameSq : a.StatusType.NameEn).FirstOrDefault(),
                Questions = a.Evaluation.EvaluationQuestionnaireNumerical.Count(a => a.Active) + a.Evaluation.EvaluationQuestionnaireOptional.Count(a => a.Active && a.EvaluationQuestionnaireOptionalTopic.Any(b => b.Active)) + a.Evaluation.EvaluationQuestionnaireTopic.Count(a => a.Active),
                Answers = a.Evaluation.EvaluationQuestionnaireNumerical.Count(a => a.Active && a.Grade.HasValue) + a.Evaluation.EvaluationQuestionnaireOptional.Count(a => a.Active && a.EvaluationQuestionnaireOptionalTopic.Any(a => a.Active && !string.IsNullOrEmpty(a.Answer))) + a.Evaluation.EvaluationQuestionnaireTopic.Count(a => a.Active && !string.IsNullOrEmpty(a.Answer)),
                Finished = (int)Status.Finished == a.Evaluation.EvaluationStatus.Where(a => a.Active).Select(a => a.StatusTypeId).FirstOrDefault(),
                Deleted = (int)Status.Deleted == a.Evaluation.EvaluationStatus.Where(a => a.Active).Select(a => a.StatusTypeId).FirstOrDefault(),
                InsertedDate = a.InsertedDate
            }).ToListAsync();
        return PartialView(list);
    }

    [HttpPost, Authorize(Policy = "70:r"), ValidateAntiForgeryToken]
    [Description("Korab Mustafa", "Form to search for students evaluation.")]
    public async Task<IActionResult> _SearchStudentStaff(Search search)
    {
        var role = User.Claims.Where(t => t.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Select(a => a.Value).FirstOrDefault();

        var list = await db.EvaluationStudentsStaff
            .Include(a => a.Evaluation).ThenInclude(a => a.EvaluationStatus).ThenInclude(a => a.StatusType)
            .Include(a => a.Evaluation).ThenInclude(a => a.EvaluationType)
            .Include(a => a.StaffDepartmentSubject).ThenInclude(a => a.StaffDepartment).ThenInclude(a => a.Staff)
            .Include(a => a.StaffDepartmentSubject).ThenInclude(a => a.Subject)
            .Where(a => a.Evaluation.EvaluationTypeId == (search.EvaluationTypeId ?? a.Evaluation.EvaluationTypeId)
                && a.Evaluation.EvaluationStatus.Any(a => a.Active && a.StatusTypeId == (search.StatusTypeId ?? a.StatusTypeId))
                && (role == "Manager" ? (a.StaffDepartmentSubject.StaffDepartment.StaffId == (search.StaffId ?? a.StaffDepartmentSubject.StaffDepartment.StaffId)) : (a.StaffDepartmentSubject.StaffDepartment.Staff.UserId == user.Id)))
            .AsSplitQuery()
            .Select(a => new EvaluationList
            {
                EvaluationIde = CryptoSecurity.Encrypt(a.EvaluationId),
                EvaluationStudentsStaffIde = CryptoSecurity.Encrypt(a.EvaluationStudentsStaffId),
                StaffName = $"{a.StaffDepartmentSubject.StaffDepartment.Staff.FirstName} {a.StaffDepartmentSubject.StaffDepartment.Staff.LastName}",
                Subject = user.Language == LanguageEnum.Albanian ? a.StaffDepartmentSubject.Subject.NameSq : a.StaffDepartmentSubject.Subject.NameEn,
                NumberOfStudents = a.StudentsNo,
                Title = a.Title,
                StatusType = a.Evaluation.EvaluationStatus.Where(a => a.Active).Select(a => user.Language == LanguageEnum.Albanian ? a.StatusType.NameSq : a.StatusType.NameEn).FirstOrDefault(),
                Questions = a.Evaluation.EvaluationQuestionnaireNumerical.Count(a => a.Active) + a.Evaluation.EvaluationQuestionnaireOptional.Count(a => a.Active && a.EvaluationQuestionnaireOptionalOption.Any(b => b.Active)),
                Answers = a.Evaluation.EvaluationQuestionnaireNumerical.Count(a => a.Active && a.Grade.HasValue) + a.Evaluation.EvaluationQuestionnaireOptional.Count(a => a.Active && a.EvaluationQuestionnaireOptionalOption.Any(a => a.Active && a.Checked)),
                Finished = (int)Status.Finished == a.Evaluation.EvaluationStatus.Where(a => a.Active).Select(a => a.StatusTypeId).FirstOrDefault(),
                Deleted = (int)Status.Deleted == a.Evaluation.EvaluationStatus.Where(a => a.Active).Select(a => a.StatusTypeId).FirstOrDefault(),
                InsertedDate = a.InsertedDate
            }).ToListAsync();
        return PartialView(list);
    }

    [HttpPost, Authorize(Policy = "70:r"), ValidateAntiForgeryToken]
    [Description("Korab Mustafa", "Form to search for self evaluations.")]
    public async Task<IActionResult> _SearchSelf(Search search)
    {
        var role = User.Claims.Where(t => t.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Select(a => a.Value).FirstOrDefault();

        var list = await db.EvaluationSelf
            .Include(a => a.Evaluation).ThenInclude(a => a.EvaluationStatus).ThenInclude(a => a.StatusType)
            .Include(a => a.Evaluation).ThenInclude(a => a.EvaluationType)
            .Where(a => a.Evaluation.EvaluationTypeId == (search.EvaluationTypeId ?? a.Evaluation.EvaluationTypeId)
                && a.Evaluation.EvaluationStatus.Any(a => a.Active && a.StatusTypeId == (search.StatusTypeId ?? a.StatusTypeId))
                && (role == "Manager" ? (a.StaffId == (search.StaffId ?? a.StaffId)) : (a.Staff.UserId == user.Id)))
            .AsSplitQuery()
            .Select(a => new EvaluationList
            {
                EvaluationIde = CryptoSecurity.Encrypt(a.EvaluationId),
                EvaluationManagerIde = CryptoSecurity.Encrypt(a.EvaluationSelfId),
                Title = a.Title,
                Description = a.Description,
                StatusType = a.Evaluation.EvaluationStatus.Where(a => a.Active).Select(a => user.Language == LanguageEnum.Albanian ? a.StatusType.NameSq : a.StatusType.NameEn).FirstOrDefault(),
                Questions = a.Evaluation.EvaluationQuestionnaireNumerical.Count(a => a.Active) + a.Evaluation.EvaluationQuestionnaireOptional.Count(a => a.Active && a.EvaluationQuestionnaireOptionalOption.Any(b => b.Active)) + a.Evaluation.EvaluationQuestionnaireTopic.Count(a => a.Active),
                Answers = a.Evaluation.EvaluationQuestionnaireNumerical.Count(a => a.Active && a.Grade.HasValue) + a.Evaluation.EvaluationQuestionnaireOptional.Count(a => a.Active && a.EvaluationQuestionnaireOptionalOption.Any(a => a.Active && a.Checked)) + a.Evaluation.EvaluationQuestionnaireTopic.Count(a => a.Active && !string.IsNullOrEmpty(a.Answer)),
                Finished = (int)Status.Finished == a.Evaluation.EvaluationStatus.Where(a => a.Active).Select(a => a.StatusTypeId).FirstOrDefault(),
                Deleted = (int)Status.Deleted == a.Evaluation.EvaluationStatus.Where(a => a.Active).Select(a => a.StatusTypeId).FirstOrDefault(),
                InsertedDate = a.InsertedDate
            }).ToListAsync();
        return PartialView(list);
    }

    [HttpPost, Authorize(Policy = "70:d"), ValidateAntiForgeryToken]
    [Description("Korab Mustafa", "Form to search for self evaluations.")]
    public async Task<IActionResult> Delete(string ide)
    {
        if (string.IsNullOrEmpty(ide))
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        var evaluationStatus = await db.EvaluationStatus.FirstOrDefaultAsync(a => a.Active && a.EvaluationId == CryptoSecurity.Decrypt<int>(ide));
        evaluationStatus.Active = false;
        evaluationStatus.UpdatedDate = DateTime.Now;
        evaluationStatus.UpdatedFrom = user.Id;
        evaluationStatus.UpdatedNo = evaluationStatus.UpdatedNo.HasValue ? ++evaluationStatus.UpdatedNo : evaluationStatus.UpdatedNo = 1;

        db.EvaluationStatus.Add(new EvaluationStatus
        {
            EvaluationId = CryptoSecurity.Decrypt<int>(ide),
            StatusTypeId = (int)Status.Deleted,
            Active = true,
            InsertedDate = DateTime.Now,
            InsertedFrom = user.Id
        });
        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataDeletedSuccessfully });
    }

    #endregion

    #region Details

    [HttpGet, Authorize(Policy = "71de:r"), Description("Korab Mustafa", "Form to display questionnaire details")]
    public async Task<ActionResult> Details(string ide)
    {
        if (string.IsNullOrEmpty(ide))
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        var evaluationType = await db.Evaluation.Where(a => a.EvaluationId == CryptoSecurity.Decrypt<int>(ide)).Select(a => (EvaluationTypeEnum)a.EvaluationTypeId).FirstOrDefaultAsync();
        var evaluationDetails = new EvaluationDetails();
        if (evaluationType == EvaluationTypeEnum.Manager)
        {
            evaluationDetails = await db.EvaluationManager.Include(a => a.Evaluation).ThenInclude(a => a.EvaluationType)
                .Where(a => a.Evaluation.EvaluationStatus.Any(a => a.StatusTypeId != (int)Status.Deleted)
                    && a.EvaluationId == CryptoSecurity.Decrypt<int>(ide))
                .Select(a => new EvaluationDetails
                {
                    EvaluationIde = ide,
                    EvaluationType = user.Language == LanguageEnum.Albanian ? a.Evaluation.EvaluationType.NameSq : a.Evaluation.EvaluationType.NameEn,
                    InsertedDate = a.InsertedDate,
                    MethodType = MethodType.Get,
                    EvaluationTypeEnum = evaluationType,
                    Questions = a.Evaluation.EvaluationQuestionnaireNumerical.Count(a => a.Active) + a.Evaluation.EvaluationQuestionnaireOptional.Count(a => a.Active && a.EvaluationQuestionnaireOptionalOption.Any(b => b.Active)) + a.Evaluation.EvaluationQuestionnaireTopic.Count(a => a.Active),
                    Answers = a.Evaluation.EvaluationQuestionnaireNumerical.Count(a => a.Active && a.Grade.HasValue) + a.Evaluation.EvaluationQuestionnaireOptional.Count(a => a.Active && a.EvaluationQuestionnaireOptionalOption.Any(a => a.Active && a.Checked)) + a.Evaluation.EvaluationQuestionnaireTopic.Count(a => a.Active && !string.IsNullOrEmpty(a.Answer)),
                    Manager = $"{a.Manager.Staff.FirstName} {a.Manager.Staff.LastName}",
                    Staff = $"{a.Staff.FirstName} {a.Staff.LastName}",
                    Title = a.Title,
                    Description = a.Description
                }).FirstOrDefaultAsync();
        }
        else if (evaluationType == EvaluationTypeEnum.StudentCollege)
        {
            evaluationDetails = await db.EvaluationStudentsCollege
                .Include(a => a.Evaluation).ThenInclude(a => a.EvaluationType)
                .Where(a => a.Evaluation.EvaluationStatus.Any(a => a.StatusTypeId != (int)Status.Deleted)
                    && a.EvaluationId == CryptoSecurity.Decrypt<int>(ide))
                .Select(a => new EvaluationDetails
                {
                    EvaluationIde = ide,
                    EvaluationType = user.Language == LanguageEnum.Albanian ? a.Evaluation.EvaluationType.NameSq : a.Evaluation.EvaluationType.NameEn,
                    InsertedDate = a.InsertedDate,
                    MethodType = MethodType.Get,
                    EvaluationTypeEnum = evaluationType,
                    Students = a.StudentsNo,
                    Questions = a.Evaluation.EvaluationQuestionnaireNumerical.Count(a => a.Active) + a.Evaluation.EvaluationQuestionnaireOptional.Count(a => a.Active && a.EvaluationQuestionnaireOptionalTopic.Any(b => b.Active)) + a.Evaluation.EvaluationQuestionnaireTopic.Count(a => a.Active),
                    Answers = a.Evaluation.EvaluationQuestionnaireNumerical.Count(a => a.Active && a.Grade.HasValue) + a.Evaluation.EvaluationQuestionnaireOptional.Count(a => a.Active && a.EvaluationQuestionnaireOptionalTopic.Any(a => a.Active && !string.IsNullOrEmpty(a.Answer))) + a.Evaluation.EvaluationQuestionnaireTopic.Count(a => a.Active && !string.IsNullOrEmpty(a.Answer)),
                    Title = a.Title,
                    Description = a.Description
                }).FirstOrDefaultAsync();
        }
        else if (evaluationType == EvaluationTypeEnum.StudentStaff)
        {
            evaluationDetails = await db.EvaluationStudentsStaff
                .Include(a => a.Evaluation).ThenInclude(a => a.EvaluationType)
                .Include(a => a.StaffDepartmentSubject).ThenInclude(a => a.StaffDepartment).ThenInclude(a => a.Staff)
                .Include(a => a.StaffDepartmentSubject).ThenInclude(a => a.Subject)
                .Where(a => a.Evaluation.EvaluationStatus.Any(a => a.StatusTypeId != (int)Status.Deleted)
                    && a.EvaluationId == CryptoSecurity.Decrypt<int>(ide))
                .Select(a => new EvaluationDetails
                {
                    EvaluationIde = ide,
                    EvaluationType = user.Language == LanguageEnum.Albanian ? a.Evaluation.EvaluationType.NameSq : a.Evaluation.EvaluationType.NameEn,
                    InsertedDate = a.InsertedDate,
                    MethodType = MethodType.Get,
                    EvaluationTypeEnum = evaluationType,
                    Staff = $"{a.StaffDepartmentSubject.StaffDepartment.Staff.FirstName} {a.StaffDepartmentSubject.StaffDepartment.Staff.LastName}",
                    Subject = user.Language == LanguageEnum.Albanian ? a.StaffDepartmentSubject.Subject.NameSq : a.StaffDepartmentSubject.Subject.NameEn,
                    Students = a.StudentsNo,
                    Questions = a.Evaluation.EvaluationQuestionnaireNumerical.Count(a => a.Active) + a.Evaluation.EvaluationQuestionnaireOptional.Count(a => a.Active && a.EvaluationQuestionnaireOptionalOption.Any(b => b.Active)),
                    Answers = a.Evaluation.EvaluationQuestionnaireNumerical.Count(a => a.Active && a.Grade.HasValue) + a.Evaluation.EvaluationQuestionnaireOptional.Count(a => a.Active && a.EvaluationQuestionnaireOptionalOption.Any(a => a.Active && a.Checked)),
                    Title = a.Title,
                    Description = a.Description
                }).FirstOrDefaultAsync();
        }
        else
        {
            evaluationDetails = await db.EvaluationSelf.Include(a => a.Evaluation).ThenInclude(a => a.EvaluationType)
                .Where(a => a.Evaluation.EvaluationStatus.Any(a => a.StatusTypeId != (int)Status.Deleted)
                    && a.EvaluationId == CryptoSecurity.Decrypt<int>(ide))
                .Select(a => new EvaluationDetails
                {
                    EvaluationIde = ide,
                    EvaluationType = user.Language == LanguageEnum.Albanian ? a.Evaluation.EvaluationType.NameSq : a.Evaluation.EvaluationType.NameEn,
                    InsertedDate = a.InsertedDate,
                    MethodType = MethodType.Get,
                    EvaluationTypeEnum = evaluationType,
                    Staff = $"{a.Staff.FirstName} {a.Staff.LastName}",
                    Questions = a.Evaluation.EvaluationQuestionnaireNumerical.Count(a => a.Active) + a.Evaluation.EvaluationQuestionnaireOptional.Count(a => a.Active && a.EvaluationQuestionnaireOptionalOption.Any(b => b.Active)) + a.Evaluation.EvaluationQuestionnaireTopic.Count(a => a.Active),
                    Answers = a.Evaluation.EvaluationQuestionnaireNumerical.Count(a => a.Active && a.Grade.HasValue) + a.Evaluation.EvaluationQuestionnaireOptional.Count(a => a.Active && a.EvaluationQuestionnaireOptionalOption.Any(a => a.Active && a.Checked)) + a.Evaluation.EvaluationQuestionnaireTopic.Count(a => a.Active && !string.IsNullOrEmpty(a.Answer)),
                    Title = a.Title,
                    Description = a.Description
                }).FirstOrDefaultAsync();
        }

        var documents = await db.EvaluationDocument
            .Include(a => a.DocumentType)
            .Where(a => a.Active && a.EvaluationId == CryptoSecurity.Decrypt<int>(ide))
            .Select(a => new Document
            {
                EvaluationDocumentIde = CryptoSecurity.Encrypt(a.EvaluationDocumentId),
                Title = a.Title,
                Path = a.Path,
                PathExtension = Path.GetExtension(a.Path),
                DocumentType = user.Language == LanguageEnum.Albanian ? a.DocumentType.NameSq : a.DocumentType.NameEn,
                Description = a.Description,
                Active = a.Active
            }).ToListAsync();

        var details = new DetailsVM
        {
            EvaluationDetails = evaluationDetails,
            Documents = documents
        };
        return View(details);
    }

    [HttpGet, Authorize(Policy = "71:r")]
    [Description("Korab Mustafa", "Form to view list of numerical questions.")]
    public async Task<IActionResult> _NumericalQuestions(string ide) =>
        PartialView(await db.EvaluationQuestionnaireNumerical
            .Where(a => a.Evaluation.EvaluationStatus.Any(a => a.StatusTypeId != (int)Status.Deleted)
                && a.EvaluationId == CryptoSecurity.Decrypt<int>(ide))
            .Select(a => new QuestionNumerical
            {
                EvaluationQuestionnaireNumericalIde = CryptoSecurity.Encrypt(a.EvaluationQuestionnaireNumericalId),
                Question = user.Language == LanguageEnum.Albanian ? a.QuestionSq : a.QuestionSq,
                Grade = a.Grade,
                Graded = a.Grade.HasValue
            }).ToListAsync());

    [HttpGet, Authorize(Policy = "71:r")]
    [Description("Korab Mustafa", "Form to view list of numerical questions.")]
    public async Task<IActionResult> _OptionalOptionQuestions(string ide) =>
        PartialView(await db.EvaluationQuestionnaireOptional
            .Where(a => a.Evaluation.EvaluationStatus.Any(a => a.StatusTypeId != (int)Status.Deleted)
                && a.EvaluationId == CryptoSecurity.Decrypt<int>(ide))
            .Select(a => new QuestionOptional
            {
                EvaluationQuestionnaireOptionalIde = CryptoSecurity.Encrypt(a.EvaluationQuestionnaireOptionalId),
                Question = user.Language == LanguageEnum.Albanian ? a.QuestionSq : a.QuestionSq,
                Options = a.EvaluationQuestionnaireOptionalOption.Select(a => new QuestionOption
                {
                    EvaluationQuestionnaireOptionalOptionIde = CryptoSecurity.Encrypt(a.EvaluationQuestionnaireOptionalOptionId),
                    Option = user.Language == LanguageEnum.Albanian ? a.OptionTitleSq : a.OptionTitleEn,
                    Checked = a.Checked
                }).ToList()
            }).ToListAsync());

    [HttpGet, Authorize(Policy = "71:r")]
    [Description("Korab Mustafa", "Form to view list of numerical questions.")]
    public async Task<IActionResult> _OptionalTopicQuestions(string ide) =>
        PartialView(await db.EvaluationQuestionnaireOptional
            .Where(a => a.Evaluation.EvaluationStatus.Any(a => a.StatusTypeId != (int)Status.Deleted)
                && a.EvaluationId == CryptoSecurity.Decrypt<int>(ide))
            .Select(a => new QuestionOptional
            {
                EvaluationQuestionnaireOptionalIde = CryptoSecurity.Encrypt(a.EvaluationQuestionnaireOptionalId),
                Question = user.Language == LanguageEnum.Albanian ? a.QuestionSq : a.QuestionSq,
                Options = a.EvaluationQuestionnaireOptionalOption.Select(a => new QuestionOption
                {
                    EvaluationQuestionnaireOptionalOptionIde = CryptoSecurity.Encrypt(a.EvaluationQuestionnaireOptionalOptionId),
                    Option = user.Language == LanguageEnum.Albanian ? a.OptionTitleSq : a.OptionTitleEn,
                    Checked = a.Checked
                }).ToList()
            }).ToListAsync());

    [HttpGet, Authorize(Policy = "71:r")]
    [Description("Korab Mustafa", "Form to view list of numerical questions.")]
    public async Task<IActionResult> _TopicQuestions(string ide) =>
        PartialView(await db.EvaluationQuestionnaireTopic
            .Where(a => a.Evaluation.EvaluationStatus.Any(a => a.StatusTypeId != (int)Status.Deleted)
                && a.EvaluationId == CryptoSecurity.Decrypt<int>(ide))
            .Select(a => new QuestionTopic
            {
                EvaluationQuestionnaireTopicIde = CryptoSecurity.Encrypt(a.EvaluationQuestionnaireTopicId),
                Question = user.Language == LanguageEnum.Albanian ? a.QuestionSq : a.QuestionSq,
                Answer = a.Answer
            }).ToListAsync());

    #endregion
}
