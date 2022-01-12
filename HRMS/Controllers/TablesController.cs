using HRMS.Data.Core;
using HRMS.Data.General;
using HRMS.Models;
using HRMS.Models.Tables;
using HRMS.Resources;
using HRMS.Utilities;
using HRMS.Utilities.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.Controllers;
[Authorize(Policy = "11t:r")]
public class TablesController : BaseController
{
    public TablesController(HRMSContext db, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        : base(db, signInManager, userManager)
    {
    }

    #region Table list

    [Authorize(Policy = "11t:r"), Description("Entry form.")]
    public IActionResult Index() => View();

    [Authorize(Policy = "11t:r"), Description("List of lookup tables.")]
    public IActionResult _LookUpTables()
    {
        var tables = new List<TableName>()
        {
            new TableName() { Table = LookUpTable.DocumentType, Title = Resource.DocumentType },
            new TableName() { Table = LookUpTable.EducationLevelType, Title = Resource.EducationLevelType },
            new TableName() { Table = LookUpTable.EvaluationType, Title = Resource.EvaluationType },
            new TableName() { Table = LookUpTable.HolidayType, Title = Resource.HolidayType },
            new TableName() { Table = LookUpTable.ProfessionType, Title = Resource.ProfessionType },
            new TableName() { Table = LookUpTable.RateType, Title = Resource.RateType },
            new TableName() { Table = LookUpTable.StaffType, Title = Resource.StaffType },
            new TableName() { Table = LookUpTable.StatusType, Title = Resource.StatusType },
            new TableName() { Table = LookUpTable.Department, Title = Resource.Department }
        };

        return PartialView(tables);
    }

    #endregion

    #region Table data

    [Authorize(Policy = "11t:r"), Description("List of data of look up tables")]
    public async Task<IActionResult> _LookUpData(LookUpTable table, string title)
    {
        switch (table)
        {
            case LookUpTable.DocumentType:
                var dataDocumentType = await db.DocumentType.Select(a => new DataList
                {
                    Ide = CryptoSecurity.Encrypt(a.DocumentTypeId),
                    NameSQ = a.NameSq,
                    NameEN = a.NameEn,
                    Active = a.Active
                }).ToListAsync();
                return PartialView(new TableData() { DataList = dataDocumentType, Table = table, Title = title });
            case LookUpTable.EducationLevelType:
                var dataEducationLevelType = await db.EducationLevelType.Select(a => new DataList
                {
                    Ide = CryptoSecurity.Encrypt(a.EducationLevelTypeId),
                    NameSQ = a.NameSq,
                    NameEN = a.NameEn,
                    Active = a.Active
                }).ToListAsync();
                return PartialView(new TableData() { DataList = dataEducationLevelType, Table = table, Title = title });
            case LookUpTable.EvaluationType:
                var dataEvaluationType = await db.EvaluationType.Select(a => new DataList
                {
                    Ide = CryptoSecurity.Encrypt(a.EvaluationTypeId),
                    NameSQ = a.NameSq,
                    NameEN = a.NameEn,
                    Active = a.Active
                }).ToListAsync();
                return PartialView(new TableData() { DataList = dataEvaluationType, Table = table, Title = title });
            case LookUpTable.HolidayType:
                var dataHolidayType = await db.HolidayType.Select(a => new DataList
                {
                    Ide = CryptoSecurity.Encrypt(a.HolidayTypeId),
                    NameSQ = a.NameSq,
                    NameEN = a.NameEn,
                    Active = true
                }).ToListAsync();
                return PartialView(new TableData() { DataList = dataHolidayType, Table = table, Title = title });
            case LookUpTable.ProfessionType:
                var dataProfessionType = await db.ProfessionType.Select(a => new DataList
                {
                    Ide = CryptoSecurity.Encrypt(a.ProfessionTypeId),
                    NameSQ = a.NameSq,
                    NameEN = a.NameEn,
                    OtherData = a.Code,
                    Active = a.Active
                }).ToListAsync();
                return PartialView(new TableData() { DataList = dataProfessionType, Table = table, Title = title });
            case LookUpTable.RateType:
                var dataRateType = await db.RateType.Select(a => new DataList
                {
                    Ide = CryptoSecurity.Encrypt(a.RateTypeId),
                    NameSQ = a.NameSq,
                    NameEN = a.NameEn,
                    OtherData = a.RateNumber.ToString(),
                    Active = a.Active
                }).ToListAsync();
                return PartialView(new TableData() { DataList = dataRateType, Table = table, Title = title });
            case LookUpTable.StaffType:
                var dataStaffType = await db.StaffType.Select(a => new DataList
                {
                    Ide = CryptoSecurity.Encrypt(a.StaffTypeId),
                    NameSQ = a.NameSq,
                    NameEN = a.NameEn,
                    Active = true
                }).ToListAsync();
                return PartialView(new TableData() { DataList = dataStaffType, Table = table, Title = title });
            case LookUpTable.StatusType:
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

    [Authorize(Policy = "11t:r"), Description("Form to create data for a look up table.")]
    public IActionResult _Create(LookUpTable table, string title) => PartialView(new CreateData { Table = table, Title = title });

    [HttpPost, Authorize(Policy = "11t:r"), ValidateAntiForgeryToken]
    [Description("Action to create data for a look up table.")]
    public async Task<IActionResult> Create(CreateData create)
    {
        if (!ModelState.IsValid)
        {
            return Json(new ErrorVM() { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }

        var error = new ErrorVM() { Status = ErrorStatus.Success, Description = Resource.DataRegisteredSuccessfully };

        switch (create.Table)
        {
            case LookUpTable.DocumentType:
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
            case LookUpTable.EducationLevelType:
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
            case LookUpTable.EvaluationType:
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
            case LookUpTable.HolidayType:
                db.HolidayType.Add(new HolidayType
                {
                    NameSq = create.NameSQ,
                    NameEn = create.NameEN,
                    //Active = true,
                    InsertedDate = DateTime.Now,
                    InsertedFrom = user.Id
                });
                await db.SaveChangesAsync();
                return Json(error);
            case LookUpTable.ProfessionType:
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
            case LookUpTable.RateType:
                db.RateType.Add(new RateType
                {
                    NameSq = create.NameSQ,
                    NameEn = create.NameEN,
                    RateNumber = int.Parse(create.OtherData),
                    Active = true,
                    InsertedDate = DateTime.Now,
                    InsertedFrom = user.Id
                });
                await db.SaveChangesAsync();
                return Json(error);
            case LookUpTable.StaffType:
                db.StaffType.Add(new StaffType
                {
                    NameSq = create.NameSQ,
                    NameEn = create.NameEN,
                    //Active = true,
                    InsertedDate = DateTime.Now,
                    InsertedFrom = user.Id
                });
                await db.SaveChangesAsync();
                return Json(error);
            case LookUpTable.StatusType:
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
            default:
                return Json(new ErrorVM() { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }
    }

    #endregion

    #region Edit

    [Authorize(Policy = "11t:r"), Description("Form to edit data from look up tables.")]
    public async Task<IActionResult> _Edit(LookUpTable table, string title, string ide)
    {
        var id = CryptoSecurity.Decrypt<int>(ide);
        switch (table)
        {
            case LookUpTable.DocumentType:
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
            case LookUpTable.EducationLevelType:
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
            case LookUpTable.EvaluationType:
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
            case LookUpTable.HolidayType:
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
            case LookUpTable.ProfessionType:
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
            case LookUpTable.RateType:
                var dataRateType = await db.RateType
                    .Where(a => a.RateTypeId == id)
                    .Select(a => new CreateData
                    {
                        Ide = CryptoSecurity.Encrypt(a.RateTypeId),
                        NameSQ = a.NameSq,
                        NameEN = a.NameEn,
                        OtherData = a.RateNumber.ToString(),
                        Title = title
                    }).FirstOrDefaultAsync();
                return PartialView(dataRateType);
            case LookUpTable.StaffType:
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
            case LookUpTable.StatusType:
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
            default:
                return PartialView();
        }
    }

    [HttpPost, Authorize(Policy = "11t:r"), ValidateAntiForgeryToken]
    [Description("Action to edit data from a look up table.")]
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
            case LookUpTable.DocumentType:
                var documentType = await db.DocumentType.FirstOrDefaultAsync(a => a.DocumentTypeId == id);
                documentType.NameSq = edit.NameSQ;
                documentType.NameEn = edit.NameEN;
                documentType.Active = edit.Active;
                documentType.UpdatedDate = DateTime.Now;
                documentType.UpdatedFrom = user.Id;
                documentType.UpdatedNo++;
                await db.SaveChangesAsync();
                return Json(error);
            case LookUpTable.EducationLevelType:
                var educationLevelType = await db.EducationLevelType.FirstOrDefaultAsync(a => a.EducationLevelTypeId == id);
                educationLevelType.NameSq = edit.NameSQ;
                educationLevelType.NameEn = edit.NameEN;
                educationLevelType.Active = edit.Active;
                educationLevelType.UpdatedDate = DateTime.Now;
                educationLevelType.UpdatedFrom = user.Id;
                educationLevelType.UpdatedNo++;
                await db.SaveChangesAsync();
                return Json(error);
            case LookUpTable.EvaluationType:
                var evaluationType = await db.EvaluationType.FirstOrDefaultAsync(a => a.EvaluationTypeId == id);
                evaluationType.NameSq = edit.NameSQ;
                evaluationType.NameEn = edit.NameEN;
                evaluationType.Active = edit.Active;
                evaluationType.UpdatedDate = DateTime.Now;
                evaluationType.UpdatedFrom = user.Id;
                evaluationType.UpdatedNo++;
                await db.SaveChangesAsync();
                return Json(error);
            case LookUpTable.HolidayType:
                var holidayType = await db.HolidayType.FirstOrDefaultAsync(a => a.HolidayTypeId == id);
                holidayType.NameSq = edit.NameSQ;
                holidayType.NameEn = edit.NameEN;
                //holidayType.Active = edit.Active;
                holidayType.UpdatedDate = DateTime.Now;
                holidayType.UpdatedFrom = user.Id;
                holidayType.UpdatedNo++;
                await db.SaveChangesAsync();
                return Json(error);
            case LookUpTable.ProfessionType:
                var professionType = await db.ProfessionType.FirstOrDefaultAsync(a => a.ProfessionTypeId == id);
                professionType.NameSq = edit.NameSQ;
                professionType.NameEn = edit.NameEN;
                professionType.Code = edit.OtherData;
                professionType.Active = edit.Active;
                professionType.UpdatedDate = DateTime.Now;
                professionType.UpdatedFrom = user.Id;
                professionType.UpdatedNo++;
                await db.SaveChangesAsync();
                return Json(error);
            case LookUpTable.RateType:
                var rateType = await db.RateType.FirstOrDefaultAsync(a => a.RateTypeId == id);
                rateType.NameSq = edit.NameSQ;
                rateType.NameEn = edit.NameEN;
                rateType.RateNumber = int.Parse(edit.OtherData);
                rateType.Active = edit.Active;
                rateType.UpdatedDate = DateTime.Now;
                rateType.UpdatedFrom = user.Id;
                rateType.UpdatedNo++;
                await db.SaveChangesAsync();
                return Json(error);
            case LookUpTable.StaffType:
                var staffType = await db.StaffType.FirstOrDefaultAsync(a => a.StaffTypeId == id);
                staffType.NameSq = edit.NameSQ;
                staffType.NameEn = edit.NameEN;
                //staffType.Active = edit.Active;
                staffType.UpdatedDate = DateTime.Now;
                staffType.UpdatedFrom = user.Id;
                staffType.UpdatedNo++;
                await db.SaveChangesAsync();
                return Json(error);
            case LookUpTable.StatusType:
                var statusType = await db.StatusType.FirstOrDefaultAsync(a => a.StatusTypeId == id);
                statusType.NameSq = edit.NameSQ;
                statusType.NameEn = edit.NameEN;
                statusType.Active = edit.Active;
                statusType.UpdatedDate = DateTime.Now;
                statusType.UpdatedFrom = user.Id;
                statusType.UpdatedNo++;
                await db.SaveChangesAsync();
                return Json(error);
            case LookUpTable.Department:
                var department = await db.Department.FirstOrDefaultAsync(a => a.DepartmentId == id);
                department.NameSq = edit.NameSQ;
                department.NameEn = edit.NameEN;
                department.Code = edit.OtherData;
                department.Active = edit.Active;
                department.UpdatedDate = DateTime.Now;
                department.UpdatedFrom = user.Id;
                department.UpdatedNo++;
                await db.SaveChangesAsync();
                return Json(error);
            default:
                return Json(new ErrorVM() { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }
    }

    #endregion

    #region Delete

    [HttpPost, Authorize(Policy = "11t:r"), ValidateAntiForgeryToken]
    [Description("Action to delete data from a lookup table.")]
    public async Task<IActionResult> Delete(LookUpTable table, string ide, bool active)
    {
        var error = new ErrorVM() { Status = ErrorStatus.Success, Description = Resource.DataUpdatedSuccessfully };

        var id = CryptoSecurity.Decrypt<int>(ide);

        switch (table)
        {
            case LookUpTable.DocumentType:
                var documentType = await db.DocumentType.FirstOrDefaultAsync(a => a.DocumentTypeId == id);
                documentType.Active = active;
                documentType.UpdatedDate = DateTime.Now;
                documentType.UpdatedFrom = user.Id;
                documentType.UpdatedNo++;
                await db.SaveChangesAsync();
                return Json(error);
            case LookUpTable.EducationLevelType:
                var educationLevelType = await db.EducationLevelType.FirstOrDefaultAsync(a => a.EducationLevelTypeId == id);
                educationLevelType.Active = active;
                educationLevelType.UpdatedDate = DateTime.Now;
                educationLevelType.UpdatedFrom = user.Id;
                educationLevelType.UpdatedNo++;
                await db.SaveChangesAsync();
                return Json(error);
            case LookUpTable.EvaluationType:
                var evaluationType = await db.EvaluationType.FirstOrDefaultAsync(a => a.EvaluationTypeId == id);
                evaluationType.Active = active;
                evaluationType.UpdatedDate = DateTime.Now;
                evaluationType.UpdatedFrom = user.Id;
                evaluationType.UpdatedNo++;
                await db.SaveChangesAsync();
                return Json(error);
            case LookUpTable.HolidayType:
                var holidayType = await db.HolidayType.FirstOrDefaultAsync(a => a.HolidayTypeId == id);
                //holidayType.Active = edit.Active;
                holidayType.UpdatedDate = DateTime.Now;
                holidayType.UpdatedFrom = user.Id;
                holidayType.UpdatedNo++;
                await db.SaveChangesAsync();
                return Json(error);
            case LookUpTable.ProfessionType:
                var professionType = await db.ProfessionType.FirstOrDefaultAsync(a => a.ProfessionTypeId == id);
                professionType.Active = active;
                professionType.UpdatedDate = DateTime.Now;
                professionType.UpdatedFrom = user.Id;
                professionType.UpdatedNo++;
                await db.SaveChangesAsync();
                return Json(error);
            case LookUpTable.RateType:
                var rateType = await db.RateType.FirstOrDefaultAsync(a => a.RateTypeId == id);
                rateType.Active = active;
                rateType.UpdatedDate = DateTime.Now;
                rateType.UpdatedFrom = user.Id;
                rateType.UpdatedNo++;
                await db.SaveChangesAsync();
                return Json(error);
            case LookUpTable.StaffType:
                var staffType = await db.StaffType.FirstOrDefaultAsync(a => a.StaffTypeId == id);
                //staffType.Active = edit.Active;
                staffType.UpdatedDate = DateTime.Now;
                staffType.UpdatedFrom = user.Id;
                staffType.UpdatedNo++;
                await db.SaveChangesAsync();
                return Json(error);
            case LookUpTable.StatusType:
                var statusType = await db.StatusType.FirstOrDefaultAsync(a => a.StatusTypeId == id);
                statusType.Active = active;
                statusType.UpdatedDate = DateTime.Now;
                statusType.UpdatedFrom = user.Id;
                statusType.UpdatedNo++;
                await db.SaveChangesAsync();
                return Json(error);
            case LookUpTable.Department:
                var department = await db.Department.FirstOrDefaultAsync(a => a.DepartmentId == id);
                department.Active = active;
                department.UpdatedDate = DateTime.Now;
                department.UpdatedFrom = user.Id;
                department.UpdatedNo++;
                await db.SaveChangesAsync();
                return Json(error);
            default:
                return Json(new ErrorVM() { Status = ErrorStatus.Warning, Description = Resource.InvalidData });
        }
    }

    #endregion
}
