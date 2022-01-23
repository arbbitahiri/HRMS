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
    Holiday = 4,
    Profession = 5,
    Rate = 6,
    Staff = 7,
    Status = 8,
    Department = 9
}

public enum StatusTypeEnum
{
    Approved = 1,
    Rejected = 2,
    Pending = 3,
    Processing = 5,
    Finished = 6
}

public enum HolidayTypeEnum
{
    Annual = 1,
    Sick = 2,
    Maternity = 3,
    Unpaid = 4
}

public enum EvaluationTypeEnum
{
    Manager = 1,
    Student = 2,
    Self = 3
}
