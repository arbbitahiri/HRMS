using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.Data.Migrations
{
    public partial class HRMSWork : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "History");

            migrationBuilder.EnsureSchema(
                name: "Core");

            migrationBuilder.CreateTable(
                name: "AppSettings",
                schema: "History",
                columns: table => new
                {
                    HistoryAppSettingsID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OldVersion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedVersion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InsertedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    IndertedDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSettings", x => x.HistoryAppSettingsID);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NameSQ = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    NameEN = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    DescriptionSQ = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    DescriptionEN = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PersonalNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Birthdate = table.Column<DateTime>(type: "date", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AllowNotification = table.Column<bool>(type: "bit", nullable: false),
                    Language = table.Column<int>(type: "int", nullable: false),
                    Mode = table.Column<int>(type: "int", nullable: false),
                    ProfileImage = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false),
                    InsertedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    InsertedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                schema: "History",
                columns: table => new
                {
                    HistoryAspNetUsersID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    PersonalNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Birthdate = table.Column<DateTime>(type: "date", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AllowNotification = table.Column<bool>(type: "bit", nullable: false),
                    Language = table.Column<int>(type: "int", nullable: false),
                    Mode = table.Column<int>(type: "int", nullable: false),
                    ProfileImage = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    InsertedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    InsertedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers_1", x => x.HistoryAspNetUsersID);
                });

            migrationBuilder.CreateTable(
                name: "Gender",
                columns: table => new
                {
                    GenderID = table.Column<int>(type: "int", nullable: false),
                    NameSQ = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NameEN = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "Log",
                schema: "Core",
                columns: table => new
                {
                    LogID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Ip = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Controller = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    HttpMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    FormContent = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    Exception = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InsertedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Error = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log", x => x.LogID);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Department",
                columns: table => new
                {
                    DepartmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NameSQ = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    NameEN = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    InsertedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    InsertedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedNo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Department", x => x.DepartmentID);
                    table.ForeignKey(
                        name: "FK_Department_AspNetUsers_Insert",
                        column: x => x.InsertedFrom,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Department_AspNetUsers_Update",
                        column: x => x.UpdatedFrom,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DocumentType",
                columns: table => new
                {
                    DocumentTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameSQ = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    NameEN = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    InsertedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    InsertedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedNo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentType", x => x.DocumentTypeID);
                    table.ForeignKey(
                        name: "FK_DocumentType_AspNetUsers_Insert",
                        column: x => x.InsertedFrom,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DocumentType_AspNetUsers_Update",
                        column: x => x.UpdatedFrom,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EducationLevelType",
                columns: table => new
                {
                    EducationLevelTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameSQ = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    NameEN = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    InsertedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    InsertedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedNo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationLevelType", x => x.EducationLevelTypeID);
                    table.ForeignKey(
                        name: "FK_EducationLevelType_AspNetUsers_Insert",
                        column: x => x.InsertedFrom,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EducationLevelType_AspNetUsers_Update",
                        column: x => x.UpdatedFrom,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EvaluationType",
                columns: table => new
                {
                    EvaluationTypeID = table.Column<int>(type: "int", nullable: false),
                    NameSQ = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    NameEN = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    InsertedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    InsertedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedNo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluationType", x => x.EvaluationTypeID);
                    table.ForeignKey(
                        name: "FK_EvaluationType_AspNetUsers_Insert",
                        column: x => x.InsertedFrom,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EvaluationType_AspNetUsers_Update",
                        column: x => x.UpdatedFrom,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HolidayType",
                columns: table => new
                {
                    HolidayTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameSQ = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    NameEN = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    InsertedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    InsertedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedNo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HolidayType", x => x.HolidayTypeID);
                    table.ForeignKey(
                        name: "FK_HolidayType_AspNetUsers_Insert",
                        column: x => x.InsertedFrom,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HolidayType_AspNetUsers_Update",
                        column: x => x.UpdatedFrom,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Menu",
                schema: "Core",
                columns: table => new
                {
                    MenuID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameSQ = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    NameEN = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    HasSubMenu = table.Column<bool>(type: "bit", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Claim = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ClaimType = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Controller = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Action = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    OrdinalNumber = table.Column<int>(type: "int", nullable: false),
                    Roles = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    OpenFor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InsertedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    InsertedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedNo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menu", x => x.MenuID);
                    table.ForeignKey(
                        name: "FK_Menu_AspNetUsers_Insert",
                        column: x => x.InsertedFrom,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Menu_AspNetUsers_Update",
                        column: x => x.UpdatedFrom,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProfessionType",
                columns: table => new
                {
                    ProfessionTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(24)", maxLength: 24, nullable: true),
                    NameSQ = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    NameEN = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    InsertedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    InsertedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedNo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfessionType", x => x.ProfessionTypeID);
                    table.ForeignKey(
                        name: "FK_ProfessionType_AspNetUsers_Insert",
                        column: x => x.InsertedFrom,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProfessionType_AspNetUsers_Update",
                        column: x => x.UpdatedFrom,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RateType",
                columns: table => new
                {
                    RateTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RateNumber = table.Column<int>(type: "int", nullable: false),
                    NameSQ = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    NameEN = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    InsertedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    InsertedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedNo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RateType", x => x.RateTypeID);
                    table.ForeignKey(
                        name: "FK_RateType_AspNetUsers_Insert",
                        column: x => x.InsertedFrom,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RateType_AspNetUsers_Update",
                        column: x => x.UpdatedFrom,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RealRole",
                schema: "Core",
                columns: table => new
                {
                    RealRoleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    RoleID = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    InsertedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    InsertedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedNo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RealRole", x => x.RealRoleID);
                    table.ForeignKey(
                        name: "FK_RealRole_AspNetRoles",
                        column: x => x.RoleID,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RealRole_AspNetUsers",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RealRole_AspNetUsers_Insert",
                        column: x => x.InsertedFrom,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RealRole_AspNetUsers_Update",
                        column: x => x.UpdatedFrom,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Staff",
                columns: table => new
                {
                    StaffID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    PersonalNumber = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    Birthdate = table.Column<DateTime>(type: "date", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    BirthPlace = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    City = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    Nationality = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    InsertedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    InsertedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedNo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staff", x => x.StaffID);
                    table.ForeignKey(
                        name: "FK_Staff_AspNetUsers_Insert",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Staff_AspNetUsers_Update",
                        column: x => x.UpdatedFrom,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StaffType",
                columns: table => new
                {
                    StaffTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameSQ = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    NameEN = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    InsertedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    InsertedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedNo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffType", x => x.StaffTypeID);
                    table.ForeignKey(
                        name: "FK_StaffType_AspNetUsers_Insert",
                        column: x => x.InsertedFrom,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StaffType_AspNetUsers_Update",
                        column: x => x.UpdatedFrom,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StatusType",
                columns: table => new
                {
                    StatusTypeID = table.Column<int>(type: "int", nullable: false),
                    NameSQ = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    NameEN = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    InsertedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    InsertedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedNo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusType", x => x.StatusTypeID);
                    table.ForeignKey(
                        name: "FK_StatusType_AspNetUsers_Insert",
                        column: x => x.InsertedFrom,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StatusType_AspNetUsers_Update",
                        column: x => x.UpdatedFrom,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Subject",
                columns: table => new
                {
                    SubjectID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NameSQ = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    NameEN = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    InsertedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    InsertedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedNo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subject", x => x.SubjectID);
                    table.ForeignKey(
                        name: "FK_Subject_AspNetUsers_Insert",
                        column: x => x.InsertedFrom,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Subject_AspNetUsers_Update",
                        column: x => x.UpdatedFrom,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Document",
                columns: table => new
                {
                    DocumentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentTypeID = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    InsertedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    InsertedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedNo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Document", x => x.DocumentID);
                    table.ForeignKey(
                        name: "FK_Document_AspNetUsers_Insert",
                        column: x => x.InsertedFrom,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Document_AspNetUsers_Update",
                        column: x => x.UpdatedFrom,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Document_DocumentType",
                        column: x => x.DocumentTypeID,
                        principalTable: "DocumentType",
                        principalColumn: "DocumentTypeID");
                });

            migrationBuilder.CreateTable(
                name: "SubMenu",
                schema: "Core",
                columns: table => new
                {
                    SubMenuID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MenuID = table.Column<int>(type: "int", nullable: false),
                    NameSQ = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    NameEN = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Claim = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ClaimType = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Controller = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Action = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    OrdinalNumber = table.Column<int>(type: "int", nullable: false),
                    Roles = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    OpenFor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InsertedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    InsertedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedNo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubMenu", x => x.SubMenuID);
                    table.ForeignKey(
                        name: "FK_SubMenu_AspNetUsers_Insert",
                        column: x => x.InsertedFrom,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SubMenu_AspNetUsers_Update",
                        column: x => x.UpdatedFrom,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SubMenu_Menu",
                        column: x => x.MenuID,
                        principalSchema: "Core",
                        principalTable: "Menu",
                        principalColumn: "MenuID");
                });

            migrationBuilder.CreateTable(
                name: "StaffDocument",
                columns: table => new
                {
                    StaffDocumentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StaffID = table.Column<int>(type: "int", nullable: false),
                    DocumentTypeID = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Path = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    InsertedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    InsertedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedNo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffDocument", x => x.StaffDocumentID);
                    table.ForeignKey(
                        name: "FK_StaffDocument_AspNetUsers_Insert",
                        column: x => x.InsertedFrom,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StaffDocument_AspNetUsers_Update",
                        column: x => x.UpdatedFrom,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StaffDocument_Document",
                        column: x => x.DocumentTypeID,
                        principalTable: "DocumentType",
                        principalColumn: "DocumentTypeID");
                    table.ForeignKey(
                        name: "FK_StaffDocument_Staff",
                        column: x => x.StaffID,
                        principalTable: "Staff",
                        principalColumn: "StaffID");
                });

            migrationBuilder.CreateTable(
                name: "StaffQualification",
                columns: table => new
                {
                    StaffQualificationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StaffID = table.Column<int>(type: "int", nullable: false),
                    ProfessionTypeID = table.Column<int>(type: "int", nullable: false),
                    EducationLevelTypeID = table.Column<int>(type: "int", nullable: false),
                    Training = table.Column<bool>(type: "bit", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    FieldStudy = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    City = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    From = table.Column<DateTime>(type: "datetime", nullable: false),
                    To = table.Column<DateTime>(type: "datetime", nullable: true),
                    OnGoing = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FinalGrade = table.Column<decimal>(type: "decimal(4,2)", nullable: true),
                    Thesis = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreditType = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreditNumber = table.Column<int>(type: "int", nullable: true),
                    Validity = table.Column<DateTime>(type: "datetime", nullable: true),
                    InsertedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    InsertedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedNo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffQualification", x => x.StaffQualificationID);
                    table.ForeignKey(
                        name: "FK_StaffQualification_AspNetUsers_Insert",
                        column: x => x.InsertedFrom,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StaffQualification_AspNetUsers_Update",
                        column: x => x.UpdatedFrom,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StaffQualification_EducationLevelType",
                        column: x => x.EducationLevelTypeID,
                        principalTable: "EducationLevelType",
                        principalColumn: "EducationLevelTypeID");
                    table.ForeignKey(
                        name: "FK_StaffQualification_ProfessionType",
                        column: x => x.ProfessionTypeID,
                        principalTable: "ProfessionType",
                        principalColumn: "ProfessionTypeID");
                    table.ForeignKey(
                        name: "FK_StaffQualification_Staff",
                        column: x => x.StaffID,
                        principalTable: "Staff",
                        principalColumn: "StaffID");
                });

            migrationBuilder.CreateTable(
                name: "StaffDepartment",
                columns: table => new
                {
                    StaffDepartmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentID = table.Column<int>(type: "int", nullable: false),
                    StaffID = table.Column<int>(type: "int", nullable: false),
                    StaffTypeID = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InsertedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    InsertedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedNo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffDepartment", x => x.StaffDepartmentID);
                    table.ForeignKey(
                        name: "FK_StaffDepartment_AspNetUsers_Insert",
                        column: x => x.InsertedFrom,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StaffDepartment_AspNetUsers_Update",
                        column: x => x.UpdatedFrom,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StaffDepartment_Department",
                        column: x => x.DepartmentID,
                        principalTable: "Department",
                        principalColumn: "DepartmentID");
                    table.ForeignKey(
                        name: "FK_StaffDepartment_Staff",
                        column: x => x.StaffID,
                        principalTable: "Staff",
                        principalColumn: "StaffID");
                    table.ForeignKey(
                        name: "FK_StaffDepartment_StaffType",
                        column: x => x.StaffTypeID,
                        principalTable: "StaffType",
                        principalColumn: "StaffTypeID");
                });

            migrationBuilder.CreateTable(
                name: "HolidayRequest",
                columns: table => new
                {
                    HolidayRequestID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HolidayTypeID = table.Column<int>(type: "int", nullable: false),
                    StaffDepartmentID = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    InsertedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    InsertedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedNo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HolidayRequest", x => x.HolidayRequestID);
                    table.ForeignKey(
                        name: "FK_HolidayRequest_AspNetUsers_Insert",
                        column: x => x.InsertedFrom,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HolidayRequest_AspNetUsers_Update",
                        column: x => x.UpdatedFrom,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HolidayRequest_HolidayType",
                        column: x => x.HolidayTypeID,
                        principalTable: "HolidayType",
                        principalColumn: "HolidayTypeID");
                    table.ForeignKey(
                        name: "FK_HolidayRequest_StaffDepartment",
                        column: x => x.StaffDepartmentID,
                        principalTable: "StaffDepartment",
                        principalColumn: "StaffDepartmentID");
                });

            migrationBuilder.CreateTable(
                name: "StaffDepartmentEvaluation",
                columns: table => new
                {
                    StaffDepartmentEvaluationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StaffDepartmentID = table.Column<int>(type: "int", nullable: false),
                    EvaluationTypeID = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    InsertedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    InsertedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedNo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffDepartmentEvaluation", x => x.StaffDepartmentEvaluationID);
                    table.ForeignKey(
                        name: "FK_StaffDepartmentEvaluation_AspNetUsers_Insert",
                        column: x => x.InsertedFrom,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StaffDepartmentEvaluation_AspNetUsers_Update",
                        column: x => x.UpdatedFrom,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StaffDepartmentEvaluation_EvaluationType",
                        column: x => x.EvaluationTypeID,
                        principalTable: "EvaluationType",
                        principalColumn: "EvaluationTypeID");
                    table.ForeignKey(
                        name: "FK_StaffDepartmentEvaluation_StaffCollege",
                        column: x => x.StaffDepartmentID,
                        principalTable: "StaffDepartment",
                        principalColumn: "StaffDepartmentID");
                });

            migrationBuilder.CreateTable(
                name: "StaffDepartmentSubject",
                columns: table => new
                {
                    StaffDepartmentSubjectID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StaffDepartmentID = table.Column<int>(type: "int", nullable: false),
                    SubjectID = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    InsertedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    InsertedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedNo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffDepartmentSubject", x => x.StaffDepartmentSubjectID);
                    table.ForeignKey(
                        name: "FK_StaffDepartmentSubject_AspNetUsers_Insert",
                        column: x => x.InsertedFrom,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StaffDepartmentSubject_AspNetUsers_Update",
                        column: x => x.UpdatedFrom,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StaffDepartmentSubject_StaffDepartment",
                        column: x => x.StaffDepartmentID,
                        principalTable: "StaffDepartment",
                        principalColumn: "StaffDepartmentID");
                    table.ForeignKey(
                        name: "FK_StaffDepartmentSubject_Subject",
                        column: x => x.SubjectID,
                        principalTable: "Subject",
                        principalColumn: "SubjectID");
                });

            migrationBuilder.CreateTable(
                name: "HolidayRequestStatus",
                columns: table => new
                {
                    HolidayRequestStatusID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HolidayRequestID = table.Column<int>(type: "int", nullable: false),
                    StatusTypeID = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    InsertedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    InsertedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedNo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HolidayRequestStatus", x => x.HolidayRequestStatusID);
                    table.ForeignKey(
                        name: "FK_HolidayRequestStatus_AspNetUsers_Insert",
                        column: x => x.InsertedFrom,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HolidayRequestStatus_AspNetUsers_Update",
                        column: x => x.UpdatedFrom,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HolidayRequestStatus_HolidayRequest",
                        column: x => x.HolidayRequestID,
                        principalTable: "HolidayRequest",
                        principalColumn: "HolidayRequestID");
                    table.ForeignKey(
                        name: "FK_HolidayRequestStatus_StatusType",
                        column: x => x.StatusTypeID,
                        principalTable: "StatusType",
                        principalColumn: "StatusTypeID");
                });

            migrationBuilder.CreateTable(
                name: "StaffDepartmentEvaluationQuestionnaire",
                columns: table => new
                {
                    StaffDepartmentEvaluationQuestionnaireID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StaffDepartmentEvaluationID = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    RateTypeID = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    InsertedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    InsertedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedNo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffDepartmentEvaluationQuestionnaire", x => x.StaffDepartmentEvaluationQuestionnaireID);
                    table.ForeignKey(
                        name: "FK_StaffDepartmentEvaluationQuestionnaire_AspNetUsers_InsertedFrom",
                        column: x => x.InsertedFrom,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StaffDepartmentEvaluationQuestionnaire_AspNetUsers_UpdatedFrom",
                        column: x => x.UpdatedFrom,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StaffDepartmentEvaluationQuestionnaire_RateType",
                        column: x => x.RateTypeID,
                        principalTable: "RateType",
                        principalColumn: "RateTypeID");
                    table.ForeignKey(
                        name: "FK_StaffDepartmentEvaluationQuestionnaire_StaffDepartmentEvaluation",
                        column: x => x.StaffDepartmentEvaluationID,
                        principalTable: "StaffDepartmentEvaluation",
                        principalColumn: "StaffDepartmentEvaluationID");
                });

            migrationBuilder.CreateTable(
                name: "StaffDepartmentEvaluationQuestionnaireRate",
                columns: table => new
                {
                    StaffDepartmentEvaluationQuestionnaireRateID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StaffDepartmentEvaluationQuestionnaireID = table.Column<int>(type: "int", nullable: false),
                    RateTypeID = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    InsertedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    InsertedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedFrom = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedNo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffDepartmentEvaluationQuestionnaireRate", x => x.StaffDepartmentEvaluationQuestionnaireRateID);
                    table.ForeignKey(
                        name: "FK_StaffDepartmentEvaluationQuestionnaireRate_RateType",
                        column: x => x.RateTypeID,
                        principalTable: "RateType",
                        principalColumn: "RateTypeID");
                    table.ForeignKey(
                        name: "FK_StaffDepartmentEvaluationQuestionnaireRate_StaffDepartmentEvaluationQuestionnaire",
                        column: x => x.StaffDepartmentEvaluationQuestionnaireID,
                        principalTable: "StaffDepartmentEvaluationQuestionnaire",
                        principalColumn: "StaffDepartmentEvaluationQuestionnaireID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "([NormalizedName] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "([NormalizedUserName] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_Department_InsertedFrom",
                table: "Department",
                column: "InsertedFrom");

            migrationBuilder.CreateIndex(
                name: "IX_Department_UpdatedFrom",
                table: "Department",
                column: "UpdatedFrom");

            migrationBuilder.CreateIndex(
                name: "IX_Document_DocumentTypeID",
                table: "Document",
                column: "DocumentTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Document_InsertedFrom",
                table: "Document",
                column: "InsertedFrom");

            migrationBuilder.CreateIndex(
                name: "IX_Document_UpdatedFrom",
                table: "Document",
                column: "UpdatedFrom");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentType_InsertedFrom",
                table: "DocumentType",
                column: "InsertedFrom");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentType_UpdatedFrom",
                table: "DocumentType",
                column: "UpdatedFrom");

            migrationBuilder.CreateIndex(
                name: "IX_EducationLevelType_InsertedFrom",
                table: "EducationLevelType",
                column: "InsertedFrom");

            migrationBuilder.CreateIndex(
                name: "IX_EducationLevelType_UpdatedFrom",
                table: "EducationLevelType",
                column: "UpdatedFrom");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationType_InsertedFrom",
                table: "EvaluationType",
                column: "InsertedFrom");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationType_UpdatedFrom",
                table: "EvaluationType",
                column: "UpdatedFrom");

            migrationBuilder.CreateIndex(
                name: "IX_HolidayRequest_HolidayTypeID",
                table: "HolidayRequest",
                column: "HolidayTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_HolidayRequest_InsertedFrom",
                table: "HolidayRequest",
                column: "InsertedFrom");

            migrationBuilder.CreateIndex(
                name: "IX_HolidayRequest_StaffDepartmentID",
                table: "HolidayRequest",
                column: "StaffDepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_HolidayRequest_UpdatedFrom",
                table: "HolidayRequest",
                column: "UpdatedFrom");

            migrationBuilder.CreateIndex(
                name: "IX_HolidayRequestStatus_HolidayRequestID",
                table: "HolidayRequestStatus",
                column: "HolidayRequestID");

            migrationBuilder.CreateIndex(
                name: "IX_HolidayRequestStatus_InsertedFrom",
                table: "HolidayRequestStatus",
                column: "InsertedFrom");

            migrationBuilder.CreateIndex(
                name: "IX_HolidayRequestStatus_StatusTypeID",
                table: "HolidayRequestStatus",
                column: "StatusTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_HolidayRequestStatus_UpdatedFrom",
                table: "HolidayRequestStatus",
                column: "UpdatedFrom");

            migrationBuilder.CreateIndex(
                name: "IX_HolidayType_InsertedFrom",
                table: "HolidayType",
                column: "InsertedFrom");

            migrationBuilder.CreateIndex(
                name: "IX_HolidayType_UpdatedFrom",
                table: "HolidayType",
                column: "UpdatedFrom");

            migrationBuilder.CreateIndex(
                name: "IX_Menu_InsertedFrom",
                schema: "Core",
                table: "Menu",
                column: "InsertedFrom");

            migrationBuilder.CreateIndex(
                name: "IX_Menu_UpdatedFrom",
                schema: "Core",
                table: "Menu",
                column: "UpdatedFrom");

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionType_InsertedFrom",
                table: "ProfessionType",
                column: "InsertedFrom");

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionType_UpdatedFrom",
                table: "ProfessionType",
                column: "UpdatedFrom");

            migrationBuilder.CreateIndex(
                name: "IX_RateType_InsertedFrom",
                table: "RateType",
                column: "InsertedFrom");

            migrationBuilder.CreateIndex(
                name: "IX_RateType_UpdatedFrom",
                table: "RateType",
                column: "UpdatedFrom");

            migrationBuilder.CreateIndex(
                name: "IX_RealRole_InsertedFrom",
                schema: "Core",
                table: "RealRole",
                column: "InsertedFrom");

            migrationBuilder.CreateIndex(
                name: "IX_RealRole_RoleID",
                schema: "Core",
                table: "RealRole",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_RealRole_UpdatedFrom",
                schema: "Core",
                table: "RealRole",
                column: "UpdatedFrom");

            migrationBuilder.CreateIndex(
                name: "IX_RealRole_UserID",
                schema: "Core",
                table: "RealRole",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Staff_UpdatedFrom",
                table: "Staff",
                column: "UpdatedFrom");

            migrationBuilder.CreateIndex(
                name: "IX_Staff_UserID",
                table: "Staff",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_StaffDepartment_DepartmentID",
                table: "StaffDepartment",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_StaffDepartment_InsertedFrom",
                table: "StaffDepartment",
                column: "InsertedFrom");

            migrationBuilder.CreateIndex(
                name: "IX_StaffDepartment_StaffID",
                table: "StaffDepartment",
                column: "StaffID");

            migrationBuilder.CreateIndex(
                name: "IX_StaffDepartment_StaffTypeID",
                table: "StaffDepartment",
                column: "StaffTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_StaffDepartment_UpdatedFrom",
                table: "StaffDepartment",
                column: "UpdatedFrom");

            migrationBuilder.CreateIndex(
                name: "IX_StaffDepartmentEvaluation_EvaluationTypeID",
                table: "StaffDepartmentEvaluation",
                column: "EvaluationTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_StaffDepartmentEvaluation_InsertedFrom",
                table: "StaffDepartmentEvaluation",
                column: "InsertedFrom");

            migrationBuilder.CreateIndex(
                name: "IX_StaffDepartmentEvaluation_StaffDepartmentID",
                table: "StaffDepartmentEvaluation",
                column: "StaffDepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_StaffDepartmentEvaluation_UpdatedFrom",
                table: "StaffDepartmentEvaluation",
                column: "UpdatedFrom");

            migrationBuilder.CreateIndex(
                name: "IX_StaffDepartmentEvaluationQuestionnaire_InsertedFrom",
                table: "StaffDepartmentEvaluationQuestionnaire",
                column: "InsertedFrom");

            migrationBuilder.CreateIndex(
                name: "IX_StaffDepartmentEvaluationQuestionnaire_RateTypeID",
                table: "StaffDepartmentEvaluationQuestionnaire",
                column: "RateTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_StaffDepartmentEvaluationQuestionnaire_StaffDepartmentEvaluationID",
                table: "StaffDepartmentEvaluationQuestionnaire",
                column: "StaffDepartmentEvaluationID");

            migrationBuilder.CreateIndex(
                name: "IX_StaffDepartmentEvaluationQuestionnaire_UpdatedFrom",
                table: "StaffDepartmentEvaluationQuestionnaire",
                column: "UpdatedFrom");

            migrationBuilder.CreateIndex(
                name: "IX_StaffDepartmentEvaluationQuestionnaireRate_RateTypeID",
                table: "StaffDepartmentEvaluationQuestionnaireRate",
                column: "RateTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_StaffDepartmentEvaluationQuestionnaireRate_StaffDepartmentEvaluationQuestionnaireID",
                table: "StaffDepartmentEvaluationQuestionnaireRate",
                column: "StaffDepartmentEvaluationQuestionnaireID");

            migrationBuilder.CreateIndex(
                name: "IX_StaffDepartmentSubject_InsertedFrom",
                table: "StaffDepartmentSubject",
                column: "InsertedFrom");

            migrationBuilder.CreateIndex(
                name: "IX_StaffDepartmentSubject_StaffDepartmentID",
                table: "StaffDepartmentSubject",
                column: "StaffDepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_StaffDepartmentSubject_SubjectID",
                table: "StaffDepartmentSubject",
                column: "SubjectID");

            migrationBuilder.CreateIndex(
                name: "IX_StaffDepartmentSubject_UpdatedFrom",
                table: "StaffDepartmentSubject",
                column: "UpdatedFrom");

            migrationBuilder.CreateIndex(
                name: "IX_StaffDocument_DocumentTypeID",
                table: "StaffDocument",
                column: "DocumentTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_StaffDocument_InsertedFrom",
                table: "StaffDocument",
                column: "InsertedFrom");

            migrationBuilder.CreateIndex(
                name: "IX_StaffDocument_StaffID",
                table: "StaffDocument",
                column: "StaffID");

            migrationBuilder.CreateIndex(
                name: "IX_StaffDocument_UpdatedFrom",
                table: "StaffDocument",
                column: "UpdatedFrom");

            migrationBuilder.CreateIndex(
                name: "IX_StaffQualification_EducationLevelTypeID",
                table: "StaffQualification",
                column: "EducationLevelTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_StaffQualification_InsertedFrom",
                table: "StaffQualification",
                column: "InsertedFrom");

            migrationBuilder.CreateIndex(
                name: "IX_StaffQualification_ProfessionTypeID",
                table: "StaffQualification",
                column: "ProfessionTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_StaffQualification_StaffID",
                table: "StaffQualification",
                column: "StaffID");

            migrationBuilder.CreateIndex(
                name: "IX_StaffQualification_UpdatedFrom",
                table: "StaffQualification",
                column: "UpdatedFrom");

            migrationBuilder.CreateIndex(
                name: "IX_StaffType_InsertedFrom",
                table: "StaffType",
                column: "InsertedFrom");

            migrationBuilder.CreateIndex(
                name: "IX_StaffType_UpdatedFrom",
                table: "StaffType",
                column: "UpdatedFrom");

            migrationBuilder.CreateIndex(
                name: "IX_StatusType_InsertedFrom",
                table: "StatusType",
                column: "InsertedFrom");

            migrationBuilder.CreateIndex(
                name: "IX_StatusType_UpdatedFrom",
                table: "StatusType",
                column: "UpdatedFrom");

            migrationBuilder.CreateIndex(
                name: "IX_Subject_InsertedFrom",
                table: "Subject",
                column: "InsertedFrom");

            migrationBuilder.CreateIndex(
                name: "IX_Subject_UpdatedFrom",
                table: "Subject",
                column: "UpdatedFrom");

            migrationBuilder.CreateIndex(
                name: "IX_SubMenu_InsertedFrom",
                schema: "Core",
                table: "SubMenu",
                column: "InsertedFrom");

            migrationBuilder.CreateIndex(
                name: "IX_SubMenu_MenuID",
                schema: "Core",
                table: "SubMenu",
                column: "MenuID");

            migrationBuilder.CreateIndex(
                name: "IX_SubMenu_UpdatedFrom",
                schema: "Core",
                table: "SubMenu",
                column: "UpdatedFrom");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppSettings",
                schema: "History");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers",
                schema: "History");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Document");

            migrationBuilder.DropTable(
                name: "Gender");

            migrationBuilder.DropTable(
                name: "HolidayRequestStatus");

            migrationBuilder.DropTable(
                name: "Log",
                schema: "Core");

            migrationBuilder.DropTable(
                name: "RealRole",
                schema: "Core");

            migrationBuilder.DropTable(
                name: "StaffDepartmentEvaluationQuestionnaireRate");

            migrationBuilder.DropTable(
                name: "StaffDepartmentSubject");

            migrationBuilder.DropTable(
                name: "StaffDocument");

            migrationBuilder.DropTable(
                name: "StaffQualification");

            migrationBuilder.DropTable(
                name: "SubMenu",
                schema: "Core");

            migrationBuilder.DropTable(
                name: "HolidayRequest");

            migrationBuilder.DropTable(
                name: "StatusType");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "StaffDepartmentEvaluationQuestionnaire");

            migrationBuilder.DropTable(
                name: "Subject");

            migrationBuilder.DropTable(
                name: "DocumentType");

            migrationBuilder.DropTable(
                name: "EducationLevelType");

            migrationBuilder.DropTable(
                name: "ProfessionType");

            migrationBuilder.DropTable(
                name: "Menu",
                schema: "Core");

            migrationBuilder.DropTable(
                name: "HolidayType");

            migrationBuilder.DropTable(
                name: "RateType");

            migrationBuilder.DropTable(
                name: "StaffDepartmentEvaluation");

            migrationBuilder.DropTable(
                name: "EvaluationType");

            migrationBuilder.DropTable(
                name: "StaffDepartment");

            migrationBuilder.DropTable(
                name: "Department");

            migrationBuilder.DropTable(
                name: "Staff");

            migrationBuilder.DropTable(
                name: "StaffType");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
