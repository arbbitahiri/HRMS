using HRMS.Data.General;
using HRMS.Resources;
using HRMS.Utilities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace HRMS.Repository;
public class DDLRepository : IDDLRepository
{
    private readonly HRMS_WorkContext db;

    public DDLRepository(HRMS_WorkContext db)
    {
        this.db = db;
    }

    public List<SelectListItem> Languages() => new()
    {
        new SelectListItem { Text = Resource.Albanian, Value = LanguageEnum.ALBANIAN.ToString() },
        new SelectListItem { Text = Resource.English, Value = LanguageEnum.ENGLISH.ToString() }
    };

    public List<SelectListItem> Genders() => new()
    {
        new SelectListItem { Value = ((int)GenderEnum.MALE).ToString(), Text = Resource.Male },
        new SelectListItem { Value = ((int)GenderEnum.FEMALE).ToString(), Text = Resource.Female },
    };

    [SupportedOSPlatform("windows")]
    public List<SelectListItem> EventLogEntryTypes() => new()
    {
        new SelectListItem { Value = EventLogEntryType.SuccessAudit.ToString(), Text = Resource.Success },
        new SelectListItem { Value = EventLogEntryType.Information.ToString(), Text = Resource.Info },
        new SelectListItem { Value = EventLogEntryType.Warning.ToString(), Text = Resource.Warning },
        new SelectListItem { Value = EventLogEntryType.Error.ToString(), Text = Resource.Error },
        new SelectListItem { Value = EventLogEntryType.FailureAudit.ToString(), Text = Resource.Failure }
    };

    public async Task<List<SelectListItem>> Roles(LanguageEnum lang) =>
        await db.AspNetRoles.Select(a => new SelectListItem
        {
            Value = a.Id,
            Text = lang == LanguageEnum.ALBANIAN ? a.NameSq : a.NameEn
        }).OrderBy(a => a.Text).ToListAsync();

    public async Task<List<SelectListItem>> Departments(LanguageEnum lang) =>
        await db.Department.Select(a => new SelectListItem
        {
            Value = a.DepartmentId.ToString(),
            Text = lang == LanguageEnum.ALBANIAN ? $"{a.Code} - {a.NameSq}" : $"{a.Code} - {a.NameEn}"
        }).ToListAsync();

    public async Task<List<SelectListItem>> StaffTypes(LanguageEnum lang) =>
        await db.StaffType.Select(a => new SelectListItem
        {
            Value = a.StaffTypeId.ToString(),
            Text = lang == LanguageEnum.ALBANIAN ? a.NameSq : a.NameEn
        }).OrderBy(a => a.Text).ToListAsync();

    public async Task<List<SelectListItem>> ProfessionTypes(LanguageEnum lang) =>
        await db.ProfessionType.Select(a => new SelectListItem
        {
            Value = a.ProfessionTypeId.ToString(),
            Text = lang == LanguageEnum.ALBANIAN ? $"{a.Code} - {a.NameSq}" : $"{a.Code} - {a.NameEn}"
        }).ToListAsync();

    public async Task<List<SelectListItem>> EducationLevelTypes(LanguageEnum lang) =>
        await db.EducationLevelType.Select(a => new SelectListItem
        {
            Value = a.EducationLevelTypeId.ToString(),
            Text = lang == LanguageEnum.ALBANIAN ? a.NameSq : a.NameEn
        }).OrderBy(a => a.Text).ToListAsync();

    public async Task<List<SelectListItem>> DocumentTypes(LanguageEnum lang) =>
        await db.DocumentType.Select(a => new SelectListItem
        {
            Value = a.DocumentTypeId.ToString(),
            Text = lang == LanguageEnum.ALBANIAN ? a.NameSq : a.NameEn
        }).OrderBy(a => a.Text).ToListAsync();

    public async Task<List<SelectListItem>> Subjects(LanguageEnum lang) =>
        await db.Subject.Select(a => new SelectListItem
        {
            Value = a.SubjectId.ToString(),
            Text = lang == LanguageEnum.ALBANIAN ? $"{a.Code} - {a.NameSq}" : $"{a.Code} - {a.NameEn}"
        }).ToListAsync();

    public async Task<List<SelectListItem>> HolidayTypes(LanguageEnum lang) =>
        await db.HolidayType.Select(a => new SelectListItem
        {
            Value = a.HolidayTypeId.ToString(),
            Text = lang == LanguageEnum.ALBANIAN ? a.NameSq : a.NameEn
        }).OrderBy(a => a.Text).ToListAsync();

    public async Task<List<SelectListItem>> StatusTypes(LanguageEnum lang) =>
        await db.StatusType.Select(a => new SelectListItem
        {
            Value = a.StatusTypeId.ToString(),
            Text = lang == LanguageEnum.ALBANIAN ? a.NameSq : a.NameEn
        }).OrderBy(a => a.Text).ToListAsync();
}
