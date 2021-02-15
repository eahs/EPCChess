using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ADSBackend.Migrations
{
    public partial class LichessIntegration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JoinCode",
                table: "School",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClockIncrement",
                table: "Match",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ClockTimeLimit",
                table: "Match",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsVirtual",
                table: "Match",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ChallengeId",
                table: "Game",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChallengeJson",
                table: "Game",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChallengeUrl",
                table: "Game",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentFen",
                table: "Game",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GameJson",
                table: "Game",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsStarted",
                table: "Game",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastGameExportTime",
                table: "Game",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JoinCode",
                table: "School");

            migrationBuilder.DropColumn(
                name: "ClockIncrement",
                table: "Match");

            migrationBuilder.DropColumn(
                name: "ClockTimeLimit",
                table: "Match");

            migrationBuilder.DropColumn(
                name: "IsVirtual",
                table: "Match");

            migrationBuilder.DropColumn(
                name: "ChallengeId",
                table: "Game");

            migrationBuilder.DropColumn(
                name: "ChallengeJson",
                table: "Game");

            migrationBuilder.DropColumn(
                name: "ChallengeUrl",
                table: "Game");

            migrationBuilder.DropColumn(
                name: "CurrentFen",
                table: "Game");

            migrationBuilder.DropColumn(
                name: "GameJson",
                table: "Game");

            migrationBuilder.DropColumn(
                name: "IsStarted",
                table: "Game");

            migrationBuilder.DropColumn(
                name: "LastGameExportTime",
                table: "Game");
        }
    }
}
