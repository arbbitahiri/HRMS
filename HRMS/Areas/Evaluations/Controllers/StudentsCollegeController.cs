using HRMS.Areas.Evaluations.Models.StudentsCollege;
using HRMS.Controllers;
using HRMS.Data.Core;
using HRMS.Data.General;
using HRMS.Models;
using HRMS.Resources;
using HRMS.Utilities;
using HRMS.Utilities.General;
using HRMS.Utilities.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.Areas.Evaluation.Controllers;

[Authorize]
[Route("/{area}/{controller}/{action}")]
public class StudentsCollegeController : BaseController
{
    private readonly IConfiguration configuration;
    private readonly IWebHostEnvironment environment;

    public StudentsCollegeController(IConfiguration configuration, IWebHostEnvironment environment,
        HRMSContext db, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        : base(db, signInManager, userManager)
    {
        this.configuration = configuration;
        this.environment = environment;
    }

    #region 1. Evaluation data

    [HttpGet, Authorize(Policy = "72:c")]
    [Description("Arb Tahiri", "Form to register/edit self evaluation. First step of registering/editing questionnaire.")]
    public async Task<IActionResult> Index(string ide)
    {
        if (string.IsNullOrEmpty(ide))
        {
            if (await db.EvaluationStatus.AnyAsync(a => a.Evaluation.EvaluationTypeId == (int)EvaluationTypeEnum.Self && a.StatusTypeId == (int)StatusTypeEnum.Processing))
            {
                return Json(new ErrorVM { Status = ErrorStatus.Warning, Title = Resource.Warning, Description = Resource.YouHaveEvaluationProcessing });
            }

            return View(new Register { MethodType = MethodType.Post });
        }

        var data = await db.EvaluationStudentsCollege.Include(a => a.Evaluation).ThenInclude(a => a.EvaluationType)
            .Where(a => a.Evaluation.EvaluationStatus.Any(a => a.StatusTypeId != (int)StatusTypeEnum.Deleted)
                && a.EvaluationStudentsCollegeId == CryptoSecurity.Decrypt<int>(ide))
            .Select(a => new Register
            {
                EvaluationIde = ide,
                EvaluationType = user.Language == LanguageEnum.Albanian ? a.Evaluation.EvaluationType.NameSq : a.Evaluation.EvaluationType.NameEn,
                InsertedDate = a.InsertedDate,
                MethodType = MethodType.Put,
                NumberOfStudents = a.StudentsNo,
                Title = a.Title,
                Description = a.Description
            }).FirstOrDefaultAsync();
        return View(data);
    }

    [HttpPost, Authorize(Policy = "72:c"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Action to register new manager evaluation.")]
    public async Task<IActionResult> Register(Register create)
    {
        if (!ModelState.IsValid)
        {
            TempData.Set("Error", new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        var evaluation = new Data.General.Evaluation
        {
            EvaluationTypeId = (int)EvaluationTypeEnum.StudentCollege,
            InsertedDate = DateTime.Now,
            InsertedFrom = user.Id
        };
        db.Evaluation.Add(evaluation);
        await db.SaveChangesAsync();

        db.EvaluationStudentsCollege.Add(new EvaluationStudentsCollege
        {
            EvaluationId = evaluation.EvaluationId,
            StudentsNo = create.NumberOfStudents,
            Title = create.Title,
            Description = create.Description,
            InsertedDate = DateTime.Now,
            InsertedFrom = user.Id
        });

        db.EvaluationStatus.Add(new EvaluationStatus
        {
            EvaluationId = evaluation.EvaluationId,
            StatusTypeId = (int)StatusTypeEnum.Unprocessed,
            InsertedDate = DateTime.Now,
            InsertedFrom = user.Id
        });
        await db.SaveChangesAsync();

        return RedirectToAction(nameof(Questions), new { ide = CryptoSecurity.Encrypt(evaluation.EvaluationId), method = MethodType.Post });
    }

    #endregion

    #region 2. Questions

    #region => List

    [HttpGet, Authorize(Policy = "71q:r")]
    [Description("Arb Tahiri", "Form to display questionnaire form. Second step of registering/editing questionnaire.")]
    public async Task<IActionResult> Questions(string ide, MethodType method)
    {
        var numericals = await db.EvaluationQuestionnaireNumerical
            .Where(a => a.Active
                && a.Evaluation.EvaluationStatus.Any(a => a.StatusTypeId != (int)StatusTypeEnum.Deleted)
                && a.EvaluationId == CryptoSecurity.Decrypt<int>(ide))
            .Select(a => new QuestionNumerical
            {
                NumericalId = a.EvaluationQuestionnaireNumericalId * 6,
                EvaluationQuestionnaireNumericalIde = CryptoSecurity.Encrypt(a.EvaluationQuestionnaireNumericalId),
                Question = user.Language == LanguageEnum.Albanian ? a.QuestionSq : a.QuestionSq,
                Grade = a.Grade,
                Graded = a.Grade.HasValue
            }).ToListAsync();

        var optionalTopics = await db.EvaluationQuestionnaireOptional
            .Where(a => a.Active
                && a.Evaluation.EvaluationStatus.Any(a => a.StatusTypeId != (int)StatusTypeEnum.Deleted)
                && a.EvaluationId == CryptoSecurity.Decrypt<int>(ide))
            .Select(a => new QuestionTopic
            {
                EvaluationQuestionnaireOptionalIde = CryptoSecurity.Encrypt(a.EvaluationQuestionnaireOptionalId),
                Question = user.Language == LanguageEnum.Albanian ? a.QuestionSq : a.QuestionSq,
                Answers = a.EvaluationQuestionnaireOptionalTopic.Where(a => a.Active).Select(a => new TopicAnswer
                {
                    TopicId = a.EvaluationQuestionnaireOptionalTopicId * 6,
                    TopicIde = CryptoSecurity.Encrypt(a.EvaluationQuestionnaireOptionalTopicId),
                    Answer = a.TopicTitle
                }).ToList()
            }).ToListAsync();

        var questionVM = new QuestionVM
        {
            EvaluationIde = ide,
            Numericals = numericals,
            OptionalTopics = optionalTopics,
            TotalQuestions = numericals.Count + optionalTopics.Count,
            Method = method
        };
        return View(questionVM);
    }

    #endregion

    #region => Create

    [Authorize(Policy = "71q:c"), Description("Arb Tahiri", "Form to add a question.")]
    public IActionResult _AddQuestion(string ide) => PartialView(new ManageQuestion { EvaluationIde = ide });

    [HttpPost, Authorize(Policy = "71q:c"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Action to add a question.")]
    public async Task<IActionResult> AddQuestion(ManageQuestion create)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        if (create.QuestionTypeId == (int)QuestionType.Optional)
        {
            db.EvaluationQuestionnaireOptional.Add(new EvaluationQuestionnaireOptional
            {
                EvaluationId = CryptoSecurity.Decrypt<int>(create.EvaluationIde),
                QuestionSq = create.QuestionSQ,
                QuestionEn = create.QuestionEN,
                Active = true,
                InsertedDate = DateTime.Now,
                InsertedFrom = user.Id
            });
        }
        else
        {
            db.EvaluationQuestionnaireNumerical.Add(new EvaluationQuestionnaireNumerical
            {
                EvaluationId = CryptoSecurity.Decrypt<int>(create.EvaluationIde),
                QuestionSq = create.QuestionSQ,
                QuestionEn = create.QuestionEN,
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

    [Authorize(Policy = "71q:e"), Description("Arb Tahiri", "Form to edit a question.")]
    public async Task<IActionResult> _EditQuestion(string ide, QuestionType questionType)
    {
        var question = new ManageQuestion();
        if (questionType == QuestionType.Numerical)
        {
            question = await db.EvaluationQuestionnaireNumerical
                .Where(a => a.Active && a.EvaluationQuestionnaireNumericalId == CryptoSecurity.Decrypt<int>(ide))
                .Select(a => new ManageQuestion
                {
                    EvaluationQuestionnaireNumericalIde = ide,
                    QuestionSQ = a.QuestionSq,
                    QuestionEN = a.QuestionEn,
                    QuestionTypeEnum = questionType
                }).FirstOrDefaultAsync();
        }
        else
        {
            question = await db.EvaluationQuestionnaireOptional
                .Where(a => a.Active && a.EvaluationQuestionnaireOptionalId == CryptoSecurity.Decrypt<int>(ide))
                .Select(a => new ManageQuestion
                {
                    EvaluationQuestionnaireOptionalIde = ide,
                    QuestionSQ = a.QuestionSq,
                    QuestionEN = a.QuestionEn,
                    QuestionTypeEnum = questionType
                }).FirstOrDefaultAsync();
        }

        return PartialView(question);
    }

    [HttpPost, Authorize(Policy = "71q:e"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Action to edit a question.")]
    public async Task<IActionResult> EditQuestion(ManageQuestion edit)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        if (edit.QuestionTypeEnum == QuestionType.Numerical)
        {
            var question = await db.EvaluationQuestionnaireNumerical.FirstOrDefaultAsync(a => a.Active && a.EvaluationQuestionnaireNumericalId == CryptoSecurity.Decrypt<int>(edit.EvaluationQuestionnaireNumericalIde));
            question.QuestionSq = edit.QuestionSQ;
            question.QuestionEn = edit.QuestionEN;
            question.UpdatedDate = DateTime.Now;
            question.UpdatedFrom = user.Id;
            question.UpdatedNo = question.UpdatedNo.HasValue ? ++question.UpdatedNo : question.UpdatedNo = 1;
        }
        else
        {
            var question = await db.EvaluationQuestionnaireOptional.FirstOrDefaultAsync(a => a.Active && a.EvaluationQuestionnaireOptionalId == CryptoSecurity.Decrypt<int>(edit.EvaluationQuestionnaireOptionalIde));
            question.QuestionSq = edit.QuestionSQ;
            question.QuestionEn = edit.QuestionEN;
            question.UpdatedDate = DateTime.Now;
            question.UpdatedFrom = user.Id;
            question.UpdatedNo = question.UpdatedNo.HasValue ? ++question.UpdatedNo : question.UpdatedNo = 1;
        }

        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataUpdatedSuccessfully });
    }

    #endregion

    #region => Delete

    [HttpPost, Authorize(Policy = "71q:d"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Action to delete a question.")]
    public async Task<IActionResult> DeleteQuestion(string ide, QuestionType questionType)
    {
        if (questionType == QuestionType.Numerical)
        {
            var question = await db.EvaluationQuestionnaireNumerical.FirstOrDefaultAsync(a => a.EvaluationQuestionnaireNumericalId == CryptoSecurity.Decrypt<int>(ide));
            question.Active = false;
            question.UpdatedDate = DateTime.Now;
            question.UpdatedFrom = user.Id;
            question.UpdatedNo = question.UpdatedNo.HasValue ? ++question.UpdatedNo : question.UpdatedNo = 1;
        }
        else
        {
            var question = await db.EvaluationQuestionnaireOptional.FirstOrDefaultAsync(a => a.EvaluationQuestionnaireOptionalId == CryptoSecurity.Decrypt<int>(ide));
            question.Active = false;
            question.UpdatedDate = DateTime.Now;
            question.UpdatedFrom = user.Id;
            question.UpdatedNo = question.UpdatedNo.HasValue ? ++question.UpdatedNo : question.UpdatedNo = 1;
        }

        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataDeletedSuccessfully });
    }

    [HttpPost, Authorize(Policy = "71q:d"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Action to delete a question.")]
    public async Task<IActionResult> ClearQuestion(string ide, QuestionType questionType)
    {
        if (questionType == QuestionType.Numerical)
        {
            var question = await db.EvaluationQuestionnaireNumerical.FirstOrDefaultAsync(a => a.EvaluationQuestionnaireNumericalId == CryptoSecurity.Decrypt<int>(ide));
            question.Grade = null;
            question.UpdatedDate = DateTime.Now;
            question.UpdatedFrom = user.Id;
            question.UpdatedNo = question.UpdatedNo.HasValue ? ++question.UpdatedNo : question.UpdatedNo = 1;
        }
        else
        {
            var question = await db.EvaluationQuestionnaireOptionalOption.FirstOrDefaultAsync(a => a.EvaluationQuestionnaireOptionalOptionId == CryptoSecurity.Decrypt<int>(ide));
            question.Checked = false;
            question.UpdatedDate = DateTime.Now;
            question.UpdatedFrom = user.Id;
            question.UpdatedNo = question.UpdatedNo.HasValue ? ++question.UpdatedNo : question.UpdatedNo = 1;
        }

        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.AnswerClearedSuccessfully });
    }

    #endregion

    #region => Answers

    [HttpPost, Authorize(Policy = "71q:a"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Action to answer a question.")]
    public async Task<IActionResult> NumericalAnswer(string ide, int num)
    {
        var error = new ErrorVM();
        if (await db.EvaluationQuestionnaireNumerical.AnyAsync(a => a.EvaluationQuestionnaireNumericalId == CryptoSecurity.Decrypt<int>(ide) && a.Grade == num))
        {
            var question = await db.EvaluationQuestionnaireNumerical.FirstOrDefaultAsync(a => a.Active && a.EvaluationQuestionnaireNumericalId == CryptoSecurity.Decrypt<int>(ide));
            question.Grade = null;
            question.UpdatedDate = DateTime.Now;
            question.UpdatedFrom = user.Id;
            question.UpdatedNo = question.UpdatedNo.HasValue ? ++question.UpdatedNo : question.UpdatedNo = 1;

            error = new ErrorVM { Status = ErrorStatus.Success, Description = Resource.Cleared };
        }
        else
        {
            var question = await db.EvaluationQuestionnaireNumerical.FirstOrDefaultAsync(a => a.Active && a.EvaluationQuestionnaireNumericalId == CryptoSecurity.Decrypt<int>(ide));
            question.Grade = num;
            question.UpdatedDate = DateTime.Now;
            question.UpdatedFrom = user.Id;
            question.UpdatedNo = question.UpdatedNo.HasValue ? ++question.UpdatedNo : question.UpdatedNo = 1;

            error = new ErrorVM { Status = ErrorStatus.Success, Description = Resource.Evaluated };
        }

        await db.SaveChangesAsync();
        return Json(error);
    }

    [HttpPost, Authorize(Policy = "71q:a"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Action to answer a question.")]
    public async Task<IActionResult> OptionalAnswer(string ide, string txt)
    {
        var error = new ErrorVM();
        if (await db.EvaluationQuestionnaireOptionalOption.AnyAsync(a => a.EvaluationQuestionnaireOptionalOptionId == CryptoSecurity.Decrypt<int>(ide) && a.Checked))
        {
            var question = await db.EvaluationQuestionnaireOptionalOption.FirstOrDefaultAsync(a => a.Active && a.EvaluationQuestionnaireOptionalOptionId == CryptoSecurity.Decrypt<int>(ide));
            question.Checked = false;
            question.UpdatedDate = DateTime.Now;
            question.UpdatedFrom = user.Id;
            question.UpdatedNo = question.UpdatedNo.HasValue ? ++question.UpdatedNo : question.UpdatedNo = 1;

            error = new ErrorVM { Status = ErrorStatus.Success, Description = Resource.Cleared };
        }
        else
        {
            var question = await db.EvaluationQuestionnaireOptionalOption
                .Include(a => a.EvaluationQuestionnaireOptional)
                .FirstOrDefaultAsync(a => a.Active && a.EvaluationQuestionnaireOptionalOptionId == CryptoSecurity.Decrypt<int>(ide));
            question.Checked = true;
            if (!string.IsNullOrEmpty(txt))
            {
                question.EvaluationQuestionnaireOptional.Description = txt;
                question.EvaluationQuestionnaireOptional.UpdatedDate = DateTime.Now;
                question.EvaluationQuestionnaireOptional.UpdatedFrom = user.Id;
                question.EvaluationQuestionnaireOptional.UpdatedNo = question.UpdatedNo.HasValue ? ++question.UpdatedNo : question.UpdatedNo = 1;
            }
            question.UpdatedDate = DateTime.Now;
            question.UpdatedFrom = user.Id;
            question.UpdatedNo = question.UpdatedNo.HasValue ? ++question.UpdatedNo : question.UpdatedNo = 1;

            error = new ErrorVM { Status = ErrorStatus.Success, Description = Resource.Evaluated };
        }

        await db.SaveChangesAsync();
        return Json(error);
    }

    #endregion

    #endregion

    #region 3. Documents

    #region => List

    [HttpGet, Authorize(Policy = "71d:r")]
    [Description("Arb Tahiri", "Form to display list of documents. Third step of registering/editing questionnaire.")]
    public async Task<IActionResult> Documents(string ide, MethodType method)
    {
        var documents = await db.EvaluationDocument
            .Include(a => a.DocumentType)
            .Where(a => a.EvaluationId == CryptoSecurity.Decrypt<int>(ide))
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

        var documentVM = new DocumentVM
        {
            EvaluationIde = ide,
            Documents = documents,
            DocumentCount = documents.Count,
            Method = method
        };
        return View(documentVM);
    }

    #endregion

    #region => Create

    [Authorize(Policy = "71d:c"), Description("Arb Tahiri", "Form to add a document.")]
    public IActionResult _AddDocument(string ide) => PartialView(new ManageDocument { EvaluationIde = ide, FileSize = $"{Convert.ToDecimal(configuration["AppSettings:FileMaxKB"]) / 1024:N1}MB" });

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "71d:c")]
    [Description("Arb Tahiri", "Action to add documents.")]
    public async Task<IActionResult> AddDocument(ManageDocument create)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        string path = await SaveFile(environment, configuration, create.DocumentFile, "StaffDocuments", null);

        db.EvaluationDocument.Add(new EvaluationDocument
        {
            EvaluationId = CryptoSecurity.Decrypt<int>(create.EvaluationIde),
            DocumentTypeId = create.DocumentTypeId,
            Title = create.Title,
            Path = path,
            Description = create.Description,
            Active = true,
            InsertedDate = DateTime.Now,
            InsertedFrom = user.Id
        });

        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataRegisteredSuccessfully });
    }

    #endregion

    #region => Edit

    [HttpGet, Authorize(Policy = "71d:e"), Description("Arb Tahiri", "Form to edit a document.")]
    public async Task<IActionResult> _EditDocument(string ide)
    {
        var question = await db.EvaluationDocument
            .Where(a => a.Active && a.EvaluationDocumentId == CryptoSecurity.Decrypt<int>(ide))
            .Select(a => new ManageDocument
            {
                EvaluationDocumentIde = ide,
                DocumentTypeId = a.DocumentTypeId,
                Title = a.Title,
                Description = a.Description,
                Active = a.Active
            }).FirstOrDefaultAsync();
        return PartialView(question);
    }

    [HttpPost, Authorize(Policy = "71d:e"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Action to edit a document.")]
    public async Task<IActionResult> EditDocument(ManageDocument edit)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        var document = await db.EvaluationDocument.FirstOrDefaultAsync(a => a.EvaluationDocumentId == CryptoSecurity.Decrypt<int>(edit.EvaluationDocumentIde));
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

    [HttpPost, Authorize(Policy = "71d:d"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Action to delete a document.")]
    public async Task<IActionResult> DeleteDocument(string ide)
    {
        var document = await db.EvaluationDocument.FirstOrDefaultAsync(a => a.EvaluationDocumentId == CryptoSecurity.Decrypt<int>(ide));
        document.Active = false;
        document.UpdatedDate = DateTime.Now;
        document.UpdatedFrom = user.Id;
        document.UpdatedNo = document.UpdatedNo.HasValue ? ++document.UpdatedNo : document.UpdatedNo = 1;

        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataDeletedSuccessfully });
    }

    #endregion

    #endregion

    #region Finish

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "71f:c")]
    [Description("Arb Tahiri", "Action to add finished status in staff registration.")]
    public async Task<IActionResult> Finish(string ide, MethodType method)
    {
        if (string.IsNullOrEmpty(ide))
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        var checkIfAnswered = await db.EvaluationQuestionnaireNumerical.AnyAsync(a => a.EvaluationId == CryptoSecurity.Decrypt<int>(ide) && !a.Grade.HasValue)
            && await db.EvaluationQuestionnaireOptionalTopic.AnyAsync(a => a.EvaluationQuestionnaireOptional.EvaluationId == CryptoSecurity.Decrypt<int>(ide) && !string.IsNullOrWhiteSpace(a.TopicTitle));

        if (method == MethodType.Post)
        {
            db.EvaluationStatus.Add(new EvaluationStatus
            {
                EvaluationId = CryptoSecurity.Decrypt<int>(ide),
                StatusTypeId = checkIfAnswered ? (int)StatusTypeEnum.PendingForAnswers : (int)StatusTypeEnum.Finished,
                InsertedDate = DateTime.Now,
                InsertedFrom = user.Id
            });
            await db.SaveChangesAsync();
        }

        TempData.Set("Error", new ErrorVM { Status = ErrorStatus.Success, Title = Resource.Success, Description = method == MethodType.Post ? Resource.DataRegisteredSuccessfully : Resource.DataUpdatedSuccessfully });
        return RedirectToAction(nameof(Index));
    }

    #endregion
}
