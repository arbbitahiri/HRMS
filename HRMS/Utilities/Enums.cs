namespace HRMS.Utilities;

public enum ErrorStatus
{
    SUCCESS = 1,
    ERROR = 2,
    WARNING = 3,
    INFO = 4
}

public enum GenderEnum
{
    MALE = 1,
    FEMALE = 2
}

public enum TemplateMode
{
    DARK = 1,
    LIGHT = 2
}

public enum LanguageEnum
{
    ALBANIAN = 1,
    ENGLISH = 2
}

public enum ImageSizeType
{
    PROFILEPHOTO = 512,
    NEWS = 1280
}

public enum MethodType
{
    GET = 1,
    POST = 2,
    PUT = 3
}

public enum StaffTypeEnum
{
    LECTURER = 1,
    ADMINISTRATOR = 4,
    MANAGER = 5
}

public enum LookUpTable
{
    DOCUMENTTYPE = 1,
    EDUCATIONLEVELTYPE = 2,
    EVALUATIONTYPE = 3,
    HOLIDAYTYPE = 4,
    PROFESSIONTYPE = 5,
    RATETYPE = 6,
    STAFFTYPE = 7,
    STATUSTYPE = 8,
    DEPARTMENT = 9
}

public enum StatusTypeEnum
{
    APPROVED = 1,
    REJECTED = 2,
    PENDING = 3
}

public enum HolidayTypeEnum
{
    ANNUALLEAVE = 1,
    SICKLEAVE = 2,
    MATERNITYLEAVE = 3,
    UNPAIDLEAVE = 4
}
