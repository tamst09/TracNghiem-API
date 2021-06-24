using Microsoft.EntityFrameworkCore.Migrations;

namespace TN.Data.Migrations
{
    public partial class editBoolProps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "isActive",
                table: "AspNetUsers",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "f7ba1362-a300-4cd1-a6f3-5538c3bae90d");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "0309def8-e334-459f-8875-05e19e28eda5");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "331c4a19-58a9-4759-9d50-1975f8f8018c", "AQAAAAEAACcQAAAAEDsJG6bpyFnzpdT16kmHgB5lX495deuvxsOvvcO6WiPc1NSW7t0LeGolitGu/wf8Sg==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "499c039a-311a-4afc-a7c4-1ae8f4bf07a4", "AQAAAAEAACcQAAAAEMyN+OEFd+BG6Cjx8jx959Wr6rR+cMEXUSE5Awtl+feaC/uXTWipo5WpRTEmbrNfOA==" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "isActive",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool));

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "b3aaa066-2531-4712-bf6b-1a2ae6ae6c0f");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "b679ec51-30b4-4596-a05f-64688634f130");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "isActive" },
                values: new object[] { "1cfd7285-b83d-4929-be1b-9f9acc94ecde", "AQAAAAEAACcQAAAAEJ73wozV8uRssalYXV853DUcGc2rRV+AVavPX9cF6M/8G9SxKpO7OK+m+2zwFEKBEQ==", true });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "isActive" },
                values: new object[] { "6b78d25e-a9d4-47e5-b451-80dbac54f56b", "AQAAAAEAACcQAAAAEFmZb1KCP6S8tE/sIK5msY8sxKGkeQnJ5XCJZ9SBqDlVW0QFYyRI2Dvb+OquTdjiAQ==", true });
        }
    }
}
