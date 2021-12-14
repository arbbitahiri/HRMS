using System;
using System.Collections.Generic;

namespace HRMS.Data.General
{
    public partial class AspNetUsers
    {
        public AspNetUsers()
        {
            AspNetUserClaims = new HashSet<AspNetUserClaims>();
            AspNetUserLogins = new HashSet<AspNetUserLogins>();
            AspNetUserTokens = new HashSet<AspNetUserTokens>();
            DepartmentInsertedFromNavigation = new HashSet<Department>();
            DepartmentUpdatedFromNavigation = new HashSet<Department>();
            DocumentInsertedFromNavigation = new HashSet<Document>();
            DocumentTypeInsertedFromNavigation = new HashSet<DocumentType>();
            DocumentTypeUpdatedFromNavigation = new HashSet<DocumentType>();
            DocumentUpdatedFromNavigation = new HashSet<Document>();
            EducationLevelTypeInsertedFromNavigation = new HashSet<EducationLevelType>();
            EducationLevelTypeUpdatedFromNavigation = new HashSet<EducationLevelType>();
            EvaluationTypeInsertedFromNavigation = new HashSet<EvaluationType>();
            EvaluationTypeUpdatedFromNavigation = new HashSet<EvaluationType>();
            HolidayRequestInsertedFromNavigation = new HashSet<HolidayRequest>();
            HolidayRequestStatusInsertedFromNavigation = new HashSet<HolidayRequestStatus>();
            HolidayRequestStatusUpdatedFromNavigation = new HashSet<HolidayRequestStatus>();
            HolidayRequestUpdatedFromNavigation = new HashSet<HolidayRequest>();
            HolidayTypeInsertedFromNavigation = new HashSet<HolidayType>();
            HolidayTypeUpdatedFromNavigation = new HashSet<HolidayType>();
            MenuInsertedFromNavigation = new HashSet<Menu>();
            MenuUpdatedFromNavigation = new HashSet<Menu>();
            ProfessionTypeInsertedFromNavigation = new HashSet<ProfessionType>();
            ProfessionTypeUpdatedFromNavigation = new HashSet<ProfessionType>();
            RateTypeInsertedFromNavigation = new HashSet<RateType>();
            RateTypeUpdatedFromNavigation = new HashSet<RateType>();
            StaffCollegeEvaluationInsertedFromNavigation = new HashSet<StaffCollegeEvaluation>();
            StaffCollegeEvaluationQuestionnaireInsertedFromNavigation = new HashSet<StaffCollegeEvaluationQuestionnaire>();
            StaffCollegeEvaluationQuestionnaireUpdatedFromNavigation = new HashSet<StaffCollegeEvaluationQuestionnaire>();
            StaffCollegeEvaluationUpdatedFromNavigation = new HashSet<StaffCollegeEvaluation>();
            StaffCollegeInsertedFromNavigation = new HashSet<StaffCollege>();
            StaffCollegeSubjectInsertedFromNavigation = new HashSet<StaffCollegeSubject>();
            StaffCollegeSubjectUpdatedFromNavigation = new HashSet<StaffCollegeSubject>();
            StaffCollegeUpdatedFromNavigation = new HashSet<StaffCollege>();
            StaffDocumentInsertedFromNavigation = new HashSet<StaffDocument>();
            StaffDocumentUpdatedFromNavigation = new HashSet<StaffDocument>();
            StaffQualificationInsertedFromNavigation = new HashSet<StaffQualification>();
            StaffQualificationUpdatedFromNavigation = new HashSet<StaffQualification>();
            StaffTypeInsertedFromNavigation = new HashSet<StaffType>();
            StaffTypeUpdatedFromNavigation = new HashSet<StaffType>();
            StaffUpdatedFromNavigation = new HashSet<Staff>();
            StaffUser = new HashSet<Staff>();
            StatusTypeInsertedFromNavigation = new HashSet<StatusType>();
            StatusTypeUpdatedFromNavigation = new HashSet<StatusType>();
            SubMenuInsertedFromNavigation = new HashSet<SubMenu>();
            SubMenuUpdatedFromNavigation = new HashSet<SubMenu>();
            SubjectInsertedFromNavigation = new HashSet<Subject>();
            SubjectUpdatedFromNavigation = new HashSet<Subject>();
            Role = new HashSet<AspNetRoles>();
        }

