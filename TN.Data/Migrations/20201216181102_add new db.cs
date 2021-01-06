using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TN.Data.Migrations
{
    public partial class addnewdb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(maxLength: 100, nullable: false),
                    isAcive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Mailboxes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SenderID = table.Column<int>(nullable: false),
                    ReceiverID = table.Column<int>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    Time = table.Column<DateTime>(nullable: false),
                    isRead = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mailboxes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Token = table.Column<string>(nullable: false),
                    ExpiryDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Token);
                });

            migrationBuilder.CreateTable(
                name: "AppNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(maxLength: 50, nullable: false),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 50, nullable: false),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(maxLength: 50, nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    FirstName = table.Column<string>(maxLength: 50, nullable: false),
                    LastName = table.Column<string>(maxLength: 50, nullable: false),
                    Avatar = table.Column<string>(nullable: true),
                    DoB = table.Column<DateTime>(nullable: false),
                    isActive = table.Column<bool>(nullable: false),
                    RefreshTokenValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_RefreshTokens_RefreshTokenValue",
                        column: x => x.RefreshTokenValue,
                        principalTable: "RefreshTokens",
                        principalColumn: "Token",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AppNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AppNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    RoleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AppNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AppNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Exams",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamName = table.Column<string>(maxLength: 50, nullable: false),
                    isPrivate = table.Column<bool>(nullable: false),
                    Time = table.Column<int>(nullable: true),
                    ImageURL = table.Column<string>(nullable: true),
                    TimeCreated = table.Column<DateTime>(nullable: false),
                    NumOfAttemps = table.Column<int>(nullable: false, defaultValue: 0),
                    CategoryID = table.Column<int>(nullable: true),
                    OwnerID = table.Column<int>(nullable: false),
                    isActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exams", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Exams_Categories_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "Categories",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Exams_AspNetUsers_OwnerID",
                        column: x => x.OwnerID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FavoriteExams",
                columns: table => new
                {
                    UserID = table.Column<int>(nullable: false),
                    ExamID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteExams", x => new { x.UserID, x.ExamID });
                    table.ForeignKey(
                        name: "FK_FavoriteExams_Exams_ExamID",
                        column: x => x.ExamID,
                        principalTable: "Exams",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FavoriteExams_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HistoryExams",
                columns: table => new
                {
                    UserID = table.Column<int>(nullable: false),
                    ExamID = table.Column<int>(nullable: false),
                    isFinish = table.Column<bool>(nullable: false),
                    TimeRemain = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoryExams", x => new { x.UserID, x.ExamID });
                    table.ForeignKey(
                        name: "FK_HistoryExams_Exams_ExamID",
                        column: x => x.ExamID,
                        principalTable: "Exams",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HistoryExams_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuesContent = table.Column<string>(nullable: false),
                    Option1 = table.Column<string>(nullable: false),
                    Option2 = table.Column<string>(nullable: false),
                    Option3 = table.Column<string>(nullable: true),
                    Option4 = table.Column<string>(nullable: true),
                    ImgURL = table.Column<string>(nullable: true),
                    Answer = table.Column<string>(nullable: false),
                    STT = table.Column<int>(nullable: false),
                    ExamID = table.Column<int>(nullable: false),
                    isActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Questions_Exams_ExamID",
                        column: x => x.ExamID,
                        principalTable: "Exams",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Results",
                columns: table => new
                {
                    UserID = table.Column<int>(nullable: false),
                    QuestionID = table.Column<int>(nullable: false),
                    OptChoose = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Results", x => new { x.UserID, x.QuestionID });
                    table.ForeignKey(
                        name: "FK_Results_Questions_QuestionID",
                        column: x => x.QuestionID,
                        principalTable: "Questions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Results_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Description", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { 1, "e60a6645-db41-4539-961e-ece999d16b89", "Administrator level", "admin", "ADMIN" },
                    { 2, "42193a8c-be9f-4b57-8adc-419eb0cd304f", "User level", "user", "USER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Avatar", "ConcurrencyStamp", "DoB", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "RefreshTokenValue", "SecurityStamp", "TwoFactorEnabled", "UserName", "isActive" },
                values: new object[,]
                {
                    { 1, 0, null, "e1259831-5511-44d4-bcc2-29b96d1fbae9", new DateTime(1999, 4, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "tamst09@gmail.com", true, "Admin", "Default", false, null, "TAMST09@GMAIL.COM", "admin1999", "AQAAAAEAACcQAAAAEIDtBuMGbnyWLLHbK8bPRmVf2KEtZY4H/WJ5stnDUDU7M0ch+agcvFafYcUOwTEJkA==", "0336709707", false, null, "", false, "admin1999", true },
                    { 2, 0, null, "0cc7901c-6654-4a0f-bf69-5a964391497b", new DateTime(1999, 4, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "thtt260499@gmail.com", true, "User", "Default", false, null, "THTT260499@GMAIL.COM", "USER1999", "AQAAAAEAACcQAAAAECMeG5hyoNNk/zmhsz8YTYtDkK5vrLYQ+mVbsx0RGx1vH4sAh52fa6+FryrFubCXnA==", "0336709707", false, null, "", false, "user1999", true }
                });

            migrationBuilder.InsertData(
                table: "AppNetUserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[] { 1, 1 });

            migrationBuilder.InsertData(
                table: "AppNetUserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[] { 2, 2 });

            migrationBuilder.CreateIndex(
                name: "IX_AppNetRoleClaims_RoleId",
                table: "AppNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AppNetUserClaims_UserId",
                table: "AppNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AppNetUserLogins_UserId",
                table: "AppNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AppNetUserRoles_RoleId",
                table: "AppNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_RefreshTokenValue",
                table: "AspNetUsers",
                column: "RefreshTokenValue",
                unique: true,
                filter: "[RefreshTokenValue] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Exams_CategoryID",
                table: "Exams",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Exams_OwnerID",
                table: "Exams",
                column: "OwnerID");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteExams_ExamID",
                table: "FavoriteExams",
                column: "ExamID");

            migrationBuilder.CreateIndex(
                name: "IX_HistoryExams_ExamID",
                table: "HistoryExams",
                column: "ExamID");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_ExamID",
                table: "Questions",
                column: "ExamID");

            migrationBuilder.CreateIndex(
                name: "IX_Results_QuestionID",
                table: "Results",
                column: "QuestionID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AppNetUserClaims");

            migrationBuilder.DropTable(
                name: "AppNetUserLogins");

            migrationBuilder.DropTable(
                name: "AppNetUserRoles");

            migrationBuilder.DropTable(
                name: "AppNetUserTokens");

            migrationBuilder.DropTable(
                name: "FavoriteExams");

            migrationBuilder.DropTable(
                name: "HistoryExams");

            migrationBuilder.DropTable(
                name: "Mailboxes");

            migrationBuilder.DropTable(
                name: "Results");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Exams");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "RefreshTokens");
        }
    }
}
