using Microsoft.EntityFrameworkCore.Migrations;

namespace ADSBackend.Migrations
{
    public partial class AdditionalGameFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChallengeMoves",
                table: "Game",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChallengeStatus",
                table: "Game",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CheatingDetected",
                table: "Game",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChallengeMoves",
                table: "Game");

            migrationBuilder.DropColumn(
                name: "ChallengeStatus",
                table: "Game");

            migrationBuilder.DropColumn(
                name: "CheatingDetected",
                table: "Game");
        }
    }
}
