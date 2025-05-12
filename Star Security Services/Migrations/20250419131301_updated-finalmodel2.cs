using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Star_Security_Services.Migrations
{
    /// <inheritdoc />
    public partial class updatedfinalmodel2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_final_tbl_service_ServiceID",
                table: "tbl_final");

            migrationBuilder.DropIndex(
                name: "IX_tbl_final_ServiceID",
                table: "tbl_final");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_tbl_final_ServiceID",
                table: "tbl_final",
                column: "ServiceID");

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_final_tbl_service_ServiceID",
                table: "tbl_final",
                column: "ServiceID",
                principalTable: "tbl_service",
                principalColumn: "ServiceID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
