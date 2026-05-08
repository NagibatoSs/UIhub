using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UIhub.Migrations
{
    /// <inheritdoc />
    public partial class Analysis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Analyses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Analyses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Analyses_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnalysisCriterias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Recommendation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThresholdValue = table.Column<double>(type: "float", nullable: true),
                    ThresholdDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StandardReference = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalysisCriterias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AnalysisFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnalysisId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageWidth = table.Column<int>(type: "int", nullable: false),
                    ImageHeight = table.Column<int>(type: "int", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalysisFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnalysisFiles_Analyses_AnalysisId",
                        column: x => x.AnalysisId,
                        principalTable: "Analyses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CriteriaResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnalysisFileId = table.Column<int>(type: "int", nullable: false),
                    CriteriaId = table.Column<int>(type: "int", nullable: false),
                    MetricValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsOk = table.Column<bool>(type: "bit", nullable: false),
                    AnalysisCriteriaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CriteriaResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CriteriaResults_AnalysisCriterias_AnalysisCriteriaId",
                        column: x => x.AnalysisCriteriaId,
                        principalTable: "AnalysisCriterias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CriteriaResults_AnalysisFiles_AnalysisFileId",
                        column: x => x.AnalysisFileId,
                        principalTable: "AnalysisFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CriteriaIssues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CriteriaResultId = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CriteriaIssues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CriteriaIssues_CriteriaResults_CriteriaResultId",
                        column: x => x.CriteriaResultId,
                        principalTable: "CriteriaResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Analyses_UserId",
                table: "Analyses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisFiles_AnalysisId",
                table: "AnalysisFiles",
                column: "AnalysisId");

            migrationBuilder.CreateIndex(
                name: "IX_CriteriaIssues_CriteriaResultId",
                table: "CriteriaIssues",
                column: "CriteriaResultId");

            migrationBuilder.CreateIndex(
                name: "IX_CriteriaResults_AnalysisCriteriaId",
                table: "CriteriaResults",
                column: "AnalysisCriteriaId");

            migrationBuilder.CreateIndex(
                name: "IX_CriteriaResults_AnalysisFileId",
                table: "CriteriaResults",
                column: "AnalysisFileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CriteriaIssues");

            migrationBuilder.DropTable(
                name: "CriteriaResults");

            migrationBuilder.DropTable(
                name: "AnalysisCriterias");

            migrationBuilder.DropTable(
                name: "AnalysisFiles");

            migrationBuilder.DropTable(
                name: "Analyses");
        }
    }
}
