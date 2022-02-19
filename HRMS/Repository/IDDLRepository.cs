﻿using HRMS.Utilities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HRMS.Repository;
public interface IDDLRepository
{
    List<SelectListItem> Languages();

    List<SelectListItem> Genders();

    List<SelectListItem> EventLogEntryTypes();

    Task<List<SelectListItem>> Roles(LanguageEnum lang);

    Task<List<SelectListItem>> Departments(LanguageEnum lang);

    Task<List<SelectListItem>> StaffTypes(LanguageEnum lang);

    Task<List<SelectListItem>> ProfessionTypes(LanguageEnum lang);

    Task<List<SelectListItem>> EducationLevelTypes(LanguageEnum lang);

    Task<List<SelectListItem>> DocumentTypes(LanguageEnum lang);

    Task<List<SelectListItem>> Subjects(LanguageEnum lang);

    Task<List<SelectListItem>> LeaveTypes(LanguageEnum lang);

    Task<List<SelectListItem>> StatusTypes(LanguageEnum lang);

    Task<List<SelectListItem>> EvaluationTypes(LanguageEnum lang);

    Task<List<SelectListItem>> EvaluationQuestionTypes(LanguageEnum lang);
}
