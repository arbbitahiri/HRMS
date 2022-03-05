using HRMS.Data.General;
using HRMS.Resources;
using HRMS.Utilities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace HRMS.Repository;
public class DDLRepository : IDDLRepository
{
    private readonly HRMSContext db;

    public DDLRepository(HRMSContext db)
    {
        this.db = db;
    }

    public List<SelectListItem> Languages() => new()
    {
        new SelectListItem { Text = Resource.Albanian, Value = LanguageEnum.Albanian.ToString() },
        new SelectListItem { Text = Resource.English, Value = LanguageEnum.English.ToString() }
    };

    public List<SelectListItem> Genders() => new()
    {
        new SelectListItem { Value = ((int)GenderEnum.Male).ToString(), Text = Resource.Male },
        new SelectListItem { Value = ((int)GenderEnum.Female).ToString(), Text = Resource.Female },
    };

    public List<SelectListItem> Months() => new()
    {
        new SelectListItem { Value = 1.ToString(), Text = Resource.January, Selected = DateTime.Now.Month == 1 },
        new SelectListItem { Value = 2.ToString(), Text = Resource.February, Selected = DateTime.Now.Month == 2 },
        new SelectListItem { Value = 3.ToString(), Text = Resource.March, Selected = DateTime.Now.Month == 3 },
        new SelectListItem { Value = 4.ToString(), Text = Resource.April, Selected = DateTime.Now.Month == 4 },
        new SelectListItem { Value = 5.ToString(), Text = Resource.May, Selected = DateTime.Now.Month == 5 },
        new SelectListItem { Value = 6.ToString(), Text = Resource.June, Selected = DateTime.Now.Month == 6 },
        new SelectListItem { Value = 7.ToString(), Text = Resource.July, Selected = DateTime.Now.Month == 7 },
        new SelectListItem { Value = 8.ToString(), Text = Resource.August, Selected = DateTime.Now.Month == 8 },
        new SelectListItem { Value = 9.ToString(), Text = Resource.September, Selected = DateTime.Now.Month == 9 },
        new SelectListItem { Value = 10.ToString(), Text = Resource.October, Selected = DateTime.Now.Month == 10 },
        new SelectListItem { Value = 11.ToString(), Text = Resource.November, Selected = DateTime.Now.Month == 11 },
        new SelectListItem { Value = 12.ToString(), Text = Resource.December, Selected = DateTime.Now.Month == 12 }
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
            Text = lang == LanguageEnum.Albanian ? a.NameSq : a.NameEn
        }).OrderBy(a => a.Text).ToListAsync();

    public async Task<List<SelectListItem>> Departments(LanguageEnum lang) =>
        await db.Department.Where(a => a.Active)
            .Select(a => new SelectListItem
            {
                Value = a.DepartmentId.ToString(),
                Text = lang == LanguageEnum.Albanian ? $"{a.Code} - {a.NameSq}" : $"{a.Code} - {a.NameEn}"
            }).ToListAsync();

    public async Task<List<SelectListItem>> StaffTypes(LanguageEnum lang) =>
        await db.StaffType.Where(a => a.Active)
            .Select(a => new SelectListItem
            {
                Value = a.StaffTypeId.ToString(),
                Text = lang == LanguageEnum.Albanian ? a.NameSq : a.NameEn
            }).OrderBy(a => a.Text).ToListAsync();

    public async Task<List<SelectListItem>> ProfessionTypes(LanguageEnum lang) =>
        await db.ProfessionType.Where(a => a.Active)
            .Select(a => new SelectListItem
            {
                Value = a.ProfessionTypeId.ToString(),
                Text = lang == LanguageEnum.Albanian ? $"{a.Code} - {a.NameSq}" : $"{a.Code} - {a.NameEn}"
            }).ToListAsync();

    public async Task<List<SelectListItem>> EducationLevelTypes(LanguageEnum lang) =>
        await db.EducationLevelType.Where(a => a.Active)
            .Select(a => new SelectListItem
            {
                Value = a.EducationLevelTypeId.ToString(),
                Text = lang == LanguageEnum.Albanian ? a.NameSq : a.NameEn
            }).OrderBy(a => a.Text).ToListAsync();

    public async Task<List<SelectListItem>> DocumentTypes(LanguageEnum lang) =>
        await db.DocumentType.Where(a => a.Active)
            .Select(a => new SelectListItem
            {
                Value = a.DocumentTypeId.ToString(),
                Text = lang == LanguageEnum.Albanian ? a.NameSq : a.NameEn
            }).OrderBy(a => a.Text).ToListAsync();

    public async Task<List<SelectListItem>> Subjects(LanguageEnum lang) =>
        await db.Subject.Where(a => a.Active)
            .Select(a => new SelectListItem
            {
                Value = a.SubjectId.ToString(),
                Text = lang == LanguageEnum.Albanian ? $"{a.Code} - {a.NameSq}" : $"{a.Code} - {a.NameEn}"
            }).ToListAsync();

    public async Task<List<SelectListItem>> LeaveTypes(LanguageEnum lang) =>
        await db.LeaveType.Where(a => a.Active)
            .Select(a => new SelectListItem
            {
                Value = a.LeaveTypeId.ToString(),
                Text = lang == LanguageEnum.Albanian ? a.NameSq : a.NameEn
            }).OrderBy(a => a.Text).ToListAsync();

    public async Task<List<SelectListItem>> StatusTypes(LanguageEnum lang) =>
        await db.StatusType.Where(a => a.Active)
            .Select(a => new SelectListItem
            {
                Value = a.StatusTypeId.ToString(),
                Text = lang == LanguageEnum.Albanian ? a.NameSq : a.NameEn
            }).OrderBy(a => a.Text).ToListAsync();

    public async Task<List<SelectListItem>> EvaluationTypes(LanguageEnum lang) =>
        await db.EvaluationType.Where(a => a.Active)
            .Select(a => new SelectListItem
            {
                Value = a.EvaluationTypeId.ToString(),
                Text = lang == LanguageEnum.Albanian ? a.NameSq : a.NameEn
            }).OrderBy(a => a.Text).ToListAsync();

    public async Task<List<SelectListItem>> EvaluationQuestionTypes(LanguageEnum lang) =>
        await db.EvaluationQuestionType.Where(a => a.Active)
            .Select(a => new SelectListItem
            {
                Value = a.EvaluationQuestionTypeId.ToString(),
                Text = lang == LanguageEnum.Albanian ? a.NameSq : a.NameEn
            }).OrderBy(a => a.Text).ToListAsync();

    public async Task<List<SelectListItem>> JobTypes(LanguageEnum lang) =>
        await db.JobType
            .Select(a => new SelectListItem
            {
                Value = a.JobTypeId.ToString(),
                Text = lang == LanguageEnum.Albanian ? a.NameSq : a.NameEn
            }).OrderBy(a => a.Text).ToListAsync();

    public async Task<List<SelectListItem>> Staff() =>
        await db.Staff
            .Select(a => new SelectListItem
            {
                Value = a.StaffId.ToString(),
                Text = a.FirstName + " " + a.LastName
            }).ToListAsync();

    public async Task<List<SelectListItem>> GetDocumentFor(LanguageEnum lang) =>
        await db.DocumentFor
            .Select(a => new SelectListItem
            {
                Value = a.DocumentForId.ToString(),
                Text = lang == LanguageEnum.Albanian ? a.NameSq : a.NameEn
            }).ToListAsync();
}
