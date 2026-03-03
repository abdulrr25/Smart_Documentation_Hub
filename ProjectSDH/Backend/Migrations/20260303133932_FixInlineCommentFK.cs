using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class FixInlineCommentFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InlineComments_DocumentVersions_DocumentVersionVersionId",
                table: "InlineComments");

            migrationBuilder.DropIndex(
                name: "IX_InlineComments_DocumentVersionVersionId",
                table: "InlineComments");

            migrationBuilder.DropColumn(
                name: "DocumentVersionVersionId",
                table: "InlineComments");

            migrationBuilder.CreateIndex(
                name: "IX_InlineComments_VersionId",
                table: "InlineComments",
                column: "VersionId");

            migrationBuilder.AddForeignKey(
                name: "FK_InlineComments_DocumentVersions_VersionId",
                table: "InlineComments",
                column: "VersionId",
                principalTable: "DocumentVersions",
                principalColumn: "VersionId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InlineComments_DocumentVersions_VersionId",
                table: "InlineComments");

            migrationBuilder.DropIndex(
                name: "IX_InlineComments_VersionId",
                table: "InlineComments");

            migrationBuilder.AddColumn<int>(
                name: "DocumentVersionVersionId",
                table: "InlineComments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_InlineComments_DocumentVersionVersionId",
                table: "InlineComments",
                column: "DocumentVersionVersionId");

            migrationBuilder.AddForeignKey(
                name: "FK_InlineComments_DocumentVersions_DocumentVersionVersionId",
                table: "InlineComments",
                column: "DocumentVersionVersionId",
                principalTable: "DocumentVersions",
                principalColumn: "VersionId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
