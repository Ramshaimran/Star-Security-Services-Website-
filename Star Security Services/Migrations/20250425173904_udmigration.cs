using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Star_Security_Services.Migrations
{
    /// <inheritdoc />
    public partial class udmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmployeeCode",
                table: "tbl_vacancyapplication");

            migrationBuilder.RenameColumn(
                name: "EmployeeName",
                table: "tbl_vacancyapplication",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "EmployeeEmail",
                table: "tbl_vacancyapplication",
                newName: "UserEmail");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "tbl_vacancyapplication",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "tbl_vacancyapplication");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "tbl_vacancyapplication",
                newName: "EmployeeName");

            migrationBuilder.RenameColumn(
                name: "UserEmail",
                table: "tbl_vacancyapplication",
                newName: "EmployeeEmail");

            migrationBuilder.AddColumn<string>(
                name: "EmployeeCode",
                table: "tbl_vacancyapplication",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
