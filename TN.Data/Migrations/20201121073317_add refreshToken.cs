using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TN.Data.Migrations
{
    public partial class addrefreshToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    TokenId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: false),
                    Token = table.Column<string>(nullable: true),
                    ExpiryDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.TokenId);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "a3e698f5-25dc-4cbe-be06-e93cc3973d8b");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "4710d550-e4bd-4450-828c-e264258bf65a");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "6e5e993d-0e0f-4501-b878-775c47113e47", "AQAAAAEAACcQAAAAEHmWncpGI9Z/0bhuAIB3EwxsausFB7453WUm9pKXy0f09qR52FHy9yedmM3BqYtKlg==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "23876861-63ff-49de-9128-30b74ccd01b9", "AQAAAAEAACcQAAAAEJrT0uNvMLmcDuFIDRIPkwQdH/ekoWnLW0AgmTqPHKC2TwgZck49/9SeU+jKXgY7Zw==" });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "052b67c0-2745-4484-86c9-632e5eaf7981");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "df5b371d-0746-4e7e-a4d8-42c6573cd0e1");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "bdae34d1-4f94-4052-8222-db178a6c7373", "AQAAAAEAACcQAAAAECOntNlfNfLeVhtjk7Bx2Nf59WtvTu1aCdK59vo/H5aRl3gqQAi3iaHPdI5V41i5Aw==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "6d5947b8-1826-4e5b-809f-15f35c850d19", "AQAAAAEAACcQAAAAEGSeOogQsYrjfgIkuH8M+zrWqEi+qmNMQIXMGFBKSWwLvGX0CP6FQ6tHBarg9LpzkQ==" });
        }
    }
}
