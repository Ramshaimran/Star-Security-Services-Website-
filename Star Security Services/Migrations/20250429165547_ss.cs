using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Star_Security_Services.Migrations
{
    /// <inheritdoc />
    public partial class ss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Username",
                table: "tbl_contact");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "tbl_contact",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
