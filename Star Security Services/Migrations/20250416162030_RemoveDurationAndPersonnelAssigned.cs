using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Star_Security_Services.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDurationAndPersonnelAssigned : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PersonnelAssigned",
                table: "tbl_service");

            migrationBuilder.DropColumn(
                name: "ServiceDuration",
                table: "tbl_service");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PersonnelAssigned",
                table: "tbl_service",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ServiceDuration",
                table: "tbl_service",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
