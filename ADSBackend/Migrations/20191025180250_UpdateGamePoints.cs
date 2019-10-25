using Microsoft.EntityFrameworkCore.Migrations;

namespace ADSBackend.Migrations
{
    public partial class UpdateGamePoints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Result",
                table: "Game");

            migrationBuilder.AddColumn<double>(
                name: "AwayPoints",
                table: "Game",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "HomePoints",
                table: "Game",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AwayPoints",
                table: "Game");

            migrationBuilder.DropColumn(
                name: "HomePoints",
                table: "Game");

            migrationBuilder.AddColumn<int>(
                name: "Result",
                table: "Game",
                nullable: false,
                defaultValue: 0);
        }
    }
}
