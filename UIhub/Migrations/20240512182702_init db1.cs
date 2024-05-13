using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UIhub.Migrations
{
    /// <inheritdoc />
    public partial class initdb1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RangingObjects_RangingSequences_RangingSequenceId",
                table: "RangingObjects");

            migrationBuilder.DropTable(
                name: "RangingSequences");

            migrationBuilder.RenameColumn(
                name: "RangingSequenceId",
                table: "RangingObjects",
                newName: "EstimateRangingId");

            migrationBuilder.RenameIndex(
                name: "IX_RangingObjects_RangingSequenceId",
                table: "RangingObjects",
                newName: "IX_RangingObjects_EstimateRangingId");

            migrationBuilder.AddColumn<string>(
                name: "NumbersOrder",
                table: "Estimates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RangingObjects_Estimates_EstimateRangingId",
                table: "RangingObjects",
                column: "EstimateRangingId",
                principalTable: "Estimates",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RangingObjects_Estimates_EstimateRangingId",
                table: "RangingObjects");

            migrationBuilder.DropColumn(
                name: "NumbersOrder",
                table: "Estimates");

            migrationBuilder.RenameColumn(
                name: "EstimateRangingId",
                table: "RangingObjects",
                newName: "RangingSequenceId");

            migrationBuilder.RenameIndex(
                name: "IX_RangingObjects_EstimateRangingId",
                table: "RangingObjects",
                newName: "IX_RangingObjects_RangingSequenceId");

            migrationBuilder.CreateTable(
                name: "RangingSequences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EstimateRangingId = table.Column<int>(type: "int", nullable: true),
                    NumbersOrder = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RangingSequences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RangingSequences_Estimates_EstimateRangingId",
                        column: x => x.EstimateRangingId,
                        principalTable: "Estimates",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RangingSequences_EstimateRangingId",
                table: "RangingSequences",
                column: "EstimateRangingId");

            migrationBuilder.AddForeignKey(
                name: "FK_RangingObjects_RangingSequences_RangingSequenceId",
                table: "RangingObjects",
                column: "RangingSequenceId",
                principalTable: "RangingSequences",
                principalColumn: "Id");
        }
    }
}
