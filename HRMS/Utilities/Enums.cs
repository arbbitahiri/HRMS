﻿namespace HRMS.Utilities;

public enum Status
{
    Approved = 1,
    Rejected = 2,
    Pending = 3,
    Processing = 4,
    Finished = 5,
    Deleted = 6,
    Unprocessed = 7,
    PendingForAnswers = 8
}

public enum ReportType
{
    PDF = 1,
    Excel = 2,
    Word = 3
}

public enum ReportOrientation
{
    Portrait = 1,
    Landscape = 2
}

public enum ErrorStatus
{
    Success = 1,
    Error = 2,
    Warning = 3,
    Info = 4
}

public enum GenderEnum
{
    Male = 1,
    Female = 2
}

public enum TemplateMode
{
    Dark = 1,
    Light = 2
}

public enum LanguageEnum
{
    Albanian = 1,
    English = 2
}

public enum ImageSizeType
{
    ProfilePhoto = 512,
    News = 1280
}

public enum MethodType
{
    Get = 1,
    Post = 2,
    Put = 3
}

public enum StaffTypeEnum
{
    Lecturer = 1,
    Administrator = 4,
    Manager = 5
}

public enum LookUpTable
{
    Document = 1,
    EducationLevel = 2,
    Evaluation = 3,
    Leave = 4,
    Profession = 5,
    Rate = 6,
    Staff = 7,
    Status = 8,
    Department = 9,
    EvaluationQuestion = 10,
    DocumentFor = 11,
    Holiday = 12,
    Repeat = 13
}

public enum LeaveTypeEnum
{
    Annual = 1,
    Sick = 2,
    Maternity = 3,
    Unpaid = 4
}

public enum EvaluationTypeEnum
{
    Manager = 1,
    StudentStaff = 2,
    StudentCollege = 3,
    Self = 4
}

public enum QuestionType
{
    Numeral = 1,
    Optional = 2,
    Topic = 3,
    OptionalTopic = 5
}

public enum StudentsEvaluationType
{
    Staff = 1,
    College = 2
}

public enum JobTypeEnum
{
    Primary = 1,
    Secondary = 2
}

public enum CountryEnum
{
    Kosova = 1
}

public enum DocumentForEnum
{
    Staff = 1,
    Evaluation = 2
}

public enum NotificationTypeEnum
{
    Success = 1,
    Info = 2,
    Warning = 3,
    Error = 4,
    Question = 5
}

public enum HolidayTypeEnum
{
    Other = 13
}

public enum RepeatTypeEnum
{
    Once = 1,
    Daily = 2,
    Weekly = 3,
    Monthly = 4,
    Anually = 5
}
