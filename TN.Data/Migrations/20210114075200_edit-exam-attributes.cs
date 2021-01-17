using Microsoft.EntityFrameworkCore.Migrations;

namespace TN.Data.Migrations
{
    public partial class editexamattributes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Time",
                table: "Exams",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CategoryID",
                table: "Exams",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "f47a17c7-9734-4f94-a76f-799e3b3e0f7b");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "6c9fc70c-88be-470d-acde-34d89f7acda2");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "isActive" },
                values: new object[] { "4804e442-77ed-4377-b0bb-8efad604c40b", "AQAAAAEAACcQAAAAEOUZE6+vi7fOWYd8nVBNfKnTRGW+s6p7cr5acecoPGMP1/rgsqReRW2n/2LYoMqHrA==", true });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "isActive" },
                values: new object[] { "56eecc27-1312-4753-8617-ff02b557213f", "AQAAAAEAACcQAAAAELtVZ1kKRnb2t4IHkfTHYk9fSSLhPlB2xAT9v87tmAeQ9VgLZeZs+qHZCVDcTPxoFw==", true });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Time",
                table: "Exams",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "CategoryID",
                table: "Exams",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "15db273c-8d6f-409b-9d3f-45482c14958e");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "146e1f78-88ed-4e05-9cbd-2decc8f5c2f9");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "isActive" },
                values: new object[] { "6f6a0a43-7e29-4e0d-b00d-e705a667fbcf", "AQAAAAEAACcQAAAAEP5orOhGpPG3DmkeK/aCuYf+4dguYgc+BSXVMWPmqbf2oh3ev/98NxMVNHMMXuRjvg==", true });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "isActive" },
                values: new object[] { "8ae0dfc1-aa3b-4255-a8c4-5c1b87ccb088", "AQAAAAEAACcQAAAAED9tRurcyflHfJeFtTxQsv2kC1tIkPG3/j/UOMK/Q6qZ4s+962m/QTzHkCfcjOupKQ==", true });
        }
    }
}
