using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartDocumentHub.Migrations
{
    /// <inheritdoc />
    public partial class AddOriginalTextToDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OriginalText",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginalText",
                table: "Documents");
        }
    }
}
