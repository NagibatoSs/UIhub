using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UIhub.Migrations
{
    /// <inheritdoc />
    public partial class RenameCriteria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // DropForeignKey, DropPrimaryKey, RenameTable — уже сделано миграцией rename, убираем

            migrationBuilder.DropColumn(
                name: "CriteriaId",
                table: "CriteriaResults");

            migrationBuilder.RenameIndex(
                name: "IX_Analyses_UserId",
                table: "Analysis",
                newName: "IX_Analysis_UserId");
        }
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Analysis_AspNetUsers_UserId",
                table: "Analysis");

            migrationBuilder.DropForeignKey(
                name: "FK_AnalysisFiles_Analysis_AnalysisId",
                table: "AnalysisFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Analysis",
                table: "Analysis");

            migrationBuilder.RenameIndex(
                name: "IX_Analysis_UserId",
                table: "Analysis",
                newName: "IX_Analyses_UserId");

            migrationBuilder.AddColumn<int>(
                name: "CriteriaId",
                table: "CriteriaResults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Analyses",
                table: "Analysis",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Analyses_AspNetUsers_UserId",
                table: "Analysis",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AnalysisFiles_Analyses_AnalysisId",
                table: "AnalysisFiles",
                column: "AnalysisId",
                principalTable: "Analysis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
