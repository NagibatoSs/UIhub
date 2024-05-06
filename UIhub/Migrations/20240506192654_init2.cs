using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UIhub.Migrations
{
    /// <inheritdoc />
    public partial class init2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1ba64fb1-faf0-424f-8a66-af45363d4c58", "8ff7a4c5-33b3-4332-a4d4-979ba86ec589" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1ba64fb1-faf0-424f-8a66-af45363d4c58");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8ff7a4c5-33b3-4332-a4d4-979ba86ec589");

            migrationBuilder.AddColumn<int>(
                name: "RankId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_RankId",
                table: "AspNetUsers",
                column: "RankId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_UserRanks_RankId",
                table: "AspNetUsers",
                column: "RankId",
                principalTable: "UserRanks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_UserRanks_RankId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_RankId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RankId",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "1ba64fb1-faf0-424f-8a66-af45363d4c58", null, "admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "Points", "Reputation", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "8ff7a4c5-33b3-4332-a4d4-979ba86ec589", 0, "f727e45e-f676-438e-8a21-1bb4d2bc3fb0", "User", "zhuravleva_02@mail.ru", true, false, null, null, "NAGIBATOSS", "AQAAAAIAAYagAAAAEC6WQRQ3O9V5U0dyYKZLt7L+srHZZP8Gta49ALXsBGU1at6ky8Jvl5M+2UiFGQLlTA==", null, false, 0, 0, "", false, "NagibatoSs" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1ba64fb1-faf0-424f-8a66-af45363d4c58", "8ff7a4c5-33b3-4332-a4d4-979ba86ec589" });
        }
    }
}
