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
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.Controllers;

[Authorize]
public class EvaluationController : BaseController
{
    public EvaluationController(HRMS_WorkContext db, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        : base(db, signInManager, userManager)
    {
    }

    [Authorize(Policy = "70:r"), Description("Arb Tahiri", "Form to display list of evaluations.")]
    public IActionResult Index() => View();

    [Authorize(Policy = "70:r"), Description("Arb Tahiri", "Form to search for manager evaluations.")]
    public async Task<IActionResult> _SearchManager(Search search)
    {
        var list = await db.EvaluationManager
            .Include(a => a.Evaluation).ThenInclude(a => a.EvaluationStatus).ThenInclude(a => a.StatusType)
            .Include(a => a.Evaluation).ThenInclude(a => a.EvaluationType)
            .Where(a => a.Evaluation.EvaluationTypeId == (search.EvaluationTypeId ?? a.Evaluation.EvaluationTypeId)
                && a.Evaluation.EvaluationStatus.Any(a => a.StatusTypeId == (search.StatusTypeId ?? a.StatusTypeId))
                && a.StaffId == (search.StaffId ?? a.StaffId))
            .AsSplitQuery()
            .Select(a => new EvaluationList
            {
                EvaluationManagerIde = CryptoSecurity.Encrypt(a.EvaluationManagerId),
                Title = a.Title,
                Description = a.Description,
                StatusType = a.Evaluation.EvaluationStatus.OrderByDescending(a => a.EvaluationStatusId).Select(a => user.Language == LanguageEnum.Albanian ? a.StatusType.NameSq : a.StatusType.NameEn).FirstOrDefault(),
                Questions = a.Evaluation.EvaluationQuestionnaireNumerical.Count(a => a.Active) + a.Evaluation.EvaluationQuestionnaireOptional.Count(a => a.Active) + a.Evaluation.EvaluationQuestionnaireTopic.Count(a => a.Active),
                Answers = a.Evaluation.EvaluationQuestionnaireNumerical.Count(a => a.Active && a.Grade.HasValue) + a.Evaluation.EvaluationQuestionnaireOptional.Count(a => a.Active && a.EvaluationQuestionnaireOptionalOption.Any(a => a.Active && a.Checked)) + a.Evaluation.EvaluationQuestionnaireTopic.Count(a => a.Active && string.IsNullOrEmpty(a.Answer)),
                InsertedDate = a.InsertedDate
            }).ToListAsync();
        return Json(list);
    }

    [Authorize(Policy = "70:r"), Description("Arb Tahiri", "Form to search for students evaluation.")]
    public async Task<IActionResult> _SearchStudent(Search search)
    {
        var list = await db.EvaluationStudents
            .Include(a => a.Evaluation).ThenInclude(a => a.EvaluationStatus).ThenInclude(a => a.StatusType)
            .Include(a => a.Evaluation).ThenInclude(a => a.EvaluationType)
            .Where(a => a.Evaluation.EvaluationTypeId == (search.EvaluationTypeId ?? a.Evaluation.EvaluationTypeId)
                && a.Evaluation.EvaluationStatus.Any(a => a.StatusTypeId == (search.StatusTypeId ?? a.StatusTypeId))
                && a.StaffDepartmentSubject.StaffDepartment.StaffId == (search.StaffId ?? a.StaffDepartmentSubject.StaffDepartment.StaffId))
            .AsSplitQuery()
            .Select(a => new EvaluationList
            {
                EvaluationManagerIde = CryptoSecurity.Encrypt(a.EvaluationStudentsId),
                Title = a.Title,
                Description = a.Description,
                StatusType = a.Evaluation.EvaluationStatus.OrderByDescending(a => a.EvaluationStatusId).Select(a => user.Language == LanguageEnum.Albanian ? a.StatusType.NameSq : a.StatusType.NameEn).FirstOrDefault(),
                Questions = a.Evaluation.EvaluationQuestionnaireNumerical.Count(a => a.Active) + a.Evaluation.EvaluationQuestionnaireOptional.Count(a => a.Active) + a.Evaluation.EvaluationQuestionnaireTopic.Count(a => a.Active),
                Answers = a.Evaluation.EvaluationQuestionnaireNumerical.Count(a => a.Active && a.Grade.HasValue) + a.Evaluation.EvaluationQuestionnaireOptional.Count(a => a.Active && a.EvaluationQuestionnaireOptionalOption.Any(a => a.Active && a.Checked)) + a.Evaluation.EvaluationQuestionnaireTopic.Count(a => a.Active && string.IsNullOrEmpty(a.Answer)),
                //NumberOfStudents = a.StudentsNo
                InsertedDate = a.InsertedDate
            }).ToListAsync();
        return Json(list);
    }

    [Authorize(Policy = "70:r"), Description("Arb Tahiri", "Form to search for evaluation types.")]
    public async Task<IActionResult> _SearchSelf(Search search)
    {
        var list = await db.EvaluationSelf
            .Include(a => a.Evaluation).ThenInclude(a => a.EvaluationStatus).ThenInclude(a => a.StatusType)
            .Include(a => a.Evaluation).ThenInclude(a => a.EvaluationType)
            .Where(a => a.Evaluation.EvaluationTypeId == (search.EvaluationTypeId ?? a.Evaluation.EvaluationTypeId)
                && a.Evaluation.EvaluationStatus.Any(a => a.StatusTypeId == (search.StatusTypeId ?? a.StatusTypeId))
                && a.StaffDepartment.StaffId == (search.StaffId ?? a.StaffDepartment.StaffId))
            .AsSplitQuery()
            .Select(a => new EvaluationList
            {
                EvaluationManagerIde = CryptoSecurity.Encrypt(a.EvaluationSelfId),
                Title = a.Title,
                Description = a.Description,
                StatusType = a.Evaluation.EvaluationStatus.OrderByDescending(a => a.EvaluationStatusId).Select(a => user.Language == LanguageEnum.Albanian ? a.StatusType.NameSq : a.StatusType.NameEn).FirstOrDefault(),
                Questions = a.Evaluation.EvaluationQuestionnaireNumerical.Count(a => a.Active) + a.Evaluation.EvaluationQuestionnaireOptional.Count(a => a.Active) + a.Evaluation.EvaluationQuestionnaireTopic.Count(a => a.Active),
                Answers = a.Evaluation.EvaluationQuestionnaireNumerical.Count(a => a.Active && a.Grade.HasValue) + a.Evaluation.EvaluationQuestionnaireOptional.Count(a => a.Active && a.EvaluationQuestionnaireOptionalOption.Any(a => a.Active && a.Checked)) + a.Evaluation.EvaluationQuestionnaireTopic.Count(a => a.Active && string.IsNullOrEmpty(a.Answer)),
                InsertedDate = a.InsertedDate
            }).ToListAsync();
        return Json(list);
    }
}
