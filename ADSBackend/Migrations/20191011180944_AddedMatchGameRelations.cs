using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ADSBackend.Migrations
{
    public partial class AddedMatchGameRelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Game_Player_BlackPlayerId",
                table: "Game");

            migrationBuilder.DropForeignKey(
                name: "FK_Game_Player_WhitePlayerId",
                table: "Game");

            migrationBuilder.DropForeignKey(
                name: "FK_Match_School_AwaySchoolSchoolId",
                table: "Match");

            migrationBuilder.DropForeignKey(
                name: "FK_Match_School_HomeSchoolSchoolId",
                table: "Match");

            migrationBuilder.DropIndex(
                name: "IX_Match_AwaySchoolSchoolId",
                table: "Match");

            migrationBuilder.DropIndex(
                name: "IX_Match_HomeSchoolSchoolId",
                table: "Match");

            migrationBuilder.DropIndex(
                name: "IX_Game_BlackPlayerId",
                table: "Game");

            migrationBuilder.DropIndex(
                name: "IX_Game_WhitePlayerId",
                table: "Game");

            migrationBuilder.DropColumn(
                name: "AwaySchoolSchoolId",
                table: "Match");

            migrationBuilder.DropColumn(
                name: "HomeSchoolSchoolId",
                table: "Match");

            migrationBuilder.DropColumn(
                name: "BlackPlayerId",
                table: "Game");

            migrationBuilder.DropColumn(
                name: "WhitePlayerId",
                table: "Game");

            migrationBuilder.AddColumn<int>(
                name: "AwaySchoolId",
                table: "Match",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HomeSchoolId",
                table: "Match",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "MatchStartTime",
                table: "Match",
                nullable: false,
                defaultValue: DateTime.Now);

            migrationBuilder.AddColumn<int>(
                name: "AwayPlayerId",
                table: "Game",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HomePlayerId",
                table: "Game",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Match_AwaySchoolId",
                table: "Match",
                column: "AwaySchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_Match_HomeSchoolId",
                table: "Match",
                column: "HomeSchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_Game_AwayPlayerId",
                table: "Game",
                column: "AwayPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Game_HomePlayerId",
                table: "Game",
                column: "HomePlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Game_Player_AwayPlayerId",
                table: "Game",
                column: "AwayPlayerId",
                principalTable: "Player",
                principalColumn: "PlayerId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Game_Player_HomePlayerId",
                table: "Game",
                column: "HomePlayerId",
                principalTable: "Player",
                principalColumn: "PlayerId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Match_School_AwaySchoolId",
                table: "Match",
                column: "AwaySchoolId",
                principalTable: "School",
                principalColumn: "SchoolId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Match_School_HomeSchoolId",
                table: "Match",
                column: "HomeSchoolId",
                principalTable: "School",
                principalColumn: "SchoolId",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Game_Player_AwayPlayerId",
                table: "Game");

            migrationBuilder.DropForeignKey(
                name: "FK_Game_Player_HomePlayerId",
                table: "Game");

            migrationBuilder.DropForeignKey(
                name: "FK_Match_School_AwaySchoolId",
                table: "Match");

            migrationBuilder.DropForeignKey(
                name: "FK_Match_School_HomeSchoolId",
                table: "Match");

            migrationBuilder.DropIndex(
                name: "IX_Match_AwaySchoolId",
                table: "Match");

            migrationBuilder.DropIndex(
                name: "IX_Match_HomeSchoolId",
                table: "Match");

            migrationBuilder.DropIndex(
                name: "IX_Game_AwayPlayerId",
                table: "Game");

            migrationBuilder.DropIndex(
                name: "IX_Game_HomePlayerId",
                table: "Game");

            migrationBuilder.DropColumn(
                name: "AwaySchoolId",
                table: "Match");

            migrationBuilder.DropColumn(
                name: "HomeSchoolId",
                table: "Match");

            migrationBuilder.DropColumn(
                name: "MatchStartTime",
                table: "Match");

            migrationBuilder.DropColumn(
                name: "AwayPlayerId",
                table: "Game");

            migrationBuilder.DropColumn(
                name: "HomePlayerId",
                table: "Game");

            migrationBuilder.AddColumn<int>(
                name: "AwaySchoolSchoolId",
                table: "Match",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HomeSchoolSchoolId",
                table: "Match",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BlackPlayerId",
                table: "Game",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WhitePlayerId",
                table: "Game",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Match_AwaySchoolSchoolId",
                table: "Match",
                column: "AwaySchoolSchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_Match_HomeSchoolSchoolId",
                table: "Match",
                column: "HomeSchoolSchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_Game_BlackPlayerId",
                table: "Game",
                column: "BlackPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Game_WhitePlayerId",
                table: "Game",
                column: "WhitePlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Game_Player_BlackPlayerId",
                table: "Game",
                column: "BlackPlayerId",
                principalTable: "Player",
                principalColumn: "PlayerId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Game_Player_WhitePlayerId",
                table: "Game",
                column: "WhitePlayerId",
                principalTable: "Player",
                principalColumn: "PlayerId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Match_School_AwaySchoolSchoolId",
                table: "Match",
                column: "AwaySchoolSchoolId",
                principalTable: "School",
                principalColumn: "SchoolId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Match_School_HomeSchoolSchoolId",
                table: "Match",
                column: "HomeSchoolSchoolId",
                principalTable: "School",
                principalColumn: "SchoolId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
