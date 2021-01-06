using Microsoft.EntityFrameworkCore.Migrations;

namespace TN.Data.Migrations
{
    public partial class edit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isAcive",
                table: "Categories");

            migrationBuilder.AlterColumn<bool>(
                name: "isActive",
                table: "Questions",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "isActive",
                table: "Exams",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<bool>(
                name: "isActive",
                table: "Categories",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AlterColumn<bool>(
                name: "isActive",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isActive",
                table: "Categories");

            migrationBuilder.AlterColumn<bool>(
                name: "isActive",
                table: "Questions",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<bool>(
                name: "isActive",
                table: "Exams",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldDefaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "isAcive",
                table: "Categories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "isActive",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldDefaultValue: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "e60a6645-db41-4539-961e-ece999d16b89");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "42193a8c-be9f-4b57-8adc-419eb0cd304f");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "e1259831-5511-44d4-bcc2-29b96d1fbae9", "AQAAAAEAACcQAAAAEIDtBuMGbnyWLLHbK8bPRmVf2KEtZY4H/WJ5stnDUDU7M0ch+agcvFafYcUOwTEJkA==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "0cc7901c-6654-4a0f-bf69-5a964391497b", "AQAAAAEAACcQAAAAECMeG5hyoNNk/zmhsz8YTYtDkK5vrLYQ+mVbsx0RGx1vH4sAh52fa6+FryrFubCXnA==" });
        }
    }
}
