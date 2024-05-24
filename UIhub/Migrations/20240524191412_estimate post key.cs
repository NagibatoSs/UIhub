using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UIhub.Migrations
{
    /// <inheritdoc />
    public partial class estimatepostkey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Estimates_Posts_PostId",
                table: "Estimates");

            migrationBuilder.DropTable(
                name: "EstimateObjects");

            migrationBuilder.AlterColumn<int>(
                name: "PostId",
                table: "Estimates",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Count_1",
                table: "Estimates",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Count_2",
                table: "Estimates",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Count_3",
                table: "Estimates",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Count_4",
                table: "Estimates",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Count_5",
                table: "Estimates",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.CreateTable(
                name: "VotingObjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VoteCount = table.Column<int>(type: "int", nullable: false),
                    EstimateVotingId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VotingObjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VotingObjects_Estimates_EstimateVotingId",
                        column: x => x.EstimateVotingId,
                        principalTable: "Estimates",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_VotingObjects_EstimateVotingId",
                table: "VotingObjects",
                column: "EstimateVotingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Estimates_Posts_PostId",
                table: "Estimates",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Estimates_Posts_PostId",
                table: "Estimates");

            migrationBuilder.DropTable(
                name: "VotingObjects");

            migrationBuilder.DropColumn(
                name: "Count_1",
                table: "Estimates");

            migrationBuilder.DropColumn(
                name: "Count_2",
                table: "Estimates");

            migrationBuilder.DropColumn(
                name: "Count_3",
                table: "Estimates");

            migrationBuilder.DropColumn(
                name: "Count_4",
                table: "Estimates");

            migrationBuilder.DropColumn(
                name: "Count_5",
                table: "Estimates");

            migrationBuilder.AlterColumn<int>(
                name: "PostId",
                table: "Estimates",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateTable(
                name: "EstimateObjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstimateScaleId = table.Column<int>(type: "int", nullable: true),
                    EstimateVotingId = table.Column<int>(type: "int", nullable: true),
                    VoteCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstimateObjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EstimateObjects_Estimates_EstimateScaleId",
                        column: x => x.EstimateScaleId,
                        principalTable: "Estimates",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EstimateObjects_Estimates_EstimateVotingId",
                        column: x => x.EstimateVotingId,
                        principalTable: "Estimates",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EstimateObjects_EstimateScaleId",
                table: "EstimateObjects",
                column: "EstimateScaleId");

            migrationBuilder.CreateIndex(
                name: "IX_EstimateObjects_EstimateVotingId",
                table: "EstimateObjects",
                column: "EstimateVotingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Estimates_Posts_PostId",
                table: "Estimates",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id");
        }
    }
}
