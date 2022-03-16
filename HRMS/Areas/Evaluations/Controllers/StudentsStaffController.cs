using HRMS.Areas.Evaluations.Models.StudentsStaff;
using HRMS.Controllers;
using HRMS.Data.Core;
using HRMS.Data.General;
using HRMS.Models;
using HRMS.Models.Shared;
using HRMS.Resources;
using HRMS.Utilities;
using HRMS.Utilities.General;
using HRMS.Utilities.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.Areas.Evaluation.Controllers;

[Authorize]
[Route("/{area}/{controller}/{action}")]
public class StudentsStaffController : BaseController
{
    private readonly IConfiguration configuration;
    private readonly IWebHostEnvironment environment;

    public StudentsStaffController(IConfiguration configuration, IWebHostEnvironment environment,
        HRMSContext db, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        : base(db, signInManager, userManager)
    {
        this.configuration = configuration;
        this.environment = environment;
    }

    #region 1. Evaluation data

    [HttpGet, Authorize(Policy = "74:c")]
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

        var data = await db.EvaluationStudentsStaff.Include(a => a.Evaluation).ThenInclude(a => a.EvaluationType)
            .Where(a => a.Evaluation.EvaluationStatus.Any(a => a.StatusTypeId != (int)Status.Deleted)
                && a.EvaluationId == CryptoSecurity.Decrypt<int>(ide))
            .Select(a => new Register
            {
                EvaluationIde = ide,
                EvaluationType = user.Language == LanguageEnum.Albanian ? a.Evaluation.EvaluationType.NameSq : a.Evaluation.EvaluationType.NameEn,
                InsertedDate = a.InsertedDate,
                MethodType = MethodType.Put,
                NumberOfStudents = a.StudentsNo,
                SubjectId = a.StaffDepartmentSubject.SubjectId,
                StaffDepartmentSubjectId = a.StaffDepartmentSubjectId,
                Title = a.Title,
                Description = a.Description
            }).FirstOrDefaultAsync();
        return View(data);
    }

