using Microsoft.EntityFrameworkCore.Migrations;

namespace TN.Data.Migrations
{
    public partial class editexam : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "isActive" },
                values: new object[] { "04365741-4b73-418a-b3ea-f437c3ff739e", "AQAAAAEAACcQAAAAEBEJdp9913EJb1YztJSB2cdk+PIOh4GZy6+A0MN8ofW8pTf2JarSB0xxlW5dsfD5dw==", true });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "isActive" },
                values: new object[] { "d2a62252-0dff-41e5-bf30-81ae747041b1", "AQAAAAEAACcQAAAAEJvrgqoGpp6/vosh9+4di3vtg0+v5rehDaGGI0HTOptLxw3Vx03va5+EFEF7WVoLNw==", true });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
