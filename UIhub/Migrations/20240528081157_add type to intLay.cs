using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UIhub.Migrations
{
    /// <inheritdoc />
    public partial class addtypetointLay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SourceType",
                table: "InterfaceLayouts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SourceType",
                table: "InterfaceLayouts");
        }
    }
}
