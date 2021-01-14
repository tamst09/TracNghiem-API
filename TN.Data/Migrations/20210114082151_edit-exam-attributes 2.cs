using Microsoft.EntityFrameworkCore.Migrations;

namespace TN.Data.Migrations
{
    public partial class editexamattributes2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "b2500b8f-9c71-4024-b71c-565ce14cb2fa");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "b16a471e-0381-48af-a66b-8e0f7decac0f");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "isActive" },
                values: new object[] { "b351f301-b836-40b9-9bed-960511d3a63b", "AQAAAAEAACcQAAAAEBKY5c6LkqyEmkG6s2Prm3OEcV0C/DvN1pHYS8/2YLAkzwGIgT10nbNnGYstoV7beg==", true });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "isActive" },
                values: new object[] { "49f75baa-c9e2-447a-a6c4-9716238b022f", "AQAAAAEAACcQAAAAEL3JJ1cG1RZ81KCGXdZLomDXEwZ7itAHtIuAVuZW7nqbe9KXIgS+kd1tGweRrSMSsw==", true });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
