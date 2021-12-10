using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

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

        public virtual DbSet<AspNetRoleClaims> AspNetRoleClaims { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUserTokens> AspNetUserTokens { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<Gender> Gender { get; set; }
        public virtual DbSet<Log> Log { get; set; }
        public virtual DbSet<Menu> Menu { get; set; }
        public virtual DbSet<StatusType> StatusType { get; set; }
        public virtual DbSet<SubMenu> SubMenu { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=HRMS;Trusted_Connection=True;MultipleActiveResultSets=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

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

            modelBuilder.Entity<AspNetUserRoles>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasIndex(e => e.RoleId, "IX_AspNetUserRoles_RoleId");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserRoles)
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

                entity.Property(e => e.PasswordHash).IsRequired();

                entity.Property(e => e.PersonalNumber)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.PhoneNumber).IsRequired();

                entity.Property(e => e.ProfileImage)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(256);
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

            modelBuilder.Entity<Log>(entity =>
            {
                entity.Property(e => e.LogId).HasColumnName("LogID");

                entity.Property(e => e.Action)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Controller)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Description).HasMaxLength(128);

                entity.Property(e => e.FormContent).HasMaxLength(1024);

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
                entity.Property(e => e.MenuId).HasColumnName("MenuID");

                entity.Property(e => e.Action).HasMaxLength(128);

                entity.Property(e => e.Claim).HasMaxLength(128);

                entity.Property(e => e.ClaimType).HasMaxLength(128);

                entity.Property(e => e.Controller).HasMaxLength(128);

                entity.Property(e => e.Icon).HasMaxLength(128);

                entity.Property(e => e.InsertedDate).HasColumnType("datetime");

                entity.Property(e => e.InsertedFrom)
                    .IsRequired()
                    .HasMaxLength(450);

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

                entity.Property(e => e.UpdatedFrom).HasMaxLength(450);

                entity.HasOne(d => d.InsertedFromNavigation)
                    .WithMany(p => p.MenuInsertedFromNavigation)
                    .HasForeignKey(d => d.InsertedFrom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Menu_AspNetUsers_Insert");

                entity.HasOne(d => d.UpdatedFromNavigation)
                    .WithMany(p => p.MenuUpdatedFromNavigation)
                    .HasForeignKey(d => d.UpdatedFrom)
                    .HasConstraintName("FK_Menu_AspNetUsers_Update");
            });

            modelBuilder.Entity<StatusType>(entity =>
            {
                entity.Property(e => e.StatusTypeId)
                    .ValueGeneratedNever()
                    .HasColumnName("StatusTypeID");

                entity.Property(e => e.InsertedDate).HasColumnType("datetime");

                entity.Property(e => e.InsertedFrom)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.NameEn)
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasColumnName("NameEN");

                entity.Property(e => e.NameSq)
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasColumnName("NameSQ");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedFrom).HasMaxLength(450);

                entity.HasOne(d => d.InsertedFromNavigation)
                    .WithMany(p => p.StatusTypeInsertedFromNavigation)
                    .HasForeignKey(d => d.InsertedFrom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StatusType_AspNetUsers_Insert");

                entity.HasOne(d => d.UpdatedFromNavigation)
                    .WithMany(p => p.StatusTypeUpdatedFromNavigation)
                    .HasForeignKey(d => d.UpdatedFrom)
                    .HasConstraintName("FK_StatusType_AspNetUsers_Update");
            });

            modelBuilder.Entity<SubMenu>(entity =>
            {
                entity.Property(e => e.SubMenuId).HasColumnName("SubMenuID");

                entity.Property(e => e.Action).HasMaxLength(128);

                entity.Property(e => e.Claim).HasMaxLength(128);

                entity.Property(e => e.ClaimType).HasMaxLength(128);

                entity.Property(e => e.Controller).HasMaxLength(128);

                entity.Property(e => e.Icon)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.InsertedDate).HasColumnType("datetime");

                entity.Property(e => e.InsertedFrom)
                    .IsRequired()
                    .HasMaxLength(450);

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

                entity.Property(e => e.UpdatedFrom).HasMaxLength(450);

                entity.HasOne(d => d.InsertedFromNavigation)
                    .WithMany(p => p.SubMenuInsertedFromNavigation)
                    .HasForeignKey(d => d.InsertedFrom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SubMenu_AspNetUsers_Insert");

                entity.HasOne(d => d.Menu)
                    .WithMany(p => p.SubMenu)
                    .HasForeignKey(d => d.MenuId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SubMenu_Menu");

                entity.HasOne(d => d.UpdatedFromNavigation)
                    .WithMany(p => p.SubMenuUpdatedFromNavigation)
                    .HasForeignKey(d => d.UpdatedFrom)
                    .HasConstraintName("FK_SubMenu_AspNetUsers_Update");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
