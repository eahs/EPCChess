using Microsoft.EntityFrameworkCore.Migrations;

namespace ADSBackend.Migrations
{
    public partial class MatchRosterLocking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AwayRosterLocked",
                table: "Match",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HomeRosterLocked",
                table: "Match",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MatchStarted",
                table: "Match",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AwayRosterLocked",
                table: "Match");

            migrationBuilder.DropColumn(
                name: "HomeRosterLocked",
                table: "Match");

            migrationBuilder.DropColumn(
                name: "MatchStarted",
                table: "Match");
        }
    }
}
