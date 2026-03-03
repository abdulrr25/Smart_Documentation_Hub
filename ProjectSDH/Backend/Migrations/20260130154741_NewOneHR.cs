using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class NewOneHR : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentVersions_Documents_DocumentDocId",
                table: "DocumentVersions");

            migrationBuilder.DropIndex(
                name: "IX_DocumentVersions_DocumentDocId",
                table: "DocumentVersions");

            migrationBuilder.DropColumn(
                name: "DocumentDocId",
                table: "DocumentVersions");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentVersions_DocId",
                table: "DocumentVersions",
                column: "DocId");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentVersions_Documents_DocId",
                table: "DocumentVersions",
                column: "DocId",
                principalTable: "Documents",
                principalColumn: "DocId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentVersions_Documents_DocId",
                table: "DocumentVersions");

            migrationBuilder.DropIndex(
                name: "IX_DocumentVersions_DocId",
                table: "DocumentVersions");

            migrationBuilder.AddColumn<int>(
                name: "DocumentDocId",
                table: "DocumentVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentVersions_DocumentDocId",
                table: "DocumentVersions",
                column: "DocumentDocId");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentVersions_Documents_DocumentDocId",
                table: "DocumentVersions",
                column: "DocumentDocId",
                principalTable: "Documents",
                principalColumn: "DocId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
