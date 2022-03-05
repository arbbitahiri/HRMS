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

    [HttpGet, Authorize(Policy = "73:c")]
    [Description("Arb Tahiri", "Form to register/edit self evaluation. First step of registering/editing questionnaire.")]
    public async Task<IActionResult> Index(string ide)
    {
        if (string.IsNullOrEmpty(ide))
        {
            if (await db.EvaluationStatus.AnyAsync(a => a.Evaluation.EvaluationTypeId == (int)EvaluationTypeEnum.Self && a.StatusTypeId == (int)Status.Processing))
            {
                return Json(new ErrorVM { Status = ErrorStatus.Warning, Title = Resource.Warning, Description = Resource.YouHaveEvaluationProcessing });
            }

            return View(new Register { MethodType = MethodType.Post });
        }

        var data = await db.EvaluationStudentsCollege.Include(a => a.Evaluation).ThenInclude(a => a.EvaluationType)
            .Where(a => a.Evaluation.EvaluationStatus.Any(a => a.StatusTypeId != (int)Status.Deleted)
                && a.EvaluationId == CryptoSecurity.Decrypt<int>(ide))
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

    [HttpPost, Authorize(Policy = "73:c"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Action to register new manager evaluation.")]
    public async Task<IActionResult> Register(Register create)
    {
        if (!ModelState.IsValid)
        {
            TempData.Set("Error", new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        var evaluationId = !string.IsNullOrEmpty(create.EvaluationIde) ? CryptoSecurity.Decrypt<int>(create.EvaluationIde) : 0;
        string evaluationIde;

        var evaluationStudentsCollege = await db.EvaluationStudentsCollege.FirstOrDefaultAsync(a => a.EvaluationId == evaluationId);
        if (evaluationStudentsCollege is null)
        {
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
                StatusTypeId = (int)Status.Unprocessed,
                Active = true,
                InsertedDate = DateTime.Now,
                InsertedFrom = user.Id
            });
            evaluationIde = CryptoSecurity.Encrypt(evaluation.EvaluationId);
        }
        else
        {
            evaluationIde = CryptoSecurity.Encrypt(evaluationStudentsCollege.EvaluationId);

            evaluationStudentsCollege.StudentsNo = create.NumberOfStudents;
            evaluationStudentsCollege.Title = create.Title;
            evaluationStudentsCollege.Description = create.Description;
            evaluationStudentsCollege.UpdatedDate = DateTime.Now;
            evaluationStudentsCollege.UpdatedFrom = user.Id;
            evaluationStudentsCollege.UpdatedNo = UpdateNo(evaluationStudentsCollege.UpdatedNo);
        }
        await db.SaveChangesAsync();

        return RedirectToAction(nameof(Questions), new { ide = evaluationIde, method = MethodType.Post });
    }

    #endregion

    #region 2. Questions

    #region => List

    [HttpGet, Authorize(Policy = "73q:r")]
    [Description("Arb Tahiri", "Form to display questionnaire form. Second step of registering/editing questionnaire.")]
    public async Task<IActionResult> Questions(string ide, MethodType method)
    {
        var numericals = await db.EvaluationQuestionnaireNumerical
            .Where(a => a.Active
                && a.Evaluation.EvaluationStatus.Any(a => a.StatusTypeId != (int)Status.Deleted)
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
                && a.Evaluation.EvaluationStatus.Any(a => a.StatusTypeId != (int)Status.Deleted)
                && a.EvaluationId == CryptoSecurity.Decrypt<int>(ide))
            .Select(a => new QuestionTopic
            {
                EvaluationQuestionnaireOptionalIde = CryptoSecurity.Encrypt(a.EvaluationQuestionnaireOptionalId),
                Question = user.Language == LanguageEnum.Albanian ? a.QuestionSq : a.QuestionSq,
                Answers = a.EvaluationQuestionnaireOptionalTopic.Where(a => a.Active).Select(a => new TopicAnswer
                {
                    TopicId = a.EvaluationQuestionnaireOptionalTopicId * 6,
                    TopicIde = CryptoSecurity.Encrypt(a.EvaluationQuestionnaireOptionalTopicId),
                    Answer = a.Answer
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

    [Authorize(Policy = "73q:c"), Description("Arb Tahiri", "Form to add a question.")]
    public IActionResult _AddQuestion(string ide) => PartialView(new ManageQuestion { EvaluationIde = ide });

    [HttpPost, Authorize(Policy = "73q:c"), ValidateAntiForgeryToken]
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

    [Authorize(Policy = "73q:e"), Description("Arb Tahiri", "Form to edit a question.")]
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

    [HttpPost, Authorize(Policy = "73q:e"), ValidateAntiForgeryToken]
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
            question.UpdatedNo = UpdateNo(question.UpdatedNo);
        }
        else
        {
            var question = await db.EvaluationQuestionnaireOptional.FirstOrDefaultAsync(a => a.Active && a.EvaluationQuestionnaireOptionalId == CryptoSecurity.Decrypt<int>(edit.EvaluationQuestionnaireOptionalIde));
            question.QuestionSq = edit.QuestionSQ;
            question.QuestionEn = edit.QuestionEN;
            question.UpdatedDate = DateTime.Now;
            question.UpdatedFrom = user.Id;
            question.UpdatedNo = UpdateNo(question.UpdatedNo);
        }

        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataUpdatedSuccessfully });
    }

    #endregion

    #region => Delete

    [HttpPost, Authorize(Policy = "73q:d"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Action to delete a question.")]
    public async Task<IActionResult> DeleteQuestion(string ide, QuestionType questionType)
    {
        if (questionType == QuestionType.Numerical)
        {
            var question = await db.EvaluationQuestionnaireNumerical.FirstOrDefaultAsync(a => a.EvaluationQuestionnaireNumericalId == CryptoSecurity.Decrypt<int>(ide));
            question.Active = false;
            question.UpdatedDate = DateTime.Now;
            question.UpdatedFrom = user.Id;
            question.UpdatedNo = UpdateNo(question.UpdatedNo);
        }
        else
        {
            var question = await db.EvaluationQuestionnaireOptional.FirstOrDefaultAsync(a => a.EvaluationQuestionnaireOptionalId == CryptoSecurity.Decrypt<int>(ide));
            question.Active = false;
            question.UpdatedDate = DateTime.Now;
            question.UpdatedFrom = user.Id;
            question.UpdatedNo = UpdateNo(question.UpdatedNo);
        }

        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataDeletedSuccessfully });
    }

    [HttpPost, Authorize(Policy = "73q:d"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Action to delete a question.")]
    public async Task<IActionResult> ClearQuestion(string ide, QuestionType questionType)
    {
        if (questionType == QuestionType.Numerical)
        {
            var question = await db.EvaluationQuestionnaireNumerical.FirstOrDefaultAsync(a => a.EvaluationQuestionnaireNumericalId == CryptoSecurity.Decrypt<int>(ide));
            question.Grade = null;
            question.UpdatedDate = DateTime.Now;
            question.UpdatedFrom = user.Id;
            question.UpdatedNo = UpdateNo(question.UpdatedNo);
        }
        else
        {
            var question = await db.EvaluationQuestionnaireOptionalOption.FirstOrDefaultAsync(a => a.EvaluationQuestionnaireOptionalOptionId == CryptoSecurity.Decrypt<int>(ide));
            question.Checked = false;
            question.UpdatedDate = DateTime.Now;
            question.UpdatedFrom = user.Id;
            question.UpdatedNo = UpdateNo(question.UpdatedNo);
        }

        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.AnswerClearedSuccessfully });
    }

    #endregion

    #region => Answers

    [HttpPost, Authorize(Policy = "73q:a"), ValidateAntiForgeryToken]
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
            question.UpdatedNo = UpdateNo(question.UpdatedNo);

            error = new ErrorVM { Status = ErrorStatus.Success, Description = Resource.Cleared };
        }
        else
        {
            var question = await db.EvaluationQuestionnaireNumerical.FirstOrDefaultAsync(a => a.Active && a.EvaluationQuestionnaireNumericalId == CryptoSecurity.Decrypt<int>(ide));
            question.Grade = num;
            question.UpdatedDate = DateTime.Now;
            question.UpdatedFrom = user.Id;
            question.UpdatedNo = UpdateNo(question.UpdatedNo);

            error = new ErrorVM { Status = ErrorStatus.Success, Description = Resource.Evaluated };
        }

        await db.SaveChangesAsync();
        return Json(error);
    }

    [HttpPost, Authorize(Policy = "73q:a"), ValidateAntiForgeryToken]
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
            question.UpdatedNo = UpdateNo(question.UpdatedNo);

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
                question.EvaluationQuestionnaireOptional.UpdatedNo = UpdateNo(question.EvaluationQuestionnaireOptional.UpdatedNo);
            }
            question.UpdatedDate = DateTime.Now;
            question.UpdatedFrom = user.Id;
            question.UpdatedNo = UpdateNo(question.UpdatedNo);

            error = new ErrorVM { Status = ErrorStatus.Success, Description = Resource.Evaluated };
        }
        await db.SaveChangesAsync();
        return Json(error);
    }

    #endregion

    #endregion

    #region 3. Documents

    #region => List

    [HttpGet, Authorize(Policy = "73d:r")]
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

    [Authorize(Policy = "73d:c"), Description("Arb Tahiri", "Form to add a document.")]
    public IActionResult _AddDocument(string ide) => PartialView(new ManageDocument { EvaluationIde = ide, FileSize = $"{Convert.ToDecimal(configuration["AppSettings:FileMaxKB"]) / 1024:N1}MB" });

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "73d:c")]
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

    [HttpGet, Authorize(Policy = "73d:e"), Description("Arb Tahiri", "Form to edit a document.")]
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

    [HttpPost, Authorize(Policy = "73d:e"), ValidateAntiForgeryToken]
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
        document.UpdatedNo = UpdateNo(document.UpdatedNo);

        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataUpdatedSuccessfully });
    }

    #endregion

    #region => Delete

    [HttpPost, Authorize(Policy = "73d:d"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Action to delete a document.")]
    public async Task<IActionResult> DeleteDocument(string ide)
    {
        var document = await db.EvaluationDocument.FirstOrDefaultAsync(a => a.EvaluationDocumentId == CryptoSecurity.Decrypt<int>(ide));
        document.Active = false;
        document.UpdatedDate = DateTime.Now;
        document.UpdatedFrom = user.Id;
        document.UpdatedNo = UpdateNo(document.UpdatedNo);

        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataDeletedSuccessfully });
    }

    #endregion

    #endregion

    #region Finish

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "73f:c")]
    [Description("Arb Tahiri", "Action to add finished status in staff registration.")]
    public async Task<IActionResult> Finish(string ide, MethodType method)
    {
        if (string.IsNullOrEmpty(ide))
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        var noQuestions = await db.EvaluationQuestionnaireNumerical.CountAsync(a => a.Active && a.EvaluationId == CryptoSecurity.Decrypt<int>(ide))
            + await db.EvaluationQuestionnaireOptional.CountAsync(a => a.Active && a.EvaluationId == CryptoSecurity.Decrypt<int>(ide) && a.EvaluationQuestionnaireOptionalTopic.Any(b => b.Active));

        var noAnswers = await db.EvaluationQuestionnaireNumerical.CountAsync(a => a.Active && a.Grade.HasValue && a.EvaluationId == CryptoSecurity.Decrypt<int>(ide))
            + await db.EvaluationQuestionnaireOptional.CountAsync(a => a.Active && a.EvaluationId == CryptoSecurity.Decrypt<int>(ide) && a.EvaluationQuestionnaireOptionalTopic.Any(b => b.Active && !string.IsNullOrEmpty(b.Answer)));

        var finished = noQuestions == noAnswers;

        var evaluationStatus = await db.EvaluationStatus.FirstOrDefaultAsync(a => a.Active && a.EvaluationId == CryptoSecurity.Decrypt<int>(ide));
        evaluationStatus.Active = false;
        evaluationStatus.UpdatedDate = DateTime.Now;
        evaluationStatus.UpdatedFrom = user.Id;
        evaluationStatus.UpdatedNo = UpdateNo(evaluationStatus.UpdatedNo);

        db.EvaluationStatus.Add(new EvaluationStatus
        {
            EvaluationId = CryptoSecurity.Decrypt<int>(ide),
            StatusTypeId = finished ? (int)Status.Finished : (int)Status.PendingForAnswers,
            Active = true,
            InsertedDate = DateTime.Now,
            InsertedFrom = user.Id
        });

        TempData.Set("Error", new ErrorVM { Status = ErrorStatus.Success, Title = Resource.Success, Description = method == MethodType.Post ? Resource.DataRegisteredSuccessfully : Resource.DataUpdatedSuccessfully });
        return RedirectToAction("Index", "Evaluation");
    }

    #endregion
}
