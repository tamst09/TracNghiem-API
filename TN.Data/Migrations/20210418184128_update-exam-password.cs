using Microsoft.EntityFrameworkCore.Migrations;

namespace TN.Data.Migrations
{
    public partial class updateexampassword : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_RefreshTokens_RefreshTokenValue",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RefreshTokens",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "Token",
                table: "RefreshTokens");

            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "RefreshTokens",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Exams",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "AspNetUsers",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RefreshTokens",
                table: "RefreshTokens",
                column: "Value");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "5ee69b00-ed44-461d-9e7b-7c26cc86e228");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "6f2cb76a-faeb-47af-83e2-854fc7713a17");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "isActive" },
                values: new object[] { "47a27ee1-c1da-456b-a4a6-918e113fcf01", "AQAAAAEAACcQAAAAEJDdvGxNP6ybNn618ge/yI7aRVqI6JdOGB4w4th210zt1ykwg4YDgr5XAeFWbHQaBA==", true });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "isActive" },
                values: new object[] { "97b3d027-4a70-4d21-97eb-6b2a563ac042", "AQAAAAEAACcQAAAAELOGkUYPuTKPTcEsx2J6E/WEccvPcJ27I+UejgksXb3V2w8/prDzYeBQZcwFSc9q3g==", true });

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_RefreshTokens_RefreshTokenValue",
                table: "AspNetUsers",
                column: "RefreshTokenValue",
                principalTable: "RefreshTokens",
                principalColumn: "Value",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_RefreshTokens_RefreshTokenValue",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RefreshTokens",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Exams");

            migrationBuilder.AddColumn<string>(
                name: "Token",
                table: "RefreshTokens",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "AspNetUsers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RefreshTokens",
                table: "RefreshTokens",
                column: "Token");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "4d3f2ab3-6257-4103-b9de-66b66825f6e5");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "2bad986d-62e5-4030-b374-3db1bb47f4b9");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "isActive" },
                values: new object[] { "fbebe5ed-7830-4be2-a57a-11af97ad6bd6", "AQAAAAEAACcQAAAAEEED9dTINbJ5JtvJSvxTx2bcxS6hip2Bb6p1WSiacbdw5AxXbf28K0Q+3j77a2Wynw==", true });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "isActive" },
                values: new object[] { "e0896ce9-ea73-4632-8676-6a422f2b4191", "AQAAAAEAACcQAAAAEDSlCFsH/PWF0ZmtCNFmU0LuxOa+MrV5rNXcypItFNGJQhgdY3tWm17FVjUI9OCXqw==", true });

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_RefreshTokens_RefreshTokenValue",
                table: "AspNetUsers",
                column: "RefreshTokenValue",
                principalTable: "RefreshTokens",
                principalColumn: "Token",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
