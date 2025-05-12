using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Star_Security_Services.Migrations
{
    /// <inheritdoc />
    public partial class ld : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Username",
                table: "tbl_user",
                newName: "username");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "username",
                table: "tbl_user",
                newName: "Username");
        }
    }
}
