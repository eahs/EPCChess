using Microsoft.EntityFrameworkCore.Migrations;

namespace ADSBackend.Migrations
{
    public partial class SchoolAdvisorContactInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdvisorEmail",
                table: "School",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdvisorName",
                table: "School",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdvisorPhoneNumber",
                table: "School",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdvisorEmail",
                table: "School");

            migrationBuilder.DropColumn(
                name: "AdvisorName",
                table: "School");

            migrationBuilder.DropColumn(
                name: "AdvisorPhoneNumber",
                table: "School");
        }
    }
}
