using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Star_Security_Services.Migrations
{
    /// <inheritdoc />
    public partial class eupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_a_employee_tbl_assign_AssignA_id",
                table: "tbl_a_employee");

            migrationBuilder.DropIndex(
                name: "IX_tbl_a_employee_AssignA_id",
                table: "tbl_a_employee");

            migrationBuilder.DropColumn(
                name: "AssignA_id",
                table: "tbl_a_employee");

            migrationBuilder.AddColumn<string>(
                name: "EmployeeDepartment",
                table: "tbl_assign",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EmployeeEmail",
                table: "tbl_assign",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EmployeeName",
                table: "tbl_assign",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmployeeDepartment",
                table: "tbl_assign");

            migrationBuilder.DropColumn(
                name: "EmployeeEmail",
                table: "tbl_assign");

            migrationBuilder.DropColumn(
                name: "EmployeeName",
                table: "tbl_assign");

            migrationBuilder.AddColumn<int>(
                name: "AssignA_id",
                table: "tbl_a_employee",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_a_employee_AssignA_id",
                table: "tbl_a_employee",
                column: "AssignA_id");

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_a_employee_tbl_assign_AssignA_id",
                table: "tbl_a_employee",
                column: "AssignA_id",
                principalTable: "tbl_assign",
                principalColumn: "A_id");
        }
    }
}
