using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UIhub.Migrations
{
    /// <inheritdoc />
    public partial class addpostReplyLike : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PostReplyLikes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PostReplyId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostReplyLikes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostReplyLikes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PostReplyLikes_PostTextReplies_PostReplyId",
                        column: x => x.PostReplyId,
                        principalTable: "PostTextReplies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostReplyLikes_PostReplyId",
                table: "PostReplyLikes",
                column: "PostReplyId");

            migrationBuilder.CreateIndex(
                name: "IX_PostReplyLikes_UserId",
                table: "PostReplyLikes",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostReplyLikes");
        }
    }
}
