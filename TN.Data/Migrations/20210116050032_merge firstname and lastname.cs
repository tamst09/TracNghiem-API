using Microsoft.EntityFrameworkCore.Migrations;

namespace TN.Data.Migrations
{
    public partial class mergefirstnameandlastname : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AspNetUsers",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

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
                columns: new[] { "ConcurrencyStamp", "Name", "PasswordHash", "isActive" },
                values: new object[] { "fbebe5ed-7830-4be2-a57a-11af97ad6bd6", "Primary Admin", "AQAAAAEAACcQAAAAEEED9dTINbJ5JtvJSvxTx2bcxS6hip2Bb6p1WSiacbdw5AxXbf28K0Q+3j77a2Wynw==", true });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "Name", "PasswordHash", "isActive" },
                values: new object[] { "e0896ce9-ea73-4632-8676-6a422f2b4191", "User Default", "AQAAAAEAACcQAAAAEDSlCFsH/PWF0ZmtCNFmU0LuxOa+MrV5rNXcypItFNGJQhgdY3tWm17FVjUI9OCXqw==", true });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "d732fbb6-f2b1-4d5a-8fe1-65c9e347596d");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "c548bc3e-6fc7-4e23-963c-1183682ca026");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "FirstName", "LastName", "PasswordHash", "isActive" },
                values: new object[] { "04365741-4b73-418a-b3ea-f437c3ff739e", "Admin", "Default", "AQAAAAEAACcQAAAAEBEJdp9913EJb1YztJSB2cdk+PIOh4GZy6+A0MN8ofW8pTf2JarSB0xxlW5dsfD5dw==", true });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "FirstName", "LastName", "PasswordHash", "isActive" },
                values: new object[] { "d2a62252-0dff-41e5-bf30-81ae747041b1", "User", "Default", "AQAAAAEAACcQAAAAEJvrgqoGpp6/vosh9+4di3vtg0+v5rehDaGGI0HTOptLxw3Vx03va5+EFEF7WVoLNw==", true });
        }
    }
}
