using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UIhub.Migrations
{
    /// <inheritdoc />
    public partial class adduserPostEstimate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserEstimates");

            migrationBuilder.CreateTable(
                name: "UserPostEstimates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PostId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPostEstimates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPostEstimates_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserPostEstimates_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserPostEstimates_PostId",
                table: "UserPostEstimates",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPostEstimates_UserId",
                table: "UserPostEstimates",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPostEstimates");

            migrationBuilder.CreateTable(
                name: "UserEstimates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EstimateId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEstimates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserEstimates_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserEstimates_Estimates_EstimateId",
                        column: x => x.EstimateId,
                        principalTable: "Estimates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserEstimates_EstimateId",
                table: "UserEstimates",
                column: "EstimateId");

            migrationBuilder.CreateIndex(
                name: "IX_UserEstimates_UserId",
                table: "UserEstimates",
                column: "UserId");
        }
    }
}
