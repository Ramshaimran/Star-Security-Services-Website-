using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Star_Security_Services.Migrations
{
    /// <inheritdoc />
    public partial class qrimagemigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QRImage",
                table: "tbl_qrcode");

            migrationBuilder.AddColumn<string>(
                name: "QRImagePath",
                table: "tbl_qrcode",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QRImagePath",
                table: "tbl_qrcode");

            migrationBuilder.AddColumn<byte[]>(
                name: "QRImage",
                table: "tbl_qrcode",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}
