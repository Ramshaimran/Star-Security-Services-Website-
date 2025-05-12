using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Star_Security_Services.Migrations
{
    /// <inheritdoc />
    public partial class assign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssignA_id",
                table: "tbl_a_employee",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "tbl_assign",
                columns: table => new
                {
                    A_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServiceName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServiceCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServiceType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServiceArea = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Personnel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServiceDuration = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BookingId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_assign", x => x.A_id);
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_a_employee_tbl_assign_AssignA_id",
                table: "tbl_a_employee");

            migrationBuilder.DropTable(
                name: "tbl_assign");

            migrationBuilder.DropIndex(
                name: "IX_tbl_a_employee_AssignA_id",
                table: "tbl_a_employee");

            migrationBuilder.DropColumn(
                name: "AssignA_id",
                table: "tbl_a_employee");
        }
    }
}
