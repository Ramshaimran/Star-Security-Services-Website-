using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Star_Security_Services.Migrations
{
    /// <inheritdoc />
    public partial class sign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "tbl_user",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "tbl_user",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Industry",
                table: "tbl_user",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "tbl_user",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "tbl_user");

            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "tbl_user");

            migrationBuilder.DropColumn(
                name: "Industry",
                table: "tbl_user");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "tbl_user");
        }
    }
}
