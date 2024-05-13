using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UIhub.Migrations
{
    /// <inheritdoc />
    public partial class addseq : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumbersOrder",
                table: "Estimates");

            migrationBuilder.CreateTable(
                name: "RangingSequences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumbersOrder = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstimateRangingId = table.Column<int>(type: "int", nullable: true)
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RangingSequences");

            migrationBuilder.AddColumn<string>(
                name: "NumbersOrder",
                table: "Estimates",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
