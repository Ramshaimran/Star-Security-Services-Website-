using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Star_Security_Services.Migrations
{
    /// <inheritdoc />
    public partial class updatemigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "tbl_service",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PersonnelAssigned",
                table: "tbl_service",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ServiceArea",
                table: "tbl_service",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ServiceCode",
                table: "tbl_service",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ServiceDuration",
                table: "tbl_service",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ServiceType",
                table: "tbl_service",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "tbl_service");

            migrationBuilder.DropColumn(
                name: "PersonnelAssigned",
                table: "tbl_service");

            migrationBuilder.DropColumn(
                name: "ServiceArea",
                table: "tbl_service");

            migrationBuilder.DropColumn(
                name: "ServiceCode",
                table: "tbl_service");

            migrationBuilder.DropColumn(
                name: "ServiceDuration",
                table: "tbl_service");

            migrationBuilder.DropColumn(
                name: "ServiceType",
                table: "tbl_service");
        }
    }
}
