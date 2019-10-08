using Microsoft.EntityFrameworkCore.Migrations;

namespace ADSBackend.Migrations
{
    public partial class UpdatedSchoolAttributes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Abbreviation",
                table: "School",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShortName",
                table: "School",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Abbreviation",
                table: "School");

            migrationBuilder.DropColumn(
                name: "ShortName",
                table: "School");
        }
    }
}
