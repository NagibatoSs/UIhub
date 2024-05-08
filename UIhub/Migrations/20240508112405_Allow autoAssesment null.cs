using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UIhub.Migrations
{
    /// <inheritdoc />
    public partial class AllowautoAssesmentnull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_AutoAssesmentResults_AutoAssessmentId",
                table: "Posts");

            migrationBuilder.AlterColumn<int>(
                name: "AutoAssessmentId",
                table: "Posts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_AutoAssesmentResults_AutoAssessmentId",
                table: "Posts",
                column: "AutoAssessmentId",
                principalTable: "AutoAssesmentResults",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_AutoAssesmentResults_AutoAssessmentId",
                table: "Posts");

            migrationBuilder.AlterColumn<int>(
                name: "AutoAssessmentId",
                table: "Posts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_AutoAssesmentResults_AutoAssessmentId",
                table: "Posts",
                column: "AutoAssessmentId",
                principalTable: "AutoAssesmentResults",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