    [HttpPost, Authorize(Policy = "74:c"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Action to register new manager evaluation.")]
    public async Task<IActionResult> Register(Register create)
    {
        if (!ModelState.IsValid)
        {
            TempData.Set("Error", new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        var evaluationId = !string.IsNullOrEmpty(create.EvaluationIde) ? CryptoSecurity.Decrypt<int>(create.EvaluationIde) : 0;
        string evaluationIde;

        var evaluationStudentsStaff = await db.EvaluationStudentsStaff.FirstOrDefaultAsync(a => a.EvaluationId == evaluationId);
        if (evaluationStudentsStaff is null)
        {
            var evaluation = new Data.General.Evaluation
            {
                EvaluationTypeId = (int)EvaluationTypeEnum.StudentStaff,
                InsertedDate = DateTime.Now,
                InsertedFrom = user.Id
            };
            db.Evaluation.Add(evaluation);
            await db.SaveChangesAsync();

            db.EvaluationStudentsStaff.Add(new EvaluationStudentsStaff
            {
                EvaluationId = evaluation.EvaluationId,
                StudentsNo = create.NumberOfStudents,
                StaffDepartmentSubjectId = create.StaffDepartmentSubjectId,
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
            evaluationIde = CryptoSecurity.Encrypt(evaluationStudentsStaff.EvaluationId);

            evaluationStudentsStaff.StudentsNo = create.NumberOfStudents;
            evaluationStudentsStaff.StaffDepartmentSubjectId = create.StaffDepartmentSubjectId;
            evaluationStudentsStaff.Title = create.Title;
            evaluationStudentsStaff.Description = create.Description;
            evaluationStudentsStaff.UpdatedDate = DateTime.Now;
            evaluationStudentsStaff.UpdatedFrom = user.Id;
            evaluationStudentsStaff.UpdatedNo = UpdateNo(evaluationStudentsStaff.UpdatedNo);
        }
        await db.SaveChangesAsync();

        return RedirectToAction(nameof(Questions), new { ide = evaluationIde });
    }

    #endregion

    #region 2. Questions

    #region => List

    [HttpGet, Authorize(Policy = "74q:r")]
    [Description("Arb Tahiri", "Form to display questionnaire form. Second step of registering/editing questionnaire.")]
    public async Task<IActionResult> Questions(string ide)
    {
        var evaluationId = CryptoSecurity.Decrypt<int>(ide);
        var details = await db.EvaluationStudentsStaff
            .Include(a => a.StaffDepartmentSubject).ThenInclude(a => a.StaffDepartment).ThenInclude(a => a.Staff)
            .Include(a => a.StaffDepartmentSubject).ThenInclude(a => a.Subject)
            .Where(a => a.Evaluation.EvaluationStatus.Any(a => a.StatusTypeId != (int)Status.Deleted)
                && a.EvaluationId == evaluationId)
            .Select(a => new Details
            {
                EvaluationIde = ide,
                Firstname = a.StaffDepartmentSubject.StaffDepartment.Staff.FirstName,
                Lastname = a.StaffDepartmentSubject.StaffDepartment.Staff.LastName,
                Subject = user.Language == LanguageEnum.Albanian ? a.StaffDepartmentSubject.Subject.NameSq : a.StaffDepartmentSubject.Subject.NameEn
            }).FirstOrDefaultAsync();

        var numerals = await db.EvaluationQuestionnaireNumerical
            .Where(a => a.Active
                && a.Evaluation.EvaluationStatus.Any(a => a.StatusTypeId != (int)Status.Deleted)
                && a.EvaluationId == evaluationId)
            .Select(a => new QuestionNumerical
            {
                NumericalId = a.EvaluationQuestionnaireNumericalId * 6,
                EvaluationQuestionnaireNumericalIde = CryptoSecurity.Encrypt(a.EvaluationQuestionnaireNumericalId),
                Question = user.Language == LanguageEnum.Albanian ? a.QuestionSq : a.QuestionSq,
                Grade = a.Grade,
                Graded = a.Grade.HasValue
            }).ToListAsync();

        var optionals = await db.EvaluationQuestionnaireOptional
            .Where(a => a.Active
                && a.Evaluation.EvaluationStatus.Any(a => a.StatusTypeId != (int)Status.Deleted)
                && a.EvaluationId == evaluationId)
            .Select(a => new QuestionOptional
            {
                OptionalId = a.EvaluationQuestionnaireOptionalId * 6,
                EvaluationQuestionnaireOptionalIde = CryptoSecurity.Encrypt(a.EvaluationQuestionnaireOptionalId),
                Question = user.Language == LanguageEnum.Albanian ? a.QuestionSq : a.QuestionSq,
                OtherDescription = a.Description,
                Options = a.EvaluationQuestionnaireOptionalOption.Select(a => new QuestionOption
                {
                    OptionId = a.EvaluationQuestionnaireOptionalOptionId * 6,
                    OptionIde = CryptoSecurity.Encrypt(a.EvaluationQuestionnaireOptionalOptionId),
                    Option = user.Language == LanguageEnum.Albanian ? a.OptionTitleSq : a.OptionTitleEn,
                    Checked = a.Checked
                }).ToList()
            }).ToListAsync();

        var questionVM = new QuestionVM
        {
            EvaluationDetails = details,
            Numerals = numerals,
            Optionals = optionals,
            TotalQuestions = numerals.Count + optionals.Count,
            MaxQuestionOptions = Convert.ToInt32(configuration["AppSettings:MaxQuestionOptions"]),
            Method = await db.Evaluation.AnyAsync(a => a.EvaluationId == evaluationId && a.EvaluationStatus.Any(a => a.Active && a.StatusTypeId == (int)Status.Finished)) ? MethodType.Put : MethodType.Post
        };
        return View(questionVM);
    }

    #endregion

    #region => Create

    [Authorize(Policy = "74q:c"), Description("Arb Tahiri", "Form to add a question.")]
    public IActionResult _AddQuestion(string ide) => PartialView(new ManageQuestion { EvaluationIde = ide, MaxQuestionOptions = Convert.ToInt32(configuration["AppSettings:MaxQuestionOptions"]) });

    [HttpPost, Authorize(Policy = "74q:c"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Action to add a question.")]
    public async Task<IActionResult> AddQuestion(ManageQuestion create)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        if (create.QuestionTypeId == (int)QuestionType.Optional)
        {
            if (create.Options == null)
            {
                return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
            }

            db.EvaluationQuestionnaireOptional.Add(new EvaluationQuestionnaireOptional
            {
                EvaluationId = CryptoSecurity.Decrypt<int>(create.EvaluationIde),
                QuestionSq = create.QuestionSQ,
                QuestionEn = create.QuestionEN,
                EvaluationQuestionnaireOptionalOption = create.Options.Select(a => new EvaluationQuestionnaireOptionalOption
                {
                    OptionTitleSq = a.TitleSQ,
                    OptionTitleEn = a.TitleEN,
                    Checked = false,
                    Active = true,
                    InsertedDate = DateTime.Now,
                    InsertedFrom = user.Id
                }).ToList(),
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

    [Authorize(Policy = "74q:e"), Description("Arb Tahiri", "Form to edit a question.")]
    public async Task<IActionResult> _EditQuestion(string ide, QuestionType questionType)
    {
        var question = new ManageQuestion();
        if (questionType == QuestionType.Numeral)
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
        else if (questionType == QuestionType.Optional)
        {
            question = await db.EvaluationQuestionnaireOptional
                .Where(a => a.Active && a.EvaluationQuestionnaireOptionalId == CryptoSecurity.Decrypt<int>(ide))
                .Select(a => new ManageQuestion
                {
                    EvaluationQuestionnaireOptionalIde = ide,
                    QuestionSQ = a.QuestionSq,
                    QuestionEN = a.QuestionEn,
                    QuestionTypeEnum = questionType,
                    MaxQuestionOptions = Convert.ToInt32(configuration["AppSettings:MaxQuestionOptions"]),
                    Options = a.EvaluationQuestionnaireOptionalOption.Select(a => new Option
                    {
                        OptionIde = CryptoSecurity.Encrypt(a.EvaluationQuestionnaireOptionalOptionId),
                        TitleSQ = a.OptionTitleSq,
                        TitleEN = a.OptionTitleEn
                    }).ToList()
                }).FirstOrDefaultAsync();
        }

        return PartialView(question);
    }

    [HttpPost, Authorize(Policy = "74q:e"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Action to edit a question.")]
    public async Task<IActionResult> EditQuestion(ManageQuestion edit)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        if (edit.QuestionTypeEnum == QuestionType.Numeral)
        {
            var question = await db.EvaluationQuestionnaireNumerical.FirstOrDefaultAsync(a => a.Active && a.EvaluationQuestionnaireNumericalId == CryptoSecurity.Decrypt<int>(edit.EvaluationQuestionnaireNumericalIde));
            question.QuestionSq = edit.QuestionSQ;
            question.QuestionEn = edit.QuestionEN;
            question.UpdatedDate = DateTime.Now;
            question.UpdatedFrom = user.Id;
            question.UpdatedNo = UpdateNo(question.UpdatedNo);
        }
        else if (edit.QuestionTypeEnum == QuestionType.Optional)
        {
            if (edit.Options == null)
            {
                return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
            }

            var question = await db.EvaluationQuestionnaireOptional
                .Include(a => a.EvaluationQuestionnaireOptionalOption)
                .FirstOrDefaultAsync(a => a.Active && a.EvaluationQuestionnaireOptionalId == CryptoSecurity.Decrypt<int>(edit.EvaluationQuestionnaireOptionalIde));
            question.QuestionSq = edit.QuestionSQ;
            question.QuestionEn = edit.QuestionEN;
            foreach (var option in edit.Options)
            {
                if (string.IsNullOrEmpty(option.OptionIde))
                {
                    db.EvaluationQuestionnaireOptionalOption.Add(new EvaluationQuestionnaireOptionalOption
                    {
                        EvaluationQuestionnaireOptionalId = CryptoSecurity.Decrypt<int>(edit.EvaluationQuestionnaireOptionalIde),
                        OptionTitleSq = option.TitleSQ,
                        OptionTitleEn = option.TitleEN,
                        Checked = false,
                        Active = true,
                        InsertedDate = DateTime.Now,
                        InsertedFrom = user.Id
                    });
                }
                else
                {
                    foreach (var item in question.EvaluationQuestionnaireOptionalOption.Where(a => a.EvaluationQuestionnaireOptionalOptionId == CryptoSecurity.Decrypt<int>(option.OptionIde)))
                    {
                        item.OptionTitleSq = option.TitleSQ;
                        item.OptionTitleEn = option.TitleEN;
                        item.UpdatedDate = DateTime.Now;
                        item.UpdatedFrom = user.Id;
                        item.UpdatedNo = UpdateNo(item.UpdatedNo);
                    }
                }

            }
            question.UpdatedDate = DateTime.Now;
            question.UpdatedFrom = user.Id;
            question.UpdatedNo = UpdateNo(question.UpdatedNo);
        }

        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataUpdatedSuccessfully });
    }

    #endregion

    #region => Delete

    [HttpPost, Authorize(Policy = "74q:d"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Action to delete a question.")]
    public async Task<IActionResult> DeleteQuestion(string ide, QuestionType questionType)
    {
        if (questionType == QuestionType.Numeral)
        {
            var question = await db.EvaluationQuestionnaireNumerical.FirstOrDefaultAsync(a => a.EvaluationQuestionnaireNumericalId == CryptoSecurity.Decrypt<int>(ide));
            question.Active = false;
            question.UpdatedDate = DateTime.Now;
            question.UpdatedFrom = user.Id;
            question.UpdatedNo = UpdateNo(question.UpdatedNo);
            question.UpdatedNo = question.UpdatedNo.HasValue ? ++question.UpdatedNo : question.UpdatedNo = 1;
        }
        else if (questionType == QuestionType.Optional)
        {
            var question = await db.EvaluationQuestionnaireOptional.FirstOrDefaultAsync(a => a.EvaluationQuestionnaireOptionalId == CryptoSecurity.Decrypt<int>(ide));
            question.Active = false;
            question.UpdatedDate = DateTime.Now;
            question.UpdatedFrom = user.Id;
            question.UpdatedNo = UpdateNo(question.UpdatedNo);
            question.UpdatedNo = question.UpdatedNo.HasValue ? ++question.UpdatedNo : question.UpdatedNo = 1;
        }

        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.DataDeletedSuccessfully });
    }

    [HttpPost, Authorize(Policy = "74q:d"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Action to delete a question.")]
    public async Task<IActionResult> ClearQuestion(string ide, QuestionType questionType)
    {
        if (questionType == QuestionType.Numeral)
        {
            var question = await db.EvaluationQuestionnaireNumerical.FirstOrDefaultAsync(a => a.EvaluationQuestionnaireNumericalId == CryptoSecurity.Decrypt<int>(ide));
            question.Grade = null;
            question.UpdatedDate = DateTime.Now;
            question.UpdatedFrom = user.Id;
            question.UpdatedNo = UpdateNo(question.UpdatedNo);
            question.UpdatedNo = question.UpdatedNo.HasValue ? ++question.UpdatedNo : question.UpdatedNo = 1;
        }
        else
        {
            var question = await db.EvaluationQuestionnaireOptionalOption.FirstOrDefaultAsync(a => a.EvaluationQuestionnaireOptionalOptionId == CryptoSecurity.Decrypt<int>(ide));
            question.Checked = false;
            question.UpdatedDate = DateTime.Now;
            question.UpdatedFrom = user.Id;
            question.UpdatedNo = UpdateNo(question.UpdatedNo);
            question.UpdatedNo = question.UpdatedNo.HasValue ? ++question.UpdatedNo : question.UpdatedNo = 1;
        }

        await db.SaveChangesAsync();
        return Json(new ErrorVM { Status = ErrorStatus.Success, Description = Resource.AnswerClearedSuccessfully });
    }

    #endregion

    #region => Answers

    [HttpPost, Authorize(Policy = "74q:a"), ValidateAntiForgeryToken]
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

    [HttpPost, Authorize(Policy = "75q:a"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Action to answer a question.")]
    public async Task<IActionResult> OptionalAnswer(string ide)
    {
        var error = new ErrorVM();
        if (await db.EvaluationQuestionnaireOptionalOption.AnyAsync(a => a.EvaluationQuestionnaireOptionalOptionId == CryptoSecurity.Decrypt<int>(ide) && a.Checked))
        {
            var question = await db.EvaluationQuestionnaireOptionalOption
                .FirstOrDefaultAsync(a => a.Active && a.EvaluationQuestionnaireOptionalOptionId == CryptoSecurity.Decrypt<int>(ide));
            question.Checked = false;
            question.UpdatedDate = DateTime.Now;
            question.UpdatedFrom = user.Id;
            question.UpdatedNo = UpdateNo(question.UpdatedNo);

            error = new ErrorVM { Status = ErrorStatus.Success, Description = Resource.Cleared };
        }
        else
        {
            var question = await db.EvaluationQuestionnaireOptionalOption
                .FirstOrDefaultAsync(a => a.Active && a.EvaluationQuestionnaireOptionalOptionId == CryptoSecurity.Decrypt<int>(ide));
            question.Checked = true;
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

    [HttpGet, Authorize(Policy = "74d:r")]
    [Description("Arb Tahiri", "Form to display list of documents. Third step of registering/editing questionnaire.")]
    public async Task<IActionResult> Documents(string ide)
    {
        var evaluationId = CryptoSecurity.Decrypt<int>(ide);
        var documents = await db.EvaluationDocument
            .Include(a => a.DocumentType)
            .Where(a => a.EvaluationId == evaluationId)
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
            Method = await db.Evaluation.AnyAsync(a => a.EvaluationId == evaluationId && a.EvaluationStatus.Any(a => a.Active && a.StatusTypeId == (int)Status.Finished)) ? MethodType.Put : MethodType.Post
        };
        return View(documentVM);
    }

    #endregion

    #region => Create

    [Authorize(Policy = "74d:c"), Description("Arb Tahiri", "Form to add a document.")]
    public IActionResult _AddDocument(string ide) => PartialView(new ManageDocument { EvaluationIde = ide, FileSize = $"{Convert.ToDecimal(configuration["AppSettings:FileMaxKB"]) / 1024:N1}MB" });

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "74d:c")]
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

    [HttpGet, Authorize(Policy = "74d:e"), Description("Arb Tahiri", "Form to edit a document.")]
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

    [HttpPost, Authorize(Policy = "74d:e"), ValidateAntiForgeryToken]
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

    [HttpPost, Authorize(Policy = "74d:d"), ValidateAntiForgeryToken]
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

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "74f:c")]
    [Description("Arb Tahiri", "Action to add finished status in staff registration.")]
    public async Task<IActionResult> Finish(string ide)
    {
        if (string.IsNullOrEmpty(ide))
        {
            return Json(new ErrorVM { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        var noQuestions = await db.EvaluationQuestionnaireNumerical.CountAsync(a => a.Active && a.EvaluationId == CryptoSecurity.Decrypt<int>(ide))
            + await db.EvaluationQuestionnaireOptional.CountAsync(a => a.Active && a.EvaluationId == CryptoSecurity.Decrypt<int>(ide) && a.EvaluationQuestionnaireOptionalOption.Any(b => b.Active));

        var noAnswers = await db.EvaluationQuestionnaireNumerical.CountAsync(a => a.Active && a.Grade.HasValue && a.EvaluationId == CryptoSecurity.Decrypt<int>(ide))
            + await db.EvaluationQuestionnaireOptional.CountAsync(a => a.Active && a.EvaluationId == CryptoSecurity.Decrypt<int>(ide) && a.EvaluationQuestionnaireOptionalOption.Any(b => b.Active && b.Checked));

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

        TempData.Set("Error", new ErrorVM { Status = ErrorStatus.Success, Title = Resource.Success, Description = finished ? Resource.DataUpdatedSuccessfully : Resource.DataRegisteredSuccessfully });
        return RedirectToAction("Search", "Evaluation");
    }

    #endregion

    #region Select list item

    [Description("Arb Tahiri", "Method to get list of staff depending of subject.")]
    public async Task<List<Select2>> StaffSubjects(int subjectId) =>
        await db.StaffDepartmentSubject.Where(a => a.Active && a.EndDate >= DateTime.Now && a.SubjectId == subjectId)
            .Select(a => new Select2
            {
                id = a.StaffDepartmentSubjectId.ToString(),
                text = $"{a.StaffDepartment.Staff.FirstName} {a.StaffDepartment.Staff.LastName}"
            }).ToListAsync();

    #endregion
}
