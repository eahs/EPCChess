using Microsoft.EntityFrameworkCore.Migrations;

namespace ADSBackend.Migrations
{
    public partial class GamePlayerRatings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AwayPlayerRatingAfter",
                table: "Game",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AwayPlayerRatingBefore",
                table: "Game",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HomePlayerRatingAfter",
                table: "Game",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HomePlayerRatingBefore",
                table: "Game",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AwayPlayerRatingAfter",
                table: "Game");

            migrationBuilder.DropColumn(
                name: "AwayPlayerRatingBefore",
                table: "Game");

            migrationBuilder.DropColumn(
                name: "HomePlayerRatingAfter",
                table: "Game");

            migrationBuilder.DropColumn(
                name: "HomePlayerRatingBefore",
                table: "Game");
        }
    }
}
