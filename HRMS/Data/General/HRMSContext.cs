using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace HRMS.Data.General
{
    public partial class HRMSContext : DbContext
    {
        public HRMSContext()
        {
        }

        public HRMSContext(DbContextOptions<HRMSContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AppSettings> AppSettings { get; set; }
        public virtual DbSet<AspNetRoleClaims> AspNetRoleClaims { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserTokens> AspNetUserTokens { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<AspNetUsersH> AspNetUsersH { get; set; }
        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<DocumentType> DocumentType { get; set; }
        public virtual DbSet<EducationLevelType> EducationLevelType { get; set; }
        public virtual DbSet<Evaluation> Evaluation { get; set; }
        public virtual DbSet<EvaluationDocument> EvaluationDocument { get; set; }
        public virtual DbSet<EvaluationManager> EvaluationManager { get; set; }
        public virtual DbSet<EvaluationQuestionType> EvaluationQuestionType { get; set; }
        public virtual DbSet<EvaluationQuestionnaireNumerical> EvaluationQuestionnaireNumerical { get; set; }
        public virtual DbSet<EvaluationQuestionnaireOptional> EvaluationQuestionnaireOptional { get; set; }
        public virtual DbSet<EvaluationQuestionnaireOptionalOption> EvaluationQuestionnaireOptionalOption { get; set; }
        public virtual DbSet<EvaluationQuestionnaireOptionalTopic> EvaluationQuestionnaireOptionalTopic { get; set; }
        public virtual DbSet<EvaluationQuestionnaireTopic> EvaluationQuestionnaireTopic { get; set; }
        public virtual DbSet<EvaluationSelf> EvaluationSelf { get; set; }
        public virtual DbSet<EvaluationStatus> EvaluationStatus { get; set; }
        public virtual DbSet<EvaluationStudentsCollege> EvaluationStudentsCollege { get; set; }
        public virtual DbSet<EvaluationStudentsStaff> EvaluationStudentsStaff { get; set; }
        public virtual DbSet<EvaluationType> EvaluationType { get; set; }
        public virtual DbSet<Gender> Gender { get; set; }
        public virtual DbSet<JobType> JobType { get; set; }
        public virtual DbSet<Leave> Leave { get; set; }
        public virtual DbSet<LeaveStaffDays> LeaveStaffDays { get; set; }
        public virtual DbSet<LeaveStatus> LeaveStatus { get; set; }
        public virtual DbSet<LeaveType> LeaveType { get; set; }
        public virtual DbSet<Log> Log { get; set; }
        public virtual DbSet<Menu> Menu { get; set; }
        public virtual DbSet<ProfessionType> ProfessionType { get; set; }
        public virtual DbSet<RateType> RateType { get; set; }
        public virtual DbSet<RealRole> RealRole { get; set; }
        public virtual DbSet<Staff> Staff { get; set; }
        public virtual DbSet<StaffDepartment> StaffDepartment { get; set; }
        public virtual DbSet<StaffDepartmentSubject> StaffDepartmentSubject { get; set; }
        public virtual DbSet<StaffDocument> StaffDocument { get; set; }
        public virtual DbSet<StaffQualification> StaffQualification { get; set; }
        public virtual DbSet<StaffRegistrationStatus> StaffRegistrationStatus { get; set; }
        public virtual DbSet<StaffType> StaffType { get; set; }
        public virtual DbSet<StatusType> StatusType { get; set; }
        public virtual DbSet<SubMenu> SubMenu { get; set; }
        public virtual DbSet<Subject> Subject { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=DESKTOP-C9V24QP;Database=HRMS;Trusted_Connection=True;MultipleActiveResultSets=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppSettings>(entity =>
            {
                entity.HasKey(e => e.HistoryAppSettingsId);

                entity.ToTable("AppSettings", "History");

                entity.Property(e => e.HistoryAppSettingsId).HasColumnName("HistoryAppSettingsID");

                entity.Property(e => e.IndertedDate).HasColumnType("datetime");

                entity.Property(e => e.InsertedFrom)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.OldVersion).IsRequired();

                entity.Property(e => e.UpdatedVersion).IsRequired();

                entity.HasOne(d => d.InsertedFromNavigation)
                    .WithMany(p => p.AppSettings)
                    .HasForeignKey(d => d.InsertedFrom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AppSettings_AspNetUsers_Inserted");
            });

            modelBuilder.Entity<AspNetRoleClaims>(entity =>
            {
                entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

                entity.Property(e => e.RoleId).IsRequired();

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetRoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AspNetRoles>(entity =>
            {
                entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedName] IS NOT NULL)");

                entity.Property(e => e.DescriptionEn)
                    .HasMaxLength(1024)
                    .HasColumnName("DescriptionEN");

                entity.Property(e => e.DescriptionSq)
                    .HasMaxLength(1024)
                    .HasColumnName("DescriptionSQ");

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NameEn)
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasColumnName("NameEN");

                entity.Property(e => e.NameSq)
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasColumnName("NameSQ");

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetUserClaims>(entity =>
            {
                entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserLogins>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.ProviderKey).HasMaxLength(128);

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserTokens>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.Name).HasMaxLength(128);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserTokens)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUsers>(entity =>
            {
                entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedUserName] IS NOT NULL)");

                entity.Property(e => e.Birthdate).HasColumnType("date");

                entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.InsertedDate).HasColumnType("datetime");

                entity.Property(e => e.InsertedFrom)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.NormalizedEmail)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.PersonalNumber)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.PhoneNumber).IsRequired();

                entity.Property(e => e.ProfileImage).HasMaxLength(512);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.AspNetUsers)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("FK_AspNetUsers_Department");

                entity.HasMany(d => d.Role)
                    .WithMany(p => p.User)
                    .UsingEntity<Dictionary<string, object>>(
                        "AspNetUserRoles",
                        l => l.HasOne<AspNetRoles>().WithMany().HasForeignKey("RoleId"),
                        r => r.HasOne<AspNetUsers>().WithMany().HasForeignKey("UserId"),
                        j =>
                        {
                            j.HasKey("UserId", "RoleId");

                            j.ToTable("AspNetUserRoles");

                            j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                        });
            });

            modelBuilder.Entity<AspNetUsersH>(entity =>
            {
                entity.HasKey(e => e.HistoryAspNetUsersId)
                    .HasName("PK_AspNetUsers_1");

                entity.ToTable("AspNetUsersH", "History");

                entity.Property(e => e.HistoryAspNetUsersId).HasColumnName("HistoryAspNetUsersID");

                entity.Property(e => e.Birthdate).HasColumnType("date");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.Id)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.InsertedDate).HasColumnType("datetime");

                entity.Property(e => e.InsertedFrom)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.NormalizedEmail)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.PasswordHash).IsRequired();

                entity.Property(e => e.PersonalNumber)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.PhoneNumber).IsRequired();

                entity.Property(e => e.ProfileImage).HasMaxLength(512);

                entity.Property(e => e.Reason)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasIndex(e => e.InsertedFrom, "IX_Department_InsertedFrom");

                entity.HasIndex(e => e.UpdatedFrom, "IX_Department_UpdatedFrom");

                entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");

                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.InsertedDate).HasColumnType("datetime");

                entity.Property(e => e.InsertedFrom).IsRequired();

                entity.Property(e => e.NameEn)
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasColumnName("NameEN");

                entity.Property(e => e.NameSq)
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasColumnName("NameSQ");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.InsertedFromNavigation)
                    .WithMany(p => p.DepartmentInsertedFromNavigation)
                    .HasForeignKey(d => d.InsertedFrom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Department_AspNetUsers_Inserted");

                entity.HasOne(d => d.UpdatedFromNavigation)
                    .WithMany(p => p.DepartmentUpdatedFromNavigation)
                    .HasForeignKey(d => d.UpdatedFrom)
                    .HasConstraintName("FK_Department_AspNetUsers_Updated");
            });

            modelBuilder.Entity<DocumentType>(entity =>
            {
                entity.HasIndex(e => e.InsertedFrom, "IX_DocumentType_InsertedFrom");

                entity.HasIndex(e => e.UpdatedFrom, "IX_DocumentType_UpdatedFrom");

                entity.Property(e => e.DocumentTypeId).HasColumnName("DocumentTypeID");

                entity.Property(e => e.InsertedDate).HasColumnType("datetime");

                entity.Property(e => e.InsertedFrom).IsRequired();

                entity.Property(e => e.NameEn)
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasColumnName("NameEN");

                entity.Property(e => e.NameSq)
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasColumnName("NameSQ");

                entity.Property(e => e.TypeFor).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.InsertedFromNavigation)
                    .WithMany(p => p.DocumentTypeInsertedFromNavigation)
                    .HasForeignKey(d => d.InsertedFrom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DocumentType_AspNetUsers_Inserted");

                entity.HasOne(d => d.UpdatedFromNavigation)
                    .WithMany(p => p.DocumentTypeUpdatedFromNavigation)
                    .HasForeignKey(d => d.UpdatedFrom)
                    .HasConstraintName("FK_DocumentType_AspNetUsers_Updated");
            });

            modelBuilder.Entity<EducationLevelType>(entity =>
            {
                entity.HasIndex(e => e.InsertedFrom, "IX_EducationLevelType_InsertedFrom");

                entity.HasIndex(e => e.UpdatedFrom, "IX_EducationLevelType_UpdatedFrom");

                entity.Property(e => e.EducationLevelTypeId).HasColumnName("EducationLevelTypeID");

                entity.Property(e => e.InsertedDate).HasColumnType("datetime");

                entity.Property(e => e.InsertedFrom).IsRequired();

                entity.Property(e => e.NameEn)
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasColumnName("NameEN");

                entity.Property(e => e.NameSq)
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasColumnName("NameSQ");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.InsertedFromNavigation)
                    .WithMany(p => p.EducationLevelTypeInsertedFromNavigation)
                    .HasForeignKey(d => d.InsertedFrom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EducationLevelType_AspNetUsers_Inserted");

                entity.HasOne(d => d.UpdatedFromNavigation)
                    .WithMany(p => p.EducationLevelTypeUpdatedFromNavigation)
                    .HasForeignKey(d => d.UpdatedFrom)
                    .HasConstraintName("FK_EducationLevelType_AspNetUsers_Updated");
            });

            modelBuilder.Entity<Evaluation>(entity =>
            {
                entity.Property(e => e.EvaluationId).HasColumnName("EvaluationID");

                entity.Property(e => e.EvaluationTypeId).HasColumnName("EvaluationTypeID");

                entity.Property(e => e.InsertedDate).HasColumnType("datetime");

                entity.Property(e => e.InsertedFrom)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedFrom).HasMaxLength(450);

                entity.HasOne(d => d.EvaluationType)
                    .WithMany(p => p.Evaluation)
                    .HasForeignKey(d => d.EvaluationTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Evaluation_EvaluationType");

                entity.HasOne(d => d.InsertedFromNavigation)
                    .WithMany(p => p.EvaluationInsertedFromNavigation)
                    .HasForeignKey(d => d.InsertedFrom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Evaluation_AspNetUsers_Inserted");

                entity.HasOne(d => d.UpdatedFromNavigation)
                    .WithMany(p => p.EvaluationUpdatedFromNavigation)
                    .HasForeignKey(d => d.UpdatedFrom)
                    .HasConstraintName("FK_Evaluation_AspNetUsers_Updated");
            });

            modelBuilder.Entity<EvaluationDocument>(entity =>
            {
                entity.Property(e => e.EvaluationDocumentId).HasColumnName("EvaluationDocumentID");

                entity.Property(e => e.Description).HasMaxLength(2048);

                entity.Property(e => e.DocumentTypeId).HasColumnName("DocumentTypeID");

                entity.Property(e => e.EvaluationId).HasColumnName("EvaluationID");

                entity.Property(e => e.InsertedDate).HasColumnType("datetime");

                entity.Property(e => e.InsertedFrom)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.Path)
                    .IsRequired()
                    .HasMaxLength(2048);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedFrom).HasMaxLength(450);

                entity.HasOne(d => d.DocumentType)
                    .WithMany(p => p.EvaluationDocument)
                    .HasForeignKey(d => d.DocumentTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EvaluationDocument_DocumentType");

                entity.HasOne(d => d.Evaluation)
                    .WithMany(p => p.EvaluationDocument)
                    .HasForeignKey(d => d.EvaluationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EvaluationDocument_Evaluation");

                entity.HasOne(d => d.InsertedFromNavigation)
                    .WithMany(p => p.EvaluationDocumentInsertedFromNavigation)
                    .HasForeignKey(d => d.InsertedFrom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EvaluationDocument_AspNetUsers_Inserted");

                entity.HasOne(d => d.UpdatedFromNavigation)
                    .WithMany(p => p.EvaluationDocumentUpdatedFromNavigation)
                    .HasForeignKey(d => d.UpdatedFrom)
                    .HasConstraintName("FK_EvaluationDocument_AspNetUsers_Updated");
            });

            modelBuilder.Entity<EvaluationManager>(entity =>
            {
                entity.Property(e => e.EvaluationManagerId).HasColumnName("EvaluationManagerID");

                entity.Property(e => e.Description).HasMaxLength(2048);

                entity.Property(e => e.EvaluationId).HasColumnName("EvaluationID");

                entity.Property(e => e.InsertedDate).HasColumnType("datetime");

                entity.Property(e => e.InsertedFrom)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.ManagerId).HasColumnName("ManagerID");

                entity.Property(e => e.StaffId).HasColumnName("StaffID");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedFrom).HasMaxLength(450);

                entity.HasOne(d => d.Evaluation)
                    .WithMany(p => p.EvaluationManager)
                    .HasForeignKey(d => d.EvaluationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EvaluationManager_Evaluation");

                entity.HasOne(d => d.InsertedFromNavigation)
                    .WithMany(p => p.EvaluationManagerInsertedFromNavigation)
                    .HasForeignKey(d => d.InsertedFrom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EvaluationManager_AspNetUsers_Inserted");

                entity.HasOne(d => d.Manager)
                    .WithMany(p => p.EvaluationManager)
                    .HasForeignKey(d => d.ManagerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EvaluationManager_StaffDepartment");

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.EvaluationManager)
                    .HasForeignKey(d => d.StaffId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EvaluationManager_Staff");

                entity.HasOne(d => d.UpdatedFromNavigation)
                    .WithMany(p => p.EvaluationManagerUpdatedFromNavigation)
                    .HasForeignKey(d => d.UpdatedFrom)
                    .HasConstraintName("FK_EvaluationManager_AspNetUsers_Updated");
            });

            modelBuilder.Entity<EvaluationQuestionType>(entity =>
            {
                entity.Property(e => e.EvaluationQuestionTypeId).HasColumnName("EvaluationQuestionTypeID");

                entity.Property(e => e.InsertedDate).HasColumnType("datetime");

                entity.Property(e => e.InsertedFrom)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.NameEn)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("NameEN");

                entity.Property(e => e.NameSq)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("NameSQ");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedFrom).HasMaxLength(450);

                entity.HasOne(d => d.InsertedFromNavigation)
                    .WithMany(p => p.EvaluationQuestionTypeInsertedFromNavigation)
                    .HasForeignKey(d => d.InsertedFrom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EvaluationQuestionType_EvaluationQuestionType_Inserted");

                entity.HasOne(d => d.UpdatedFromNavigation)
                    .WithMany(p => p.EvaluationQuestionTypeUpdatedFromNavigation)
                    .HasForeignKey(d => d.UpdatedFrom)
                    .HasConstraintName("FK_EvaluationQuestionType_EvaluationQuestionType_Updated");
            });

            modelBuilder.Entity<EvaluationQuestionnaireNumerical>(entity =>
            {
                entity.Property(e => e.EvaluationQuestionnaireNumericalId).HasColumnName("EvaluationQuestionnaireNumericalID");

                entity.Property(e => e.EvaluationId).HasColumnName("EvaluationID");

                entity.Property(e => e.InsertedDate).HasColumnType("datetime");

                entity.Property(e => e.InsertedFrom)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.QuestionEn)
                    .IsRequired()
                    .HasMaxLength(1024)
                    .HasColumnName("QuestionEN");

                entity.Property(e => e.QuestionSq)
                    .IsRequired()
                    .HasMaxLength(1024)
                    .HasColumnName("QuestionSQ");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedFrom).HasMaxLength(450);

                entity.HasOne(d => d.Evaluation)
                    .WithMany(p => p.EvaluationQuestionnaireNumerical)
                    .HasForeignKey(d => d.EvaluationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EvaluationQuestionnaire_Evaluation");

                entity.HasOne(d => d.InsertedFromNavigation)
                    .WithMany(p => p.EvaluationQuestionnaireNumericalInsertedFromNavigation)
                    .HasForeignKey(d => d.InsertedFrom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EvaluationQuestionnaire_AspNetUsers_Inserted");

                entity.HasOne(d => d.UpdatedFromNavigation)
                    .WithMany(p => p.EvaluationQuestionnaireNumericalUpdatedFromNavigation)
                    .HasForeignKey(d => d.UpdatedFrom)
                    .HasConstraintName("FK_EvaluationQuestionnaire_AspNetUsers_Updated");
            });

            modelBuilder.Entity<EvaluationQuestionnaireOptional>(entity =>
            {
                entity.Property(e => e.EvaluationQuestionnaireOptionalId).HasColumnName("EvaluationQuestionnaireOptionalID");

                entity.Property(e => e.Description).HasMaxLength(2048);

                entity.Property(e => e.EvaluationId).HasColumnName("EvaluationID");

                entity.Property(e => e.InsertedDate).HasColumnType("datetime");

                entity.Property(e => e.InsertedFrom)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.QuestionEn)
                    .IsRequired()
                    .HasMaxLength(1024)
                    .HasColumnName("QuestionEN");

                entity.Property(e => e.QuestionSq)
                    .IsRequired()
                    .HasMaxLength(1024)
                    .HasColumnName("QuestionSQ");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedFrom).HasMaxLength(450);

                entity.HasOne(d => d.Evaluation)
                    .WithMany(p => p.EvaluationQuestionnaireOptional)
                    .HasForeignKey(d => d.EvaluationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EvaluationQuestionnaireOptional_Evaluation");

                entity.HasOne(d => d.InsertedFromNavigation)
                    .WithMany(p => p.EvaluationQuestionnaireOptionalInsertedFromNavigation)
                    .HasForeignKey(d => d.InsertedFrom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EvaluationQuestionnaireOptional_AspNetUsers_Inserted");

                entity.HasOne(d => d.UpdatedFromNavigation)
                    .WithMany(p => p.EvaluationQuestionnaireOptionalUpdatedFromNavigation)
                    .HasForeignKey(d => d.UpdatedFrom)
                    .HasConstraintName("FK_EvaluationQuestionnaireOptional_AspNetUsers_Updated");
            });

            modelBuilder.Entity<EvaluationQuestionnaireOptionalOption>(entity =>
            {
                entity.Property(e => e.EvaluationQuestionnaireOptionalOptionId).HasColumnName("EvaluationQuestionnaireOptionalOptionID");

                entity.Property(e => e.Description).HasMaxLength(2048);

                entity.Property(e => e.EvaluationQuestionnaireOptionalId).HasColumnName("EvaluationQuestionnaireOptionalID");

                entity.Property(e => e.InsertedDate).HasColumnType("datetime");

                entity.Property(e => e.InsertedFrom)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.OptionTitleEn)
                    .IsRequired()
                    .HasMaxLength(512)
                    .HasColumnName("OptionTitleEN");

                entity.Property(e => e.OptionTitleSq)
                    .IsRequired()
                    .HasMaxLength(512)
                    .HasColumnName("OptionTitleSQ");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedFrom).HasMaxLength(450);

                entity.HasOne(d => d.EvaluationQuestionnaireOptional)
                    .WithMany(p => p.EvaluationQuestionnaireOptionalOption)
                    .HasForeignKey(d => d.EvaluationQuestionnaireOptionalId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EvaluationQuestionnaireOptionalOption_EvaluationQuestionnaireOptional");

                entity.HasOne(d => d.InsertedFromNavigation)
                    .WithMany(p => p.EvaluationQuestionnaireOptionalOptionInsertedFromNavigation)
                    .HasForeignKey(d => d.InsertedFrom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EvaluationQuestionnaireOptionalOption_AspNetUsers_Inserted");

                entity.HasOne(d => d.UpdatedFromNavigation)
                    .WithMany(p => p.EvaluationQuestionnaireOptionalOptionUpdatedFromNavigation)
                    .HasForeignKey(d => d.UpdatedFrom)
                    .HasConstraintName("FK_EvaluationQuestionnaireOptionalOption_AspNetUsers_Updated");
            });

            modelBuilder.Entity<EvaluationQuestionnaireOptionalTopic>(entity =>
            {
                entity.Property(e => e.EvaluationQuestionnaireOptionalTopicId).HasColumnName("EvaluationQuestionnaireOptionalTopicID");

                entity.Property(e => e.EvaluationQuestionnaireOptionalId).HasColumnName("EvaluationQuestionnaireOptionalID");

                entity.Property(e => e.InsertedDate).HasColumnType("datetime");

                entity.Property(e => e.InsertedFrom)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.TopicTitle)
                    .IsRequired()
                    .HasMaxLength(1024);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedFrom).HasMaxLength(450);

                entity.HasOne(d => d.EvaluationQuestionnaireOptional)
                    .WithMany(p => p.EvaluationQuestionnaireOptionalTopic)
                    .HasForeignKey(d => d.EvaluationQuestionnaireOptionalId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EvaluationQuestionnaireOptionalTopic_EvaluationQuestionnaireOptional");

                entity.HasOne(d => d.InsertedFromNavigation)
                    .WithMany(p => p.EvaluationQuestionnaireOptionalTopicInsertedFromNavigation)
                    .HasForeignKey(d => d.InsertedFrom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EvaluationQuestionnaireOptionalTopic_AspNetUsers_Inserted");

                entity.HasOne(d => d.UpdatedFromNavigation)
                    .WithMany(p => p.EvaluationQuestionnaireOptionalTopicUpdatedFromNavigation)
                    .HasForeignKey(d => d.UpdatedFrom)
                    .HasConstraintName("FK_EvaluationQuestionnaireOptionalTopic_AspNetUsers_Updated");
            });

            modelBuilder.Entity<EvaluationQuestionnaireTopic>(entity =>
            {
                entity.Property(e => e.EvaluationQuestionnaireTopicId).HasColumnName("EvaluationQuestionnaireTopicID");

                entity.Property(e => e.EvaluationId).HasColumnName("EvaluationID");

                entity.Property(e => e.InsertedDate).HasColumnType("datetime");

                entity.Property(e => e.InsertedFrom)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.QuestionEn)
                    .IsRequired()
                    .HasMaxLength(1024)
                    .HasColumnName("QuestionEN");

                entity.Property(e => e.QuestionSq)
                    .IsRequired()
                    .HasMaxLength(1024)
                    .HasColumnName("QuestionSQ");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedFrom).HasMaxLength(450);

                entity.HasOne(d => d.Evaluation)
                    .WithMany(p => p.EvaluationQuestionnaireTopic)
                    .HasForeignKey(d => d.EvaluationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EvaluationQuestionnaireTopic_Evaluation");

                entity.HasOne(d => d.InsertedFromNavigation)
                    .WithMany(p => p.EvaluationQuestionnaireTopicInsertedFromNavigation)
                    .HasForeignKey(d => d.InsertedFrom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EvaluationQuestionnaireTopic_AspNetUsers_Inserted");

                entity.HasOne(d => d.UpdatedFromNavigation)
                    .WithMany(p => p.EvaluationQuestionnaireTopicUpdatedFromNavigation)
                    .HasForeignKey(d => d.UpdatedFrom)
                    .HasConstraintName("FK_EvaluationQuestionnaireTopic_AspNetUsers_Updated");
            });

            modelBuilder.Entity<EvaluationSelf>(entity =>
            {
                entity.Property(e => e.EvaluationSelfId).HasColumnName("EvaluationSelfID");

                entity.Property(e => e.Description).HasMaxLength(2048);

                entity.Property(e => e.EvaluationId).HasColumnName("EvaluationID");

                entity.Property(e => e.InsertedDate).HasColumnType("datetime");

                entity.Property(e => e.InsertedFrom)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.StaffId).HasColumnName("StaffID");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedFrom).HasMaxLength(450);

                entity.HasOne(d => d.Evaluation)
                    .WithMany(p => p.EvaluationSelf)
                    .HasForeignKey(d => d.EvaluationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EvaluationSelf_Evaluation");

                entity.HasOne(d => d.InsertedFromNavigation)
                    .WithMany(p => p.EvaluationSelfInsertedFromNavigation)
                    .HasForeignKey(d => d.InsertedFrom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EvaluationSelf_AspNetUsers_Inserted");

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.EvaluationSelf)
                    .HasForeignKey(d => d.StaffId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EvaluationSelf_Staff");

                entity.HasOne(d => d.UpdatedFromNavigation)
                    .WithMany(p => p.EvaluationSelfUpdatedFromNavigation)
                    .HasForeignKey(d => d.UpdatedFrom)
                    .HasConstraintName("FK_EvaluationSelf_AspNetUsers_Updated");
            });

            modelBuilder.Entity<EvaluationStatus>(entity =>
            {
                entity.Property(e => e.EvaluationStatusId).HasColumnName("EvaluationStatusID");

                entity.Property(e => e.Description).HasMaxLength(2048);

                entity.Property(e => e.EvaluationId).HasColumnName("EvaluationID");

                entity.Property(e => e.InsertedDate).HasColumnType("datetime");

                entity.Property(e => e.InsertedFrom)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.StatusTypeId).HasColumnName("StatusTypeID");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedFrom).HasMaxLength(450);

                entity.HasOne(d => d.Evaluation)
                    .WithMany(p => p.EvaluationStatus)
                    .HasForeignKey(d => d.EvaluationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EvaluationStatus_Evaluation");

                entity.HasOne(d => d.InsertedFromNavigation)
                    .WithMany(p => p.EvaluationStatusInsertedFromNavigation)
                    .HasForeignKey(d => d.InsertedFrom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StaffDepartmentEvaluationStatus_AspNetUsers_Inserted");

                entity.HasOne(d => d.StatusType)
                    .WithMany(p => p.EvaluationStatus)
                    .HasForeignKey(d => d.StatusTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StaffDepartmentEvaluationStatus_StatusType");

                entity.HasOne(d => d.UpdatedFromNavigation)
                    .WithMany(p => p.EvaluationStatusUpdatedFromNavigation)
                    .HasForeignKey(d => d.UpdatedFrom)
                    .HasConstraintName("FK_StaffDepartmentEvaluationStatus_AspNetUsers_Updated");
            });

            modelBuilder.Entity<EvaluationStudentsCollege>(entity =>
            {
                entity.Property(e => e.EvaluationStudentsCollegeId).HasColumnName("EvaluationStudentsCollegeID");

                entity.Property(e => e.Description).HasMaxLength(2048);

                entity.Property(e => e.EvaluationId).HasColumnName("EvaluationID");

                entity.Property(e => e.InsertedDate).HasColumnType("datetime");

                entity.Property(e => e.InsertedFrom)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedFrom).HasMaxLength(450);

                entity.HasOne(d => d.Evaluation)
                    .WithMany(p => p.EvaluationStudentsCollege)
                    .HasForeignKey(d => d.EvaluationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EvaluationStudentsCollege_Evaluation");

                entity.HasOne(d => d.InsertedFromNavigation)
                    .WithMany(p => p.EvaluationStudentsCollegeInsertedFromNavigation)
                    .HasForeignKey(d => d.InsertedFrom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EvaluationStudentsCollege_AspNetUsers_Inserted");

                entity.HasOne(d => d.UpdatedFromNavigation)
                    .WithMany(p => p.EvaluationStudentsCollegeUpdatedFromNavigation)
                    .HasForeignKey(d => d.UpdatedFrom)
                    .HasConstraintName("FK_EvaluationStudentsCollege_AspNetUsers_Updated");
            });

            modelBuilder.Entity<EvaluationStudentsStaff>(entity =>
            {
                entity.Property(e => e.EvaluationStudentsStaffId).HasColumnName("EvaluationStudentsStaffID");

                entity.Property(e => e.Description).HasMaxLength(2048);

                entity.Property(e => e.EvaluationId).HasColumnName("EvaluationID");

                entity.Property(e => e.InsertedDate).HasColumnType("datetime");

                entity.Property(e => e.InsertedFrom)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.StaffDepartmentSubjectId).HasColumnName("StaffDepartmentSubjectID");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedFrom).HasMaxLength(450);

                entity.HasOne(d => d.Evaluation)
                    .WithMany(p => p.EvaluationStudentsStaff)
                    .HasForeignKey(d => d.EvaluationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EvaluationStudentsStaff_Evaluation");

                entity.HasOne(d => d.InsertedFromNavigation)
                    .WithMany(p => p.EvaluationStudentsStaffInsertedFromNavigation)
                    .HasForeignKey(d => d.InsertedFrom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EvaluationStudentsStaff_AspNetUsers_Inserted");

                entity.HasOne(d => d.StaffDepartmentSubject)
                    .WithMany(p => p.EvaluationStudentsStaff)
                    .HasForeignKey(d => d.StaffDepartmentSubjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EvaluationStudentsStaff_StaffDepartmentSubject");

                entity.HasOne(d => d.UpdatedFromNavigation)
                    .WithMany(p => p.EvaluationStudentsStaffUpdatedFromNavigation)
                    .HasForeignKey(d => d.UpdatedFrom)
                    .HasConstraintName("FK_EvaluationStudentsStaff_AspNetUsers_Updated");
            });

            modelBuilder.Entity<EvaluationType>(entity =>
            {
                entity.HasIndex(e => e.InsertedFrom, "IX_EvaluationType_InsertedFrom");

                entity.HasIndex(e => e.UpdatedFrom, "IX_EvaluationType_UpdatedFrom");

                entity.Property(e => e.EvaluationTypeId).HasColumnName("EvaluationTypeID");

                entity.Property(e => e.InsertedDate).HasColumnType("datetime");

                entity.Property(e => e.InsertedFrom).IsRequired();

                entity.Property(e => e.NameEn)
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasColumnName("NameEN");

                entity.Property(e => e.NameSq)
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasColumnName("NameSQ");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.InsertedFromNavigation)
                    .WithMany(p => p.EvaluationTypeInsertedFromNavigation)
                    .HasForeignKey(d => d.InsertedFrom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EvaluationType_AspNetUsers_Inserted");

                entity.HasOne(d => d.UpdatedFromNavigation)
                    .WithMany(p => p.EvaluationTypeUpdatedFromNavigation)
                    .HasForeignKey(d => d.UpdatedFrom)
                    .HasConstraintName("FK_EvaluationType_AspNetUsers_Updated");
            });

            modelBuilder.Entity<Gender>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.GenderId).HasColumnName("GenderID");

                entity.Property(e => e.NameEn)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("NameEN");

                entity.Property(e => e.NameSq)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("NameSQ");
            });

            modelBuilder.Entity<JobType>(entity =>
            {
                entity.Property(e => e.JobTypeId).HasColumnName("JobTypeID");

                entity.Property(e => e.NameEn)
                    .IsRequired()
                    .HasMaxLength(128)
                    .HasColumnName("NameEN");

                entity.Property(e => e.NameSq)
                    .IsRequired()
                    .HasMaxLength(128)
                    .HasColumnName("NameSQ");
            });

            modelBuilder.Entity<Leave>(entity =>
            {
                entity.HasIndex(e => e.LeaveTypeId, "IX_HolidayRequest_HolidayTypeID");

                entity.HasIndex(e => e.InsertedFrom, "IX_HolidayRequest_InsertedFrom");

                entity.HasIndex(e => e.StaffId, "IX_HolidayRequest_StaffDepartmentID");

                entity.HasIndex(e => e.UpdatedFrom, "IX_HolidayRequest_UpdatedFrom");

                entity.Property(e => e.LeaveId).HasColumnName("LeaveID");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.InsertedDate).HasColumnType("datetime");

                entity.Property(e => e.InsertedFrom).IsRequired();

                entity.Property(e => e.LeaveTypeId).HasColumnName("LeaveTypeID");

                entity.Property(e => e.StaffId).HasColumnName("StaffID");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.InsertedFromNavigation)
                    .WithMany(p => p.LeaveInsertedFromNavigation)
                    .HasForeignKey(d => d.InsertedFrom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Leave_AspNetUsers_Inserted");

                entity.HasOne(d => d.LeaveType)
                    .WithMany(p => p.Leave)
                    .HasForeignKey(d => d.LeaveTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Leave_LeaveType");

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.Leave)
                    .HasForeignKey(d => d.StaffId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Leave_Staff");

                entity.HasOne(d => d.UpdatedFromNavigation)
                    .WithMany(p => p.LeaveUpdatedFromNavigation)
                    .HasForeignKey(d => d.UpdatedFrom)
                    .HasConstraintName("FK_Leave_AspNetUsers_Updated");
            });

            modelBuilder.Entity<LeaveStaffDays>(entity =>
            {
                entity.HasKey(e => e.LeaveTypeRemainingDaysId);

                entity.Property(e => e.LeaveTypeRemainingDaysId).HasColumnName("LeaveTypeRemainingDaysID");

                entity.Property(e => e.InsertedDate).HasColumnType("datetime");

                entity.Property(e => e.InsertedFrom)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.LeaveTypeId).HasColumnName("LeaveTypeID");

                entity.Property(e => e.StaffId).HasColumnName("StaffID");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedFrom).HasMaxLength(450);

                entity.HasOne(d => d.InsertedFromNavigation)
                    .WithMany(p => p.LeaveStaffDaysInsertedFromNavigation)
                    .HasForeignKey(d => d.InsertedFrom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LeaveStaffDays_AspNetUsers_Inserted");

                entity.HasOne(d => d.LeaveType)
                    .WithMany(p => p.LeaveStaffDays)
                    .HasForeignKey(d => d.LeaveTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LeaveStaffDays_LeaveType");

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.LeaveStaffDays)
                    .HasForeignKey(d => d.StaffId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LeaveStaffDays_Staff");

                entity.HasOne(d => d.UpdatedFromNavigation)
                    .WithMany(p => p.LeaveStaffDaysUpdatedFromNavigation)
                    .HasForeignKey(d => d.UpdatedFrom)
                    .HasConstraintName("FK_LeaveStaffDays_AspNetUsers_Updated");
            });

            modelBuilder.Entity<LeaveStatus>(entity =>
            {
                entity.HasIndex(e => e.LeaveId, "IX_HolidayRequestStatus_HolidayRequestID");

                entity.HasIndex(e => e.InsertedFrom, "IX_HolidayRequestStatus_InsertedFrom");

                entity.HasIndex(e => e.StatusTypeId, "IX_HolidayRequestStatus_StatusTypeID");

                entity.HasIndex(e => e.UpdatedFrom, "IX_HolidayRequestStatus_UpdatedFrom");

                entity.Property(e => e.LeaveStatusId).HasColumnName("LeaveStatusID");

                entity.Property(e => e.Description).HasMaxLength(1024);

                entity.Property(e => e.InsertedDate).HasColumnType("datetime");

                entity.Property(e => e.InsertedFrom).IsRequired();

                entity.Property(e => e.LeaveId).HasColumnName("LeaveID");

                entity.Property(e => e.StatusTypeId).HasColumnName("StatusTypeID");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.InsertedFromNavigation)
                    .WithMany(p => p.LeaveStatusInsertedFromNavigation)
                    .HasForeignKey(d => d.InsertedFrom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LeaveStatus_AspNetUsers_Inserted");

                entity.HasOne(d => d.Leave)
                    .WithMany(p => p.LeaveStatus)
                    .HasForeignKey(d => d.LeaveId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LeaveStatus_Leave");

                entity.HasOne(d => d.StatusType)
                    .WithMany(p => p.LeaveStatus)
                    .HasForeignKey(d => d.StatusTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LeaveStatus_StatusType");

                entity.HasOne(d => d.UpdatedFromNavigation)
                    .WithMany(p => p.LeaveStatusUpdatedFromNavigation)
                    .HasForeignKey(d => d.UpdatedFrom)
                    .HasConstraintName("FK_LeaveStatus_AspNetUsers_Updated");
            });

            modelBuilder.Entity<LeaveType>(entity =>
            {
                entity.HasIndex(e => e.InsertedFrom, "IX_HolidayType_InsertedFrom");

                entity.HasIndex(e => e.UpdatedFrom, "IX_HolidayType_UpdatedFrom");

                entity.Property(e => e.LeaveTypeId).HasColumnName("LeaveTypeID");

                entity.Property(e => e.InsertedDate).HasColumnType("datetime");

                entity.Property(e => e.InsertedFrom).IsRequired();

                entity.Property(e => e.NameEn)
                    .IsRequired()
                    .HasMaxLength(128)
                    .HasColumnName("NameEN");

                entity.Property(e => e.NameSq)
                    .IsRequired()
                    .HasMaxLength(128)
                    .HasColumnName("NameSQ");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.InsertedFromNavigation)
                    .WithMany(p => p.LeaveTypeInsertedFromNavigation)
                    .HasForeignKey(d => d.InsertedFrom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LeaveType_AspNetUsers_Inserted");

                entity.HasOne(d => d.UpdatedFromNavigation)
                    .WithMany(p => p.LeaveTypeUpdatedFromNavigation)
                    .HasForeignKey(d => d.UpdatedFrom)
                    .HasConstraintName("FK_LeaveType_AspNetUsers_Updated");
            });

            modelBuilder.Entity<Log>(entity =>
            {
                entity.ToTable("Log", "Core");

                entity.Property(e => e.LogId).HasColumnName("LogID");

                entity.Property(e => e.Action)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Controller)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Description).HasMaxLength(128);

                entity.Property(e => e.Developer).HasMaxLength(50);

                entity.Property(e => e.FormContent).HasMaxLength(2048);

                entity.Property(e => e.HttpMethod)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.InsertedDate).HasColumnType("datetime");

                entity.Property(e => e.Ip)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasMaxLength(1024);

                entity.Property(e => e.UserId)
                    .HasMaxLength(450)
                    .HasColumnName("UserID");
            });

            modelBuilder.Entity<Menu>(entity =>
            {
                entity.ToTable("Menu", "Core");

                entity.HasIndex(e => e.InsertedFrom, "IX_Menu_InsertedFrom");

                entity.HasIndex(e => e.UpdatedFrom, "IX_Menu_UpdatedFrom");

                entity.Property(e => e.MenuId).HasColumnName("MenuID");

                entity.Property(e => e.Action).HasMaxLength(128);

                entity.Property(e => e.Area).HasMaxLength(128);

                entity.Property(e => e.Claim).HasMaxLength(128);

                entity.Property(e => e.ClaimType).HasMaxLength(128);

                entity.Property(e => e.Controller).HasMaxLength(128);

                entity.Property(e => e.Icon).HasMaxLength(128);

                entity.Property(e => e.InsertedDate).HasColumnType("datetime");

                entity.Property(e => e.InsertedFrom).IsRequired();

                entity.Property(e => e.NameEn)
                    .IsRequired()
                    .HasMaxLength(128)
                    .HasColumnName("NameEN");

                entity.Property(e => e.NameSq)
                    .IsRequired()
                    .HasMaxLength(128)
                    .HasColumnName("NameSQ");

                entity.Property(e => e.Roles).HasMaxLength(1024);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.InsertedFromNavigation)
                    .WithMany(p => p.MenuInsertedFromNavigation)
                    .HasForeignKey(d => d.InsertedFrom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Menu_AspNetUsers_Inserted");

                entity.HasOne(d => d.UpdatedFromNavigation)
                    .WithMany(p => p.MenuUpdatedFromNavigation)
                    .HasForeignKey(d => d.UpdatedFrom)
                    .HasConstraintName("FK_Menu_AspNetUsers_Updated");
            });

            modelBuilder.Entity<ProfessionType>(entity =>
            {
                entity.HasIndex(e => e.InsertedFrom, "IX_ProfessionType_InsertedFrom");

                entity.HasIndex(e => e.UpdatedFrom, "IX_ProfessionType_UpdatedFrom");

                entity.Property(e => e.ProfessionTypeId).HasColumnName("ProfessionTypeID");

                entity.Property(e => e.Code).HasMaxLength(24);

                entity.Property(e => e.InsertedDate).HasColumnType("datetime");

                entity.Property(e => e.InsertedFrom).IsRequired();

                entity.Property(e => e.NameEn)
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasColumnName("NameEN");

                entity.Property(e => e.NameSq)
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasColumnName("NameSQ");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.InsertedFromNavigation)
                    .WithMany(p => p.ProfessionTypeInsertedFromNavigation)
                    .HasForeignKey(d => d.InsertedFrom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProfessionType_AspNetUsers_Inserted");

                entity.HasOne(d => d.UpdatedFromNavigation)
                    .WithMany(p => p.ProfessionTypeUpdatedFromNavigation)
                    .HasForeignKey(d => d.UpdatedFrom)
                    .HasConstraintName("FK_ProfessionType_AspNetUsers_Updated");
            });

            modelBuilder.Entity<RateType>(entity =>
            {
                entity.HasIndex(e => e.InsertedFrom, "IX_RateType_InsertedFrom");

                entity.HasIndex(e => e.UpdatedFrom, "IX_RateType_UpdatedFrom");

                entity.Property(e => e.RateTypeId).HasColumnName("RateTypeID");

                entity.Property(e => e.InsertedDate).HasColumnType("datetime");

                entity.Property(e => e.InsertedFrom).IsRequired();

                entity.Property(e => e.NameEn)
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasColumnName("NameEN");

                entity.Property(e => e.NameSq)
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasColumnName("NameSQ");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.InsertedFromNavigation)
                    .WithMany(p => p.RateTypeInsertedFromNavigation)
                    .HasForeignKey(d => d.InsertedFrom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RateType_AspNetUsers_Insert");

                entity.HasOne(d => d.UpdatedFromNavigation)
                    .WithMany(p => p.RateTypeUpdatedFromNavigation)
                    .HasForeignKey(d => d.UpdatedFrom)
                    .HasConstraintName("FK_RateType_AspNetUsers_Update");
            });

            modelBuilder.Entity<RealRole>(entity =>
            {
                entity.ToTable("RealRole", "Core");

                entity.HasIndex(e => e.InsertedFrom, "IX_RealRole_InsertedFrom");

                entity.HasIndex(e => e.RoleId, "IX_RealRole_RoleID");

                entity.HasIndex(e => e.UpdatedFrom, "IX_RealRole_UpdatedFrom");

                entity.HasIndex(e => e.UserId, "IX_RealRole_UserID");

                entity.Property(e => e.RealRoleId).HasColumnName("RealRoleID");

                entity.Property(e => e.InsertedDate).HasColumnType("datetime");

                entity.Property(e => e.InsertedFrom).IsRequired();

                entity.Property(e => e.RoleId)
                    .IsRequired()
                    .HasColumnName("RoleID");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("UserID");

                entity.HasOne(d => d.InsertedFromNavigation)
                    .WithMany(p => p.RealRoleInsertedFromNavigation)
                    .HasForeignKey(d => d.InsertedFrom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RealRole_AspNetUsers_Inserted");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.RealRole)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RealRole_AspNetRoles");

                entity.HasOne(d => d.UpdatedFromNavigation)
                    .WithMany(p => p.RealRoleUpdatedFromNavigation)
                    .HasForeignKey(d => d.UpdatedFrom)
                    .HasConstraintName("FK_RealRole_AspNetUsers_Updated");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.RealRoleUser)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RealRole_AspNetUsers");
            });

            modelBuilder.Entity<Staff>(entity =>
            {
                entity.HasIndex(e => e.UpdatedFrom, "IX_Staff_UpdatedFrom");

                entity.HasIndex(e => e.UserId, "IX_Staff_UserID");

                entity.Property(e => e.StaffId).HasColumnName("StaffID");

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.BirthPlace).HasMaxLength(128);

                entity.Property(e => e.Birthdate).HasColumnType("date");

                entity.Property(e => e.City).HasMaxLength(128);

                entity.Property(e => e.Country).HasMaxLength(128);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(1024);

                entity.Property(e => e.InsertedDate).HasColumnType("datetime");

                entity.Property(e => e.InsertedFrom)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(1024);

                entity.Property(e => e.Nationality)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.PersonalNumber)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.Property(e => e.PostalCode).HasMaxLength(8);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("UserID");

                entity.HasOne(d => d.UpdatedFromNavigation)
                    .WithMany(p => p.StaffUpdatedFromNavigation)
                    .HasForeignKey(d => d.UpdatedFrom)
                    .HasConstraintName("FK_Staff_AspNetUsers_Updated");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.StaffUser)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Staff_AspNetUsers");
            });

            modelBuilder.Entity<StaffDepartment>(entity =>
            {
                entity.HasIndex(e => e.DepartmentId, "IX_StaffDepartment_DepartmentID");

                entity.HasIndex(e => e.InsertedFrom, "IX_StaffDepartment_InsertedFrom");

                entity.HasIndex(e => e.StaffId, "IX_StaffDepartment_StaffID");

                entity.HasIndex(e => e.StaffTypeId, "IX_StaffDepartment_StaffTypeID");

                entity.HasIndex(e => e.UpdatedFrom, "IX_StaffDepartment_UpdatedFrom");

                entity.Property(e => e.StaffDepartmentId).HasColumnName("StaffDepartmentID");

                entity.Property(e => e.BruttoSalary).HasColumnType("money");

                entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");

                entity.Property(e => e.EmployeeContribution).HasColumnType("decimal(5, 2)");

                entity.Property(e => e.EmployerContribution).HasColumnType("decimal(5, 2)");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.InsertedDate).HasColumnType("datetime");

                entity.Property(e => e.InsertedFrom).IsRequired();

                entity.Property(e => e.JobTypeId).HasColumnName("JobTypeID");

                entity.Property(e => e.StaffId).HasColumnName("StaffID");

                entity.Property(e => e.StaffTypeId).HasColumnName("StaffTypeID");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.StaffDepartment)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StaffDepartment_Department");

                entity.HasOne(d => d.InsertedFromNavigation)
                    .WithMany(p => p.StaffDepartmentInsertedFromNavigation)
                    .HasForeignKey(d => d.InsertedFrom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StaffDepartment_AspNetUsers_Inserted");

                entity.HasOne(d => d.JobType)
                    .WithMany(p => p.StaffDepartment)
                    .HasForeignKey(d => d.JobTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StaffDepartment_JobType");

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.StaffDepartment)
                    .HasForeignKey(d => d.StaffId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StaffDepartment_Staff");

                entity.HasOne(d => d.StaffType)
                    .WithMany(p => p.StaffDepartment)
                    .HasForeignKey(d => d.StaffTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StaffDepartment_StaffType");

                entity.HasOne(d => d.UpdatedFromNavigation)
                    .WithMany(p => p.StaffDepartmentUpdatedFromNavigation)
                    .HasForeignKey(d => d.UpdatedFrom)
                    .HasConstraintName("FK_StaffDepartment_AspNetUsers_Updated");
            });

            modelBuilder.Entity<StaffDepartmentSubject>(entity =>
            {
                entity.HasIndex(e => e.InsertedFrom, "IX_StaffDepartmentSubject_InsertedFrom");

                entity.HasIndex(e => e.StaffDepartmentId, "IX_StaffDepartmentSubject_StaffDepartmentID");

                entity.HasIndex(e => e.SubjectId, "IX_StaffDepartmentSubject_SubjectID");

                entity.HasIndex(e => e.UpdatedFrom, "IX_StaffDepartmentSubject_UpdatedFrom");

                entity.Property(e => e.StaffDepartmentSubjectId).HasColumnName("StaffDepartmentSubjectID");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.InsertedDate).HasColumnType("datetime");

                entity.Property(e => e.InsertedFrom).IsRequired();

                entity.Property(e => e.StaffDepartmentId).HasColumnName("StaffDepartmentID");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.SubjectId).HasColumnName("SubjectID");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.InsertedFromNavigation)
                    .WithMany(p => p.StaffDepartmentSubjectInsertedFromNavigation)
                    .HasForeignKey(d => d.InsertedFrom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StaffDepartmentSubject_AspNetUsers_Inserted");

                entity.HasOne(d => d.StaffDepartment)
                    .WithMany(p => p.StaffDepartmentSubject)
                    .HasForeignKey(d => d.StaffDepartmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StaffDepartmentSubject_StaffDepartment");

                entity.HasOne(d => d.Subject)
                    .WithMany(p => p.StaffDepartmentSubject)
                    .HasForeignKey(d => d.SubjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StaffDepartmentSubject_Subject");

                entity.HasOne(d => d.UpdatedFromNavigation)
                    .WithMany(p => p.StaffDepartmentSubjectUpdatedFromNavigation)
                    .HasForeignKey(d => d.UpdatedFrom)
                    .HasConstraintName("FK_StaffDepartmentSubject_AspNetUsers_Updated");
            });

            modelBuilder.Entity<StaffDocument>(entity =>
            {
                entity.HasIndex(e => e.DocumentTypeId, "IX_StaffDocument_DocumentTypeID");

                entity.HasIndex(e => e.InsertedFrom, "IX_StaffDocument_InsertedFrom");

                entity.HasIndex(e => e.StaffId, "IX_StaffDocument_StaffID");

                entity.HasIndex(e => e.UpdatedFrom, "IX_StaffDocument_UpdatedFrom");

                entity.Property(e => e.StaffDocumentId).HasColumnName("StaffDocumentID");

                entity.Property(e => e.DocumentTypeId).HasColumnName("DocumentTypeID");

                entity.Property(e => e.InsertedDate).HasColumnType("datetime");

                entity.Property(e => e.InsertedFrom).IsRequired();

                entity.Property(e => e.Path).HasMaxLength(2048);

                entity.Property(e => e.StaffId).HasColumnName("StaffID");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.DocumentType)
                    .WithMany(p => p.StaffDocument)
                    .HasForeignKey(d => d.DocumentTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StaffDocument_Document");

                entity.HasOne(d => d.InsertedFromNavigation)
                    .WithMany(p => p.StaffDocumentInsertedFromNavigation)
                    .HasForeignKey(d => d.InsertedFrom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StaffDocument_AspNetUsers_Inserted");

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.StaffDocument)
                    .HasForeignKey(d => d.StaffId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StaffDocument_Staff");

                entity.HasOne(d => d.UpdatedFromNavigation)
                    .WithMany(p => p.StaffDocumentUpdatedFromNavigation)
                    .HasForeignKey(d => d.UpdatedFrom)
                    .HasConstraintName("FK_StaffDocument_AspNetUsers_Updated");
            });

            modelBuilder.Entity<StaffQualification>(entity =>
            {
                entity.HasIndex(e => e.EducationLevelTypeId, "IX_StaffQualification_EducationLevelTypeID");

                entity.HasIndex(e => e.InsertedFrom, "IX_StaffQualification_InsertedFrom");

                entity.HasIndex(e => e.ProfessionTypeId, "IX_StaffQualification_ProfessionTypeID");

                entity.HasIndex(e => e.StaffId, "IX_StaffQualification_StaffID");

                entity.HasIndex(e => e.UpdatedFrom, "IX_StaffQualification_UpdatedFrom");

                entity.Property(e => e.StaffQualificationId).HasColumnName("StaffQualificationID");

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.Country)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.CreditType).HasMaxLength(128);

                entity.Property(e => e.Description).HasMaxLength(2048);

                entity.Property(e => e.EducationLevelTypeId).HasColumnName("EducationLevelTypeID");

                entity.Property(e => e.FieldStudy).HasMaxLength(128);

                entity.Property(e => e.FinalGrade).HasColumnType("decimal(4, 2)");

                entity.Property(e => e.From).HasColumnType("datetime");

                entity.Property(e => e.InsertedDate).HasColumnType("datetime");

                entity.Property(e => e.InsertedFrom).IsRequired();

                entity.Property(e => e.ProfessionTypeId).HasColumnName("ProfessionTypeID");

                entity.Property(e => e.StaffId).HasColumnName("StaffID");

                entity.Property(e => e.Thesis).HasMaxLength(128);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.To).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.Validity).HasColumnType("datetime");

                entity.HasOne(d => d.EducationLevelType)
                    .WithMany(p => p.StaffQualification)
                    .HasForeignKey(d => d.EducationLevelTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StaffQualification_EducationLevelType");

                entity.HasOne(d => d.InsertedFromNavigation)
                    .WithMany(p => p.StaffQualificationInsertedFromNavigation)
                    .HasForeignKey(d => d.InsertedFrom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StaffQualification_AspNetUsers_Inserted");

                entity.HasOne(d => d.ProfessionType)
                    .WithMany(p => p.StaffQualification)
                    .HasForeignKey(d => d.ProfessionTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StaffQualification_ProfessionType");

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.StaffQualification)
                    .HasForeignKey(d => d.StaffId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StaffQualification_Staff");

                entity.HasOne(d => d.UpdatedFromNavigation)
                    .WithMany(p => p.StaffQualificationUpdatedFromNavigation)
                    .HasForeignKey(d => d.UpdatedFrom)
                    .HasConstraintName("FK_StaffQualification_AspNetUsers_Updated");
            });

            modelBuilder.Entity<StaffRegistrationStatus>(entity =>
            {
                entity.Property(e => e.StaffRegistrationStatusId).HasColumnName("StaffRegistrationStatusID");

                entity.Property(e => e.InsertedDate).HasColumnType("datetime");

                entity.Property(e => e.InsertedFrom)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.StaffId).HasColumnName("StaffID");

                entity.Property(e => e.StatusTypeId).HasColumnName("StatusTypeID");

                entity.HasOne(d => d.InsertedFromNavigation)
                    .WithMany(p => p.StaffRegistrationStatus)
                    .HasForeignKey(d => d.InsertedFrom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StaffRegistrationStatus_AspNetUsers_Inserted");

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.StaffRegistrationStatus)
                    .HasForeignKey(d => d.StaffId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StaffRegistrationStatus_Staff");

                entity.HasOne(d => d.StatusType)
                    .WithMany(p => p.StaffRegistrationStatus)
                    .HasForeignKey(d => d.StatusTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StaffRegistrationStatus_StatusType");
            });

            modelBuilder.Entity<StaffType>(entity =>
            {
                entity.HasIndex(e => e.InsertedFrom, "IX_StaffType_InsertedFrom");

                entity.HasIndex(e => e.UpdatedFrom, "IX_StaffType_UpdatedFrom");

                entity.Property(e => e.StaffTypeId).HasColumnName("StaffTypeID");

                entity.Property(e => e.InsertedDate).HasColumnType("datetime");

                entity.Property(e => e.InsertedFrom).IsRequired();

                entity.Property(e => e.NameEn)
                    .HasMaxLength(128)
                    .HasColumnName("NameEN");

                entity.Property(e => e.NameSq)
                    .IsRequired()
                    .HasMaxLength(128)
                    .HasColumnName("NameSQ");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.InsertedFromNavigation)
                    .WithMany(p => p.StaffTypeInsertedFromNavigation)
                    .HasForeignKey(d => d.InsertedFrom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StaffType_AspNetUsers_Inserted");

                entity.HasOne(d => d.UpdatedFromNavigation)
                    .WithMany(p => p.StaffTypeUpdatedFromNavigation)
                    .HasForeignKey(d => d.UpdatedFrom)
                    .HasConstraintName("FK_StaffType_AspNetUsers_Updated");
            });

            modelBuilder.Entity<StatusType>(entity =>
            {
                entity.HasIndex(e => e.InsertedFrom, "IX_StatusType_InsertedFrom");

                entity.HasIndex(e => e.UpdatedFrom, "IX_StatusType_UpdatedFrom");

                entity.Property(e => e.StatusTypeId).HasColumnName("StatusTypeID");

                entity.Property(e => e.InsertedDate).HasColumnType("datetime");

                entity.Property(e => e.InsertedFrom).IsRequired();

                entity.Property(e => e.NameEn)
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasColumnName("NameEN");

                entity.Property(e => e.NameSq)
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasColumnName("NameSQ");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.InsertedFromNavigation)
                    .WithMany(p => p.StatusTypeInsertedFromNavigation)
                    .HasForeignKey(d => d.InsertedFrom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StatusType_AspNetUsers_Inserted");

                entity.HasOne(d => d.UpdatedFromNavigation)
                    .WithMany(p => p.StatusTypeUpdatedFromNavigation)
                    .HasForeignKey(d => d.UpdatedFrom)
                    .HasConstraintName("FK_StatusType_AspNetUsers_Updated");
            });

            modelBuilder.Entity<SubMenu>(entity =>
            {
                entity.ToTable("SubMenu", "Core");

                entity.HasIndex(e => e.InsertedFrom, "IX_SubMenu_InsertedFrom");

                entity.HasIndex(e => e.MenuId, "IX_SubMenu_MenuID");

                entity.HasIndex(e => e.UpdatedFrom, "IX_SubMenu_UpdatedFrom");

                entity.Property(e => e.SubMenuId).HasColumnName("SubMenuID");

                entity.Property(e => e.Action).HasMaxLength(128);

                entity.Property(e => e.Area).HasMaxLength(128);

                entity.Property(e => e.Claim).HasMaxLength(128);

                entity.Property(e => e.ClaimType).HasMaxLength(128);

                entity.Property(e => e.Controller).HasMaxLength(128);

                entity.Property(e => e.Icon)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.InsertedDate).HasColumnType("datetime");

                entity.Property(e => e.InsertedFrom).IsRequired();

                entity.Property(e => e.MenuId).HasColumnName("MenuID");

                entity.Property(e => e.NameEn)
                    .IsRequired()
                    .HasMaxLength(128)
                    .HasColumnName("NameEN");

                entity.Property(e => e.NameSq)
                    .IsRequired()
                    .HasMaxLength(128)
                    .HasColumnName("NameSQ");

                entity.Property(e => e.Roles).HasMaxLength(1024);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.InsertedFromNavigation)
                    .WithMany(p => p.SubMenuInsertedFromNavigation)
                    .HasForeignKey(d => d.InsertedFrom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SubMenu_AspNetUsers_Inserted");

                entity.HasOne(d => d.Menu)
                    .WithMany(p => p.SubMenu)
                    .HasForeignKey(d => d.MenuId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SubMenu_Menu");

                entity.HasOne(d => d.UpdatedFromNavigation)
                    .WithMany(p => p.SubMenuUpdatedFromNavigation)
                    .HasForeignKey(d => d.UpdatedFrom)
                    .HasConstraintName("FK_SubMenu_AspNetUsers_Updated");
            });

            modelBuilder.Entity<Subject>(entity =>
            {
                entity.HasIndex(e => e.InsertedFrom, "IX_Subject_InsertedFrom");

                entity.HasIndex(e => e.UpdatedFrom, "IX_Subject_UpdatedFrom");

                entity.Property(e => e.SubjectId).HasColumnName("SubjectID");

                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.InsertedDate).HasColumnType("datetime");

                entity.Property(e => e.InsertedFrom).IsRequired();

                entity.Property(e => e.NameEn)
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasColumnName("NameEN");

                entity.Property(e => e.NameSq)
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasColumnName("NameSQ");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.InsertedFromNavigation)
                    .WithMany(p => p.SubjectInsertedFromNavigation)
                    .HasForeignKey(d => d.InsertedFrom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Subject_AspNetUsers_Inserted");

                entity.HasOne(d => d.UpdatedFromNavigation)
                    .WithMany(p => p.SubjectUpdatedFromNavigation)
                    .HasForeignKey(d => d.UpdatedFrom)
                    .HasConstraintName("FK_Subject_AspNetUsers_Updated");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
