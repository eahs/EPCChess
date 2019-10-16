using Microsoft.EntityFrameworkCore.Migrations;

namespace ADSBackend.Migrations
{
    public partial class AddedGameBoardPosition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Game_Player_AwayPlayerId",
                table: "Game");

            migrationBuilder.DropForeignKey(
                name: "FK_Game_Player_HomePlayerId",
                table: "Game");

            migrationBuilder.AlterColumn<int>(
                name: "HomePlayerId",
                table: "Game",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "AwayPlayerId",
                table: "Game",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "BoardPosition",
                table: "Game",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Game_Player_AwayPlayerId",
                table: "Game",
                column: "AwayPlayerId",
                principalTable: "Player",
                principalColumn: "PlayerId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Game_Player_HomePlayerId",
                table: "Game",
                column: "HomePlayerId",
                principalTable: "Player",
                principalColumn: "PlayerId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Game_Player_AwayPlayerId",
                table: "Game");

            migrationBuilder.DropForeignKey(
                name: "FK_Game_Player_HomePlayerId",
                table: "Game");

            migrationBuilder.DropColumn(
                name: "BoardPosition",
                table: "Game");

            migrationBuilder.AlterColumn<int>(
                name: "HomePlayerId",
                table: "Game",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AwayPlayerId",
                table: "Game",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Game_Player_AwayPlayerId",
                table: "Game",
                column: "AwayPlayerId",
                principalTable: "Player",
                principalColumn: "PlayerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Game_Player_HomePlayerId",
                table: "Game",
                column: "HomePlayerId",
                principalTable: "Player",
                principalColumn: "PlayerId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