        public string Id { get; set; }
        public string PersonalNumber { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? Birthdate { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string NormalizedUserName { get; set; }
        public string NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool? PhoneNumberConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string ConcurrencyStamp { get; set; }
        public bool AllowNotification { get; set; }
        public int Language { get; set; }
        public int Mode { get; set; }
        public string ProfileImage { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public DateTime InsertedDate { get; set; }
        public string InsertedFrom { get; set; }

        public virtual ICollection<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual ICollection<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual ICollection<AspNetUserTokens> AspNetUserTokens { get; set; }
        public virtual ICollection<Department> DepartmentInsertedFromNavigation { get; set; }
        public virtual ICollection<Department> DepartmentUpdatedFromNavigation { get; set; }
        public virtual ICollection<Document> DocumentInsertedFromNavigation { get; set; }
        public virtual ICollection<DocumentType> DocumentTypeInsertedFromNavigation { get; set; }
        public virtual ICollection<DocumentType> DocumentTypeUpdatedFromNavigation { get; set; }
        public virtual ICollection<Document> DocumentUpdatedFromNavigation { get; set; }
        public virtual ICollection<EducationLevelType> EducationLevelTypeInsertedFromNavigation { get; set; }
        public virtual ICollection<EducationLevelType> EducationLevelTypeUpdatedFromNavigation { get; set; }
        public virtual ICollection<EvaluationType> EvaluationTypeInsertedFromNavigation { get; set; }
        public virtual ICollection<EvaluationType> EvaluationTypeUpdatedFromNavigation { get; set; }
        public virtual ICollection<HolidayRequest> HolidayRequestInsertedFromNavigation { get; set; }
        public virtual ICollection<HolidayRequestStatus> HolidayRequestStatusInsertedFromNavigation { get; set; }
        public virtual ICollection<HolidayRequestStatus> HolidayRequestStatusUpdatedFromNavigation { get; set; }
        public virtual ICollection<HolidayRequest> HolidayRequestUpdatedFromNavigation { get; set; }
        public virtual ICollection<HolidayType> HolidayTypeInsertedFromNavigation { get; set; }
        public virtual ICollection<HolidayType> HolidayTypeUpdatedFromNavigation { get; set; }
        public virtual ICollection<Menu> MenuInsertedFromNavigation { get; set; }
        public virtual ICollection<Menu> MenuUpdatedFromNavigation { get; set; }
        public virtual ICollection<ProfessionType> ProfessionTypeInsertedFromNavigation { get; set; }
        public virtual ICollection<ProfessionType> ProfessionTypeUpdatedFromNavigation { get; set; }
        public virtual ICollection<RateType> RateTypeInsertedFromNavigation { get; set; }
        public virtual ICollection<RateType> RateTypeUpdatedFromNavigation { get; set; }
        public virtual ICollection<StaffCollegeEvaluation> StaffCollegeEvaluationInsertedFromNavigation { get; set; }
        public virtual ICollection<StaffCollegeEvaluationQuestionnaire> StaffCollegeEvaluationQuestionnaireInsertedFromNavigation { get; set; }
        public virtual ICollection<StaffCollegeEvaluationQuestionnaire> StaffCollegeEvaluationQuestionnaireUpdatedFromNavigation { get; set; }
        public virtual ICollection<StaffCollegeEvaluation> StaffCollegeEvaluationUpdatedFromNavigation { get; set; }
        public virtual ICollection<StaffCollege> StaffCollegeInsertedFromNavigation { get; set; }
        public virtual ICollection<StaffCollegeSubject> StaffCollegeSubjectInsertedFromNavigation { get; set; }
        public virtual ICollection<StaffCollegeSubject> StaffCollegeSubjectUpdatedFromNavigation { get; set; }
        public virtual ICollection<StaffCollege> StaffCollegeUpdatedFromNavigation { get; set; }
        public virtual ICollection<StaffDocument> StaffDocumentInsertedFromNavigation { get; set; }
        public virtual ICollection<StaffDocument> StaffDocumentUpdatedFromNavigation { get; set; }
        public virtual ICollection<StaffQualification> StaffQualificationInsertedFromNavigation { get; set; }
        public virtual ICollection<StaffQualification> StaffQualificationUpdatedFromNavigation { get; set; }
        public virtual ICollection<StaffType> StaffTypeInsertedFromNavigation { get; set; }
        public virtual ICollection<StaffType> StaffTypeUpdatedFromNavigation { get; set; }
        public virtual ICollection<Staff> StaffUpdatedFromNavigation { get; set; }
        public virtual ICollection<Staff> StaffUser { get; set; }
        public virtual ICollection<StatusType> StatusTypeInsertedFromNavigation { get; set; }
        public virtual ICollection<StatusType> StatusTypeUpdatedFromNavigation { get; set; }
        public virtual ICollection<SubMenu> SubMenuInsertedFromNavigation { get; set; }
        public virtual ICollection<SubMenu> SubMenuUpdatedFromNavigation { get; set; }
        public virtual ICollection<Subject> SubjectInsertedFromNavigation { get; set; }
        public virtual ICollection<Subject> SubjectUpdatedFromNavigation { get; set; }

        public virtual ICollection<AspNetRoles> Role { get; set; }
    }
}
