using Microsoft.EntityFrameworkCore.Migrations;

namespace ADSBackend.Migrations
{
    public partial class ActiveSeason : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Game_Player_BlackPlayerID",
                table: "Game");

            migrationBuilder.DropForeignKey(
                name: "FK_Game_Match_MatchID",
                table: "Game");

            migrationBuilder.DropForeignKey(
                name: "FK_Game_Player_WhitePlayerID",
                table: "Game");

            migrationBuilder.DropForeignKey(
                name: "FK_Player_School_PlayerSchoolSchoolId",
                table: "Player");

            migrationBuilder.DropIndex(
                name: "IX_Player_PlayerSchoolSchoolId",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "PlayerSchoolSchoolId",
                table: "Player");

            migrationBuilder.RenameColumn(
                name: "PlayerID",
                table: "Player",
                newName: "PlayerId");

            migrationBuilder.RenameColumn(
                name: "MatchID",
                table: "Match",
                newName: "MatchId");

            migrationBuilder.RenameColumn(
                name: "WhitePlayerID",
                table: "Game",
                newName: "WhitePlayerId");

            migrationBuilder.RenameColumn(
                name: "MatchID",
                table: "Game",
                newName: "MatchId");

            migrationBuilder.RenameColumn(
                name: "BlackPlayerID",
                table: "Game",
                newName: "BlackPlayerId");

            migrationBuilder.RenameIndex(
                name: "IX_Game_WhitePlayerID",
                table: "Game",
                newName: "IX_Game_WhitePlayerId");

            migrationBuilder.RenameIndex(
                name: "IX_Game_MatchID",
                table: "Game",
                newName: "IX_Game_MatchId");

            migrationBuilder.RenameIndex(
                name: "IX_Game_BlackPlayerID",
                table: "Game",
                newName: "IX_Game_BlackPlayerId");

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "Season",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "ShortName",
                table: "School",
                maxLength: 12,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Abbreviation",
                table: "School",
                maxLength: 2,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SchoolId",
                table: "Player",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AlterColumn<int>(
                name: "MatchId",
                table: "Game",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Player_SchoolId",
                table: "Player",
                column: "SchoolId");

            migrationBuilder.AddForeignKey(
                name: "FK_Game_Player_BlackPlayerId",
                table: "Game",
                column: "BlackPlayerId",
                principalTable: "Player",
                principalColumn: "PlayerId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Game_Match_MatchId",
                table: "Game",
                column: "MatchId",
                principalTable: "Match",
                principalColumn: "MatchId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Game_Player_WhitePlayerId",
                table: "Game",
                column: "WhitePlayerId",
                principalTable: "Player",
                principalColumn: "PlayerId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Player_School_SchoolId",
                table: "Player",
                column: "SchoolId",
                principalTable: "School",
                principalColumn: "SchoolId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Game_Player_BlackPlayerId",
                table: "Game");

            migrationBuilder.DropForeignKey(
                name: "FK_Game_Match_MatchId",
                table: "Game");

            migrationBuilder.DropForeignKey(
                name: "FK_Game_Player_WhitePlayerId",
                table: "Game");

            migrationBuilder.DropForeignKey(
                name: "FK_Player_School_SchoolId",
                table: "Player");

            migrationBuilder.DropIndex(
                name: "IX_Player_SchoolId",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "Season");

            migrationBuilder.DropColumn(
                name: "SchoolId",
                table: "Player");

            migrationBuilder.RenameColumn(
                name: "PlayerId",
                table: "Player",
                newName: "PlayerID");

            migrationBuilder.RenameColumn(
                name: "MatchId",
                table: "Match",
                newName: "MatchID");

            migrationBuilder.RenameColumn(
                name: "WhitePlayerId",
                table: "Game",
                newName: "WhitePlayerID");

            migrationBuilder.RenameColumn(
                name: "MatchId",
                table: "Game",
                newName: "MatchID");

            migrationBuilder.RenameColumn(
                name: "BlackPlayerId",
                table: "Game",
                newName: "BlackPlayerID");

            migrationBuilder.RenameIndex(
                name: "IX_Game_WhitePlayerId",
                table: "Game",
                newName: "IX_Game_WhitePlayerID");

            migrationBuilder.RenameIndex(
                name: "IX_Game_MatchId",
                table: "Game",
                newName: "IX_Game_MatchID");

            migrationBuilder.RenameIndex(
                name: "IX_Game_BlackPlayerId",
                table: "Game",
                newName: "IX_Game_BlackPlayerID");

            migrationBuilder.AlterColumn<string>(
                name: "ShortName",
                table: "School",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 12,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Abbreviation",
                table: "School",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 2,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlayerSchoolSchoolId",
                table: "Player",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MatchID",
                table: "Game",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.CreateIndex(
                name: "IX_Player_PlayerSchoolSchoolId",
                table: "Player",
                column: "PlayerSchoolSchoolId");

            migrationBuilder.AddForeignKey(
                name: "FK_Game_Player_BlackPlayerID",
                table: "Game",
                column: "BlackPlayerID",
                principalTable: "Player",
                principalColumn: "PlayerID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Game_Match_MatchID",
                table: "Game",
                column: "MatchID",
                principalTable: "Match",
                principalColumn: "MatchID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Game_Player_WhitePlayerID",
                table: "Game",
                column: "WhitePlayerID",
                principalTable: "Player",
                principalColumn: "PlayerID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Player_School_PlayerSchoolSchoolId",
                table: "Player",
                column: "PlayerSchoolSchoolId",
                principalTable: "School",
                principalColumn: "SchoolId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
