using System;
using System.Collections.Generic;

namespace HRMS.Data.General
{
    public partial class AspNetUsers
    {
        public AspNetUsers()
        {
            AppSettings = new HashSet<AppSettings>();
            AspNetUserClaims = new HashSet<AspNetUserClaims>();
            AspNetUserLogins = new HashSet<AspNetUserLogins>();
            AspNetUserTokens = new HashSet<AspNetUserTokens>();
            DepartmentInsertedFromNavigation = new HashSet<Department>();
            DepartmentUpdatedFromNavigation = new HashSet<Department>();
            DocumentTypeInsertedFromNavigation = new HashSet<DocumentType>();
            DocumentTypeUpdatedFromNavigation = new HashSet<DocumentType>();
            EducationLevelTypeInsertedFromNavigation = new HashSet<EducationLevelType>();
            EducationLevelTypeUpdatedFromNavigation = new HashSet<EducationLevelType>();
            EvaluationDocumentInsertedFromNavigation = new HashSet<EvaluationDocument>();
            EvaluationDocumentUpdatedFromNavigation = new HashSet<EvaluationDocument>();
            EvaluationInsertedFromNavigation = new HashSet<Evaluation>();
            EvaluationManagerInsertedFromNavigation = new HashSet<EvaluationManager>();
            EvaluationManagerUpdatedFromNavigation = new HashSet<EvaluationManager>();
            EvaluationQuestionTypeInsertedFromNavigation = new HashSet<EvaluationQuestionType>();
            EvaluationQuestionTypeUpdatedFromNavigation = new HashSet<EvaluationQuestionType>();
            EvaluationQuestionnaireNumericalInsertedFromNavigation = new HashSet<EvaluationQuestionnaireNumerical>();
            EvaluationQuestionnaireNumericalUpdatedFromNavigation = new HashSet<EvaluationQuestionnaireNumerical>();
            EvaluationQuestionnaireOptionalInsertedFromNavigation = new HashSet<EvaluationQuestionnaireOptional>();
            EvaluationQuestionnaireOptionalOptionInsertedFromNavigation = new HashSet<EvaluationQuestionnaireOptionalOption>();
            EvaluationQuestionnaireOptionalOptionUpdatedFromNavigation = new HashSet<EvaluationQuestionnaireOptionalOption>();
            EvaluationQuestionnaireOptionalTopicInsertedFromNavigation = new HashSet<EvaluationQuestionnaireOptionalTopic>();
            EvaluationQuestionnaireOptionalTopicUpdatedFromNavigation = new HashSet<EvaluationQuestionnaireOptionalTopic>();
            EvaluationQuestionnaireOptionalUpdatedFromNavigation = new HashSet<EvaluationQuestionnaireOptional>();
            EvaluationQuestionnaireTopicInsertedFromNavigation = new HashSet<EvaluationQuestionnaireTopic>();
            EvaluationQuestionnaireTopicUpdatedFromNavigation = new HashSet<EvaluationQuestionnaireTopic>();
            EvaluationSelfInsertedFromNavigation = new HashSet<EvaluationSelf>();
            EvaluationSelfUpdatedFromNavigation = new HashSet<EvaluationSelf>();
            EvaluationStatusInsertedFromNavigation = new HashSet<EvaluationStatus>();
            EvaluationStatusUpdatedFromNavigation = new HashSet<EvaluationStatus>();
            EvaluationStudentsCollegeInsertedFromNavigation = new HashSet<EvaluationStudentsCollege>();
            EvaluationStudentsCollegeUpdatedFromNavigation = new HashSet<EvaluationStudentsCollege>();
            EvaluationStudentsStaffInsertedFromNavigation = new HashSet<EvaluationStudentsStaff>();
            EvaluationStudentsStaffUpdatedFromNavigation = new HashSet<EvaluationStudentsStaff>();
            EvaluationTypeInsertedFromNavigation = new HashSet<EvaluationType>();
            EvaluationTypeUpdatedFromNavigation = new HashSet<EvaluationType>();
            EvaluationUpdatedFromNavigation = new HashSet<Evaluation>();
            LeaveInsertedFromNavigation = new HashSet<Leave>();
            LeaveStaffDaysInsertedFromNavigation = new HashSet<LeaveStaffDays>();
            LeaveStaffDaysUpdatedFromNavigation = new HashSet<LeaveStaffDays>();
            LeaveStatusInsertedFromNavigation = new HashSet<LeaveStatus>();
            LeaveStatusUpdatedFromNavigation = new HashSet<LeaveStatus>();
            LeaveTypeInsertedFromNavigation = new HashSet<LeaveType>();
            LeaveTypeUpdatedFromNavigation = new HashSet<LeaveType>();
            LeaveUpdatedFromNavigation = new HashSet<Leave>();
            MenuInsertedFromNavigation = new HashSet<Menu>();
            MenuUpdatedFromNavigation = new HashSet<Menu>();
            ProfessionTypeInsertedFromNavigation = new HashSet<ProfessionType>();
            ProfessionTypeUpdatedFromNavigation = new HashSet<ProfessionType>();
            RateTypeInsertedFromNavigation = new HashSet<RateType>();
            RateTypeUpdatedFromNavigation = new HashSet<RateType>();
            RealRoleInsertedFromNavigation = new HashSet<RealRole>();
            RealRoleUpdatedFromNavigation = new HashSet<RealRole>();
            RealRoleUser = new HashSet<RealRole>();
            StaffDepartmentInsertedFromNavigation = new HashSet<StaffDepartment>();
            StaffDepartmentSubjectInsertedFromNavigation = new HashSet<StaffDepartmentSubject>();
            StaffDepartmentSubjectUpdatedFromNavigation = new HashSet<StaffDepartmentSubject>();
            StaffDepartmentUpdatedFromNavigation = new HashSet<StaffDepartment>();
            StaffDocumentInsertedFromNavigation = new HashSet<StaffDocument>();
            StaffDocumentUpdatedFromNavigation = new HashSet<StaffDocument>();
            StaffPayroll = new HashSet<StaffPayroll>();
            StaffQualificationInsertedFromNavigation = new HashSet<StaffQualification>();
            StaffQualificationUpdatedFromNavigation = new HashSet<StaffQualification>();
            StaffRegistrationStatus = new HashSet<StaffRegistrationStatus>();
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
        public int? DepartmentId { get; set; }
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

        public virtual Department Department { get; set; }
        public virtual ICollection<AppSettings> AppSettings { get; set; }
        public virtual ICollection<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual ICollection<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual ICollection<AspNetUserTokens> AspNetUserTokens { get; set; }
        public virtual ICollection<Department> DepartmentInsertedFromNavigation { get; set; }
        public virtual ICollection<Department> DepartmentUpdatedFromNavigation { get; set; }
        public virtual ICollection<DocumentType> DocumentTypeInsertedFromNavigation { get; set; }
        public virtual ICollection<DocumentType> DocumentTypeUpdatedFromNavigation { get; set; }
        public virtual ICollection<EducationLevelType> EducationLevelTypeInsertedFromNavigation { get; set; }
        public virtual ICollection<EducationLevelType> EducationLevelTypeUpdatedFromNavigation { get; set; }
        public virtual ICollection<EvaluationDocument> EvaluationDocumentInsertedFromNavigation { get; set; }
        public virtual ICollection<EvaluationDocument> EvaluationDocumentUpdatedFromNavigation { get; set; }
        public virtual ICollection<Evaluation> EvaluationInsertedFromNavigation { get; set; }
        public virtual ICollection<EvaluationManager> EvaluationManagerInsertedFromNavigation { get; set; }
        public virtual ICollection<EvaluationManager> EvaluationManagerUpdatedFromNavigation { get; set; }
        public virtual ICollection<EvaluationQuestionType> EvaluationQuestionTypeInsertedFromNavigation { get; set; }
        public virtual ICollection<EvaluationQuestionType> EvaluationQuestionTypeUpdatedFromNavigation { get; set; }
        public virtual ICollection<EvaluationQuestionnaireNumerical> EvaluationQuestionnaireNumericalInsertedFromNavigation { get; set; }
        public virtual ICollection<EvaluationQuestionnaireNumerical> EvaluationQuestionnaireNumericalUpdatedFromNavigation { get; set; }
        public virtual ICollection<EvaluationQuestionnaireOptional> EvaluationQuestionnaireOptionalInsertedFromNavigation { get; set; }
        public virtual ICollection<EvaluationQuestionnaireOptionalOption> EvaluationQuestionnaireOptionalOptionInsertedFromNavigation { get; set; }
        public virtual ICollection<EvaluationQuestionnaireOptionalOption> EvaluationQuestionnaireOptionalOptionUpdatedFromNavigation { get; set; }
        public virtual ICollection<EvaluationQuestionnaireOptionalTopic> EvaluationQuestionnaireOptionalTopicInsertedFromNavigation { get; set; }
        public virtual ICollection<EvaluationQuestionnaireOptionalTopic> EvaluationQuestionnaireOptionalTopicUpdatedFromNavigation { get; set; }
        public virtual ICollection<EvaluationQuestionnaireOptional> EvaluationQuestionnaireOptionalUpdatedFromNavigation { get; set; }
        public virtual ICollection<EvaluationQuestionnaireTopic> EvaluationQuestionnaireTopicInsertedFromNavigation { get; set; }
        public virtual ICollection<EvaluationQuestionnaireTopic> EvaluationQuestionnaireTopicUpdatedFromNavigation { get; set; }
        public virtual ICollection<EvaluationSelf> EvaluationSelfInsertedFromNavigation { get; set; }
        public virtual ICollection<EvaluationSelf> EvaluationSelfUpdatedFromNavigation { get; set; }
        public virtual ICollection<EvaluationStatus> EvaluationStatusInsertedFromNavigation { get; set; }
        public virtual ICollection<EvaluationStatus> EvaluationStatusUpdatedFromNavigation { get; set; }
        public virtual ICollection<EvaluationStudentsCollege> EvaluationStudentsCollegeInsertedFromNavigation { get; set; }
        public virtual ICollection<EvaluationStudentsCollege> EvaluationStudentsCollegeUpdatedFromNavigation { get; set; }
        public virtual ICollection<EvaluationStudentsStaff> EvaluationStudentsStaffInsertedFromNavigation { get; set; }
        public virtual ICollection<EvaluationStudentsStaff> EvaluationStudentsStaffUpdatedFromNavigation { get; set; }
        public virtual ICollection<EvaluationType> EvaluationTypeInsertedFromNavigation { get; set; }
        public virtual ICollection<EvaluationType> EvaluationTypeUpdatedFromNavigation { get; set; }
        public virtual ICollection<Evaluation> EvaluationUpdatedFromNavigation { get; set; }
        public virtual ICollection<Leave> LeaveInsertedFromNavigation { get; set; }
        public virtual ICollection<LeaveStaffDays> LeaveStaffDaysInsertedFromNavigation { get; set; }
        public virtual ICollection<LeaveStaffDays> LeaveStaffDaysUpdatedFromNavigation { get; set; }
        public virtual ICollection<LeaveStatus> LeaveStatusInsertedFromNavigation { get; set; }
        public virtual ICollection<LeaveStatus> LeaveStatusUpdatedFromNavigation { get; set; }
        public virtual ICollection<LeaveType> LeaveTypeInsertedFromNavigation { get; set; }
        public virtual ICollection<LeaveType> LeaveTypeUpdatedFromNavigation { get; set; }
        public virtual ICollection<Leave> LeaveUpdatedFromNavigation { get; set; }
        public virtual ICollection<Menu> MenuInsertedFromNavigation { get; set; }
        public virtual ICollection<Menu> MenuUpdatedFromNavigation { get; set; }
        public virtual ICollection<ProfessionType> ProfessionTypeInsertedFromNavigation { get; set; }
        public virtual ICollection<ProfessionType> ProfessionTypeUpdatedFromNavigation { get; set; }
        public virtual ICollection<RateType> RateTypeInsertedFromNavigation { get; set; }
        public virtual ICollection<RateType> RateTypeUpdatedFromNavigation { get; set; }
        public virtual ICollection<RealRole> RealRoleInsertedFromNavigation { get; set; }
        public virtual ICollection<RealRole> RealRoleUpdatedFromNavigation { get; set; }
        public virtual ICollection<RealRole> RealRoleUser { get; set; }
        public virtual ICollection<StaffDepartment> StaffDepartmentInsertedFromNavigation { get; set; }
        public virtual ICollection<StaffDepartmentSubject> StaffDepartmentSubjectInsertedFromNavigation { get; set; }
        public virtual ICollection<StaffDepartmentSubject> StaffDepartmentSubjectUpdatedFromNavigation { get; set; }
        public virtual ICollection<StaffDepartment> StaffDepartmentUpdatedFromNavigation { get; set; }
        public virtual ICollection<StaffDocument> StaffDocumentInsertedFromNavigation { get; set; }
        public virtual ICollection<StaffDocument> StaffDocumentUpdatedFromNavigation { get; set; }
        public virtual ICollection<StaffPayroll> StaffPayroll { get; set; }
        public virtual ICollection<StaffQualification> StaffQualificationInsertedFromNavigation { get; set; }
        public virtual ICollection<StaffQualification> StaffQualificationUpdatedFromNavigation { get; set; }
        public virtual ICollection<StaffRegistrationStatus> StaffRegistrationStatus { get; set; }
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
