namespace HRMS.Utilities;
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
    GET = 1,
    POST = 2,
    PUT = 3
}

public enum StaffTypeEnum
{
    Administrator = 1,
    Professor = 2
}

public enum LookUpTable
{
    DocumentType = 1,
    EducationLevelType = 2,
    EvaluationType = 3,
    HolidayType = 4,
    ProfessionType = 5,
    RateType = 6,
    StaffType = 7,
    StatusType = 8
}
