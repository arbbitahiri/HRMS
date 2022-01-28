using HRMS.Data.Core;
using HRMS.Data.General;
using HRMS.Models;
using HRMS.Models.Tables;
using HRMS.Resources;
using HRMS.Utilities;
using HRMS.Utilities.General;
using HRMS.Utilities.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.Controllers;
[Authorize(Policy = "16:r")]
public class TablesController : BaseController
{
    public TablesController(HRMS_WorkContext db, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        : base(db, signInManager, userManager)
    {
    }

    #region Table list

    [Authorize(Policy = "16:r"), Description("Arb Tahiri", "Entry form.")]
    public IActionResult Index() => View();

    [Authorize(Policy = "16:r"), Description("Arb Tahiri", "List of lookup tables.")]
    public IActionResult _LookUpTables()
    {
        var tables = new List<TableName>()
        {
            new TableName() { Table = LookUpTable.Document, Title = Resource.DocumentType },
            new TableName() { Table = LookUpTable.EducationLevel, Title = Resource.EducationLevelType },
            new TableName() { Table = LookUpTable.Evaluation, Title = Resource.EvaluationType },
            new TableName() { Table = LookUpTable.Holiday, Title = Resource.HolidayType },
            new TableName() { Table = LookUpTable.Profession, Title = Resource.ProfessionType },
            new TableName() { Table = LookUpTable.Staff, Title = Resource.StaffType },
            new TableName() { Table = LookUpTable.Status, Title = Resource.StatusType },
            new TableName() { Table = LookUpTable.Department, Title = Resource.Department },
            new TableName() { Table = LookUpTable.EvaluationQuestion, Title = Resource.EvaluationQuestionType }
        };

        return PartialView(tables);
    }

    #endregion

    #region Table data

    [Authorize(Policy = "16:r"), Description("Arb Tahiri", "List of data of look up tables")]
    public async Task<IActionResult> _LookUpData(LookUpTable table, string title)
    {
        switch (table)
        {
            case LookUpTable.Document:
                var dataDocumentType = await db.DocumentType.Select(a => new DataList
                {
                    Ide = CryptoSecurity.Encrypt(a.DocumentTypeId),
                    NameSQ = a.NameSq,
                    NameEN = a.NameEn,
                    Active = a.Active
                }).ToListAsync();
                return PartialView(new TableData() { DataList = dataDocumentType, Table = table, Title = title });
            case LookUpTable.EducationLevel:
                var dataEducationLevelType = await db.EducationLevelType.Select(a => new DataList
                {
                    Ide = CryptoSecurity.Encrypt(a.EducationLevelTypeId),
                    NameSQ = a.NameSq,
                    NameEN = a.NameEn,
                    Active = a.Active
                }).ToListAsync();
                return PartialView(new TableData() { DataList = dataEducationLevelType, Table = table, Title = title });
            case LookUpTable.Evaluation:
                var dataEvaluationType = await db.EvaluationType.Select(a => new DataList
                {
                    Ide = CryptoSecurity.Encrypt(a.EvaluationTypeId),
                    NameSQ = a.NameSq,
                    NameEN = a.NameEn,
                    Active = a.Active
                }).ToListAsync();
                return PartialView(new TableData() { DataList = dataEvaluationType, Table = table, Title = title });
            case LookUpTable.Holiday:
                var dataHolidayType = await db.HolidayType.Select(a => new DataList
                {
                    Ide = CryptoSecurity.Encrypt(a.HolidayTypeId),
                    NameSQ = a.NameSq,
                    NameEN = a.NameEn,
                    Active = true
                }).ToListAsync();
                return PartialView(new TableData() { DataList = dataHolidayType, Table = table, Title = title });
            case LookUpTable.Profession:
                var dataProfessionType = await db.ProfessionType.Select(a => new DataList
                {
                    Ide = CryptoSecurity.Encrypt(a.ProfessionTypeId),
                    NameSQ = a.NameSq,
                    NameEN = a.NameEn,
                    OtherData = a.Code,
                    Active = a.Active
                }).ToListAsync();
                return PartialView(new TableData() { DataList = dataProfessionType, Table = table, Title = title });
            case LookUpTable.Staff:
                var dataStaffType = await db.StaffType.Select(a => new DataList
                {
                    Ide = CryptoSecurity.Encrypt(a.StaffTypeId),
                    NameSQ = a.NameSq,
                    NameEN = a.NameEn,
                    Active = true
                }).ToListAsync();
                return PartialView(new TableData() { DataList = dataStaffType, Table = table, Title = title });
            case LookUpTable.Status:
                var dataStatusType = await db.StatusType.Select(a => new DataList
                {
                    Ide = CryptoSecurity.Encrypt(a.StatusTypeId),
                    NameSQ = a.NameSq,
                    NameEN = a.NameEn,
                    Active = a.Active
                }).ToListAsync();
                return PartialView(new TableData() { DataList = dataStatusType, Table = table, Title = title });
            case LookUpTable.Department:
                var dataDepartment = await db.Department.Select(a => new DataList
                {
                    Ide = CryptoSecurity.Encrypt(a.DepartmentId),
                    OtherData = a.Code,
                    NameSQ = a.NameSq,
                    NameEN = a.NameEn,
                    Active = a.Active
                }).ToListAsync();
                return PartialView(new TableData() { DataList = dataDepartment, Table = table, Title = title });
            case LookUpTable.EvaluationQuestion:
                var dataQuestion = await db.EvaluationQuestionType.Select(a => new DataList
                {
                    Ide = CryptoSecurity.Encrypt(a.EvaluationQuestionTypeId),
                    NameSQ = a.NameSq,
                    NameEN = a.NameEn,
                    Active = (bool)a.Active
                }).ToListAsync();
                return PartialView(new TableData() { DataList = dataQuestion, Table = table, Title = title });
            default:
                var dataDefault = await db.DocumentType.Select(a => new DataList
                {
                    Ide = CryptoSecurity.Encrypt(a.DocumentTypeId),
                    NameSQ = a.NameSq,
                    NameEN = a.NameEn,
                    Active = a.Active
                }).ToListAsync();
                return PartialView(new TableData() { DataList = dataDefault, Table = table, Title = title });
        }
    }

    #endregion

    #region Create

    [Authorize(Policy = "16:c"), Description("Arb Tahiri", "Form to create data for a look up table.")]
    public IActionResult _Create(LookUpTable table, string title) => PartialView(new CreateData { Table = table, Title = title });

    [HttpPost, Authorize(Policy = "16:c"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Action to create data for a look up table.")]
    public async Task<IActionResult> Create(CreateData create)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM() { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        var error = new ErrorVM() { Status = ErrorStatus.Success, Description = Resource.DataRegisteredSuccessfully };

        switch (create.Table)
        {
            case LookUpTable.Document:
                db.DocumentType.Add(new DocumentType
                {
                    NameSq = create.NameSQ,
                    NameEn = create.NameEN,
                    Active = true,
                    InsertedDate = DateTime.Now,
                    InsertedFrom = user.Id
                });
                await db.SaveChangesAsync();
                return Json(error);
            case LookUpTable.EducationLevel:
                db.EducationLevelType.Add(new EducationLevelType
                {
                    NameSq = create.NameSQ,
                    NameEn = create.NameEN,
                    Active = true,
                    InsertedDate = DateTime.Now,
                    InsertedFrom = user.Id
                });
                await db.SaveChangesAsync();
                return Json(error);
            case LookUpTable.Evaluation:
                db.EvaluationType.Add(new EvaluationType
                {
                    NameSq = create.NameSQ,
                    NameEn = create.NameEN,
                    Active = true,
                    InsertedDate = DateTime.Now,
                    InsertedFrom = user.Id
                });
                await db.SaveChangesAsync();
                return Json(error);
            case LookUpTable.Holiday:
                db.HolidayType.Add(new HolidayType
                {
                    NameSq = create.NameSQ,
                    NameEn = create.NameEN,
                    Active = true,
                    InsertedDate = DateTime.Now,
                    InsertedFrom = user.Id
                });
                await db.SaveChangesAsync();
                return Json(error);
            case LookUpTable.Profession:
                db.ProfessionType.Add(new ProfessionType
                {
                    NameSq = create.NameSQ,
                    NameEn = create.NameEN,
                    Code = create.OtherData,
                    Active = true,
                    InsertedDate = DateTime.Now,
                    InsertedFrom = user.Id
                });
                await db.SaveChangesAsync();
                return Json(error);
            case LookUpTable.Staff:
                db.StaffType.Add(new StaffType
                {
                    NameSq = create.NameSQ,
                    NameEn = create.NameEN,
                    Active = true,
                    InsertedDate = DateTime.Now,
                    InsertedFrom = user.Id
                });
                await db.SaveChangesAsync();
                return Json(error);
            case LookUpTable.Status:
                db.StatusType.Add(new StatusType
                {
                    NameSq = create.NameSQ,
                    NameEn = create.NameEN,
                    Active = true,
                    InsertedDate = DateTime.Now,
                    InsertedFrom = user.Id
                });
                await db.SaveChangesAsync();
                return Json(error);
            case LookUpTable.Department:
                db.Department.Add(new Department
                {
                    NameSq = create.NameSQ,
                    NameEn = create.NameEN,
                    Code = create.OtherData,
                    Active = true,
                    InsertedDate = DateTime.Now,
                    InsertedFrom = user.Id
                });
                await db.SaveChangesAsync();
                return Json(error);
            case LookUpTable.EvaluationQuestion:
                db.EvaluationQuestionType.Add(new EvaluationQuestionType
                {
                    NameSq = create.NameSQ,
                    NameEn = create.NameEN,
                    Active = true,
                    InsertedDate = DateTime.Now,
                    InsertedFrom = user.Id
                });
                await db.SaveChangesAsync();
                return Json(error);
            default:
                return Json(new ErrorVM() { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }
    }

    #endregion

    #region Edit

    [Authorize(Policy = "16:e"), Description("Arb Tahiri", "Form to edit data from look up tables.")]
    public async Task<IActionResult> _Edit(LookUpTable table, string title, string ide)
    {
        var id = CryptoSecurity.Decrypt<int>(ide);
        switch (table)
        {
            case LookUpTable.Document:
                var dataDocumentType = await db.DocumentType
                    .Where(a => a.DocumentTypeId == id)
                    .Select(a => new CreateData
                    {
                        Ide = CryptoSecurity.Encrypt(a.DocumentTypeId),
                        NameSQ = a.NameSq,
                        NameEN = a.NameEn,
                        Title = title
                    }).FirstOrDefaultAsync();
                return PartialView(dataDocumentType);
            case LookUpTable.EducationLevel:
                var dataEducationLevelType = await db.EducationLevelType
                    .Where(a => a.EducationLevelTypeId == id)
                    .Select(a => new CreateData
                    {
                        Ide = CryptoSecurity.Encrypt(a.EducationLevelTypeId),
                        NameSQ = a.NameSq,
                        NameEN = a.NameEn,
                        Title = title
                    }).FirstOrDefaultAsync();
                return PartialView(dataEducationLevelType);
            case LookUpTable.Evaluation:
                var dataEvaluationType = await db.EvaluationType
                    .Where(a => a.EvaluationTypeId == id)
                    .Select(a => new CreateData
                    {
                        Ide = CryptoSecurity.Encrypt(a.EvaluationTypeId),
                        NameSQ = a.NameSq,
                        NameEN = a.NameEn,
                        Title = title
                    }).FirstOrDefaultAsync();
                return PartialView(dataEvaluationType);
            case LookUpTable.Holiday:
                var dataHolidayType = await db.HolidayType
                    .Where(a => a.HolidayTypeId == id)
                    .Select(a => new CreateData
                    {
                        Ide = CryptoSecurity.Encrypt(a.HolidayTypeId),
                        NameSQ = a.NameSq,
                        NameEN = a.NameEn,
                        Title = title
                    }).FirstOrDefaultAsync();
                return PartialView(dataHolidayType);
            case LookUpTable.Profession:
                var dataProfessionType = await db.ProfessionType
                    .Where(a => a.ProfessionTypeId == id)
                    .Select(a => new CreateData
                    {
                        Ide = CryptoSecurity.Encrypt(a.ProfessionTypeId),
                        NameSQ = a.NameSq,
                        NameEN = a.NameEn,
                        OtherData = a.Code,
                        Title = title
                    }).FirstOrDefaultAsync();
                return PartialView(dataProfessionType);
            case LookUpTable.Staff:
                var dataStaffType = await db.StaffType
                    .Where(a => a.StaffTypeId == id)
                    .Select(a => new CreateData
                    {
                        Ide = CryptoSecurity.Encrypt(a.StaffTypeId),
                        NameSQ = a.NameSq,
                        NameEN = a.NameEn,
                        Title = title
                    }).FirstOrDefaultAsync();
                return PartialView(dataStaffType);
            case LookUpTable.Status:
                var dataStatusType = await db.StatusType
                    .Where(a => a.StatusTypeId == id)
                    .Select(a => new CreateData
                    {
                        Ide = CryptoSecurity.Encrypt(a.StatusTypeId),
                        NameSQ = a.NameSq,
                        NameEN = a.NameEn,
                        Title = title
                    }).FirstOrDefaultAsync();
                return PartialView(dataStatusType);
            case LookUpTable.Department:
                var dataDepartment = await db.Department
                    .Where(a => a.DepartmentId == id)
                    .Select(a => new CreateData
                    {
                        Ide = CryptoSecurity.Encrypt(a.DepartmentId),
                        OtherData = a.Code,
                        NameSQ = a.NameSq,
                        NameEN = a.NameEn,
                        Title = title
                    }).FirstOrDefaultAsync();
                return PartialView(dataDepartment);
            case LookUpTable.EvaluationQuestion:
                var dataQuestion = await db.EvaluationQuestionType
                    .Where(a => a.EvaluationQuestionTypeId == id)
                    .Select(a => new CreateData
                    {
                        Ide = CryptoSecurity.Encrypt(a.EvaluationQuestionTypeId),
                        NameSQ = a.NameSq,
                        NameEN = a.NameEn,
                        Title = title
                    }).FirstOrDefaultAsync();
                return PartialView(dataQuestion);
            default:
                return PartialView();
        }
    }

    [HttpPost, Authorize(Policy = "16:e"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Action to edit data from a look up table.")]
    public async Task<IActionResult> Edit(CreateData edit)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM() { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        var error = new ErrorVM() { Status = ErrorStatus.Success, Description = Resource.DataUpdatedSuccessfully };
        var id = CryptoSecurity.Decrypt<int>(edit.Ide);

        switch (edit.Table)
        {
            case LookUpTable.Document:
                var documentType = await db.DocumentType.FirstOrDefaultAsync(a => a.DocumentTypeId == id);
                documentType.NameSq = edit.NameSQ;
                documentType.NameEn = edit.NameEN;
                documentType.UpdatedDate = DateTime.Now;
                documentType.UpdatedFrom = user.Id;
                documentType.UpdatedNo = documentType.UpdatedNo.HasValue ? ++documentType.UpdatedNo : documentType.UpdatedNo = 1;
                await db.SaveChangesAsync();
                return Json(error);
            case LookUpTable.EducationLevel:
                var educationLevelType = await db.EducationLevelType.FirstOrDefaultAsync(a => a.EducationLevelTypeId == id);
                educationLevelType.NameSq = edit.NameSQ;
                educationLevelType.NameEn = edit.NameEN;
                educationLevelType.UpdatedDate = DateTime.Now;
                educationLevelType.UpdatedFrom = user.Id;
                educationLevelType.UpdatedNo = educationLevelType.UpdatedNo.HasValue ? ++educationLevelType.UpdatedNo : educationLevelType.UpdatedNo = 1;
                await db.SaveChangesAsync();
                return Json(error);
            case LookUpTable.Evaluation:
                var evaluationType = await db.EvaluationType.FirstOrDefaultAsync(a => a.EvaluationTypeId == id);
                evaluationType.NameSq = edit.NameSQ;
                evaluationType.NameEn = edit.NameEN;
                evaluationType.UpdatedDate = DateTime.Now;
                evaluationType.UpdatedFrom = user.Id;
                evaluationType.UpdatedNo = evaluationType.UpdatedNo.HasValue ? ++evaluationType.UpdatedNo : evaluationType.UpdatedNo = 1;
                await db.SaveChangesAsync();
                return Json(error);
            case LookUpTable.Holiday:
                var holidayType = await db.HolidayType.FirstOrDefaultAsync(a => a.HolidayTypeId == id);
                holidayType.NameSq = edit.NameSQ;
                holidayType.NameEn = edit.NameEN;
                holidayType.UpdatedDate = DateTime.Now;
                holidayType.UpdatedFrom = user.Id;
                holidayType.UpdatedNo = holidayType.UpdatedNo.HasValue ? ++holidayType.UpdatedNo : holidayType.UpdatedNo = 1;
                await db.SaveChangesAsync();
                return Json(error);
            case LookUpTable.Profession:
                var professionType = await db.ProfessionType.FirstOrDefaultAsync(a => a.ProfessionTypeId == id);
                professionType.NameSq = edit.NameSQ;
                professionType.NameEn = edit.NameEN;
                professionType.Code = edit.OtherData;
                professionType.UpdatedDate = DateTime.Now;
                professionType.UpdatedFrom = user.Id;
                professionType.UpdatedNo = professionType.UpdatedNo.HasValue ? ++professionType.UpdatedNo : professionType.UpdatedNo = 1;
                await db.SaveChangesAsync();
                return Json(error);
            case LookUpTable.Staff:
                var staffType = await db.StaffType.FirstOrDefaultAsync(a => a.StaffTypeId == id);
                staffType.NameSq = edit.NameSQ;
                staffType.NameEn = edit.NameEN;
                staffType.UpdatedDate = DateTime.Now;
                staffType.UpdatedFrom = user.Id;
                staffType.UpdatedNo = staffType.UpdatedNo.HasValue ? ++staffType.UpdatedNo : staffType.UpdatedNo = 1;
                await db.SaveChangesAsync();
                return Json(error);
            case LookUpTable.Status:
                var statusType = await db.StatusType.FirstOrDefaultAsync(a => a.StatusTypeId == id);
                statusType.NameSq = edit.NameSQ;
                statusType.NameEn = edit.NameEN;
                statusType.UpdatedDate = DateTime.Now;
                statusType.UpdatedFrom = user.Id;
                statusType.UpdatedNo = statusType.UpdatedNo.HasValue ? ++statusType.UpdatedNo : statusType.UpdatedNo = 1;
                await db.SaveChangesAsync();
                return Json(error);
            case LookUpTable.Department:
                var department = await db.Department.FirstOrDefaultAsync(a => a.DepartmentId == id);
                department.NameSq = edit.NameSQ;
                department.NameEn = edit.NameEN;
                department.Code = edit.OtherData;
                department.UpdatedDate = DateTime.Now;
                department.UpdatedFrom = user.Id;
                department.UpdatedNo = department.UpdatedNo.HasValue ? ++department.UpdatedNo : department.UpdatedNo = 1;
                await db.SaveChangesAsync();
                return Json(error);
            case LookUpTable.EvaluationQuestion:
                var question = await db.EvaluationQuestionType.FirstOrDefaultAsync(a => a.EvaluationQuestionTypeId == id);
                question.NameSq = edit.NameSQ;
                question.NameEn = edit.NameEN;
                question.UpdatedDate = DateTime.Now;
                question.UpdatedFrom = user.Id;
                question.UpdatedNo = question.UpdatedNo.HasValue ? ++question.UpdatedNo : question.UpdatedNo = 1;
                await db.SaveChangesAsync();
                return Json(error);
            default:
                return Json(new ErrorVM() { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }
    }

    #endregion

    #region Delete

    [HttpPost, Authorize(Policy = "16:d"), ValidateAntiForgeryToken]
    [Description("Arb Tahiri", "Action to delete data from a lookup table.")]
    public async Task<IActionResult> Delete(LookUpTable table, string ide, bool active)
    {
        var error = new ErrorVM() { Status = ErrorStatus.Success, Description = Resource.DataDeletedSuccessfully };

        var id = CryptoSecurity.Decrypt<int>(ide);

        switch (table)
        {
            case LookUpTable.Document:
                var documentType = await db.DocumentType.FirstOrDefaultAsync(a => a.DocumentTypeId == id);
                documentType.Active = active;
                documentType.UpdatedDate = DateTime.Now;
                documentType.UpdatedFrom = user.Id;
                documentType.UpdatedNo = documentType.UpdatedNo.HasValue ? ++documentType.UpdatedNo : documentType.UpdatedNo = 1;
                await db.SaveChangesAsync();
                return Json(error);
            case LookUpTable.EducationLevel:
                var educationLevelType = await db.EducationLevelType.FirstOrDefaultAsync(a => a.EducationLevelTypeId == id);
                educationLevelType.Active = active;
                educationLevelType.UpdatedDate = DateTime.Now;
                educationLevelType.UpdatedFrom = user.Id;
                educationLevelType.UpdatedNo = educationLevelType.UpdatedNo.HasValue ? ++educationLevelType.UpdatedNo : educationLevelType.UpdatedNo = 1;
                await db.SaveChangesAsync();
                return Json(error);
            case LookUpTable.Evaluation:
                var evaluationType = await db.EvaluationType.FirstOrDefaultAsync(a => a.EvaluationTypeId == id);
                evaluationType.Active = active;
                evaluationType.UpdatedDate = DateTime.Now;
                evaluationType.UpdatedFrom = user.Id;
                evaluationType.UpdatedNo = evaluationType.UpdatedNo.HasValue ? ++evaluationType.UpdatedNo : evaluationType.UpdatedNo = 1;
                await db.SaveChangesAsync();
                return Json(error);
            case LookUpTable.Holiday:
                var holidayType = await db.HolidayType.FirstOrDefaultAsync(a => a.HolidayTypeId == id);
                holidayType.Active = active;
                holidayType.UpdatedDate = DateTime.Now;
                holidayType.UpdatedFrom = user.Id;
                holidayType.UpdatedNo = holidayType.UpdatedNo.HasValue ? ++holidayType.UpdatedNo : holidayType.UpdatedNo = 1;
                await db.SaveChangesAsync();
                return Json(error);
            case LookUpTable.Profession:
                var professionType = await db.ProfessionType.FirstOrDefaultAsync(a => a.ProfessionTypeId == id);
                professionType.Active = active;
                professionType.UpdatedDate = DateTime.Now;
                professionType.UpdatedFrom = user.Id;
                professionType.UpdatedNo = professionType.UpdatedNo.HasValue ? ++professionType.UpdatedNo : professionType.UpdatedNo = 1;
                await db.SaveChangesAsync();
                return Json(error);
            case LookUpTable.Staff:
                var staffType = await db.StaffType.FirstOrDefaultAsync(a => a.StaffTypeId == id);
                staffType.Active = active;
                staffType.UpdatedDate = DateTime.Now;
                staffType.UpdatedFrom = user.Id;
                staffType.UpdatedNo = staffType.UpdatedNo.HasValue ? ++staffType.UpdatedNo : staffType.UpdatedNo = 1;
                await db.SaveChangesAsync();
                return Json(error);
            case LookUpTable.Status:
                var statusType = await db.StatusType.FirstOrDefaultAsync(a => a.StatusTypeId == id);
                statusType.Active = active;
                statusType.UpdatedDate = DateTime.Now;
                statusType.UpdatedFrom = user.Id;
                statusType.UpdatedNo = statusType.UpdatedNo.HasValue ? ++statusType.UpdatedNo : statusType.UpdatedNo = 1;
                await db.SaveChangesAsync();
                return Json(error);
            case LookUpTable.Department:
                var department = await db.Department.FirstOrDefaultAsync(a => a.DepartmentId == id);
                department.Active = active;
                department.UpdatedDate = DateTime.Now;
                department.UpdatedFrom = user.Id;
                department.UpdatedNo = department.UpdatedNo.HasValue ? ++department.UpdatedNo : department.UpdatedNo = 1;
                await db.SaveChangesAsync();
                return Json(error);
            case LookUpTable.EvaluationQuestion:
                var question = await db.EvaluationQuestionType.FirstOrDefaultAsync(a => a.EvaluationQuestionTypeId == id);
                question.Active = active;
                question.UpdatedDate = DateTime.Now;
                question.UpdatedFrom = user.Id;
                question.UpdatedNo = question.UpdatedNo.HasValue ? ++question.UpdatedNo : question.UpdatedNo = 1;
                await db.SaveChangesAsync();
                return Json(error);
            default:
                return Json(new ErrorVM() { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }
    }

    #endregion
}
