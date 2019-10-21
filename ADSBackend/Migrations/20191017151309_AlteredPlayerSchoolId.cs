using Microsoft.EntityFrameworkCore.Migrations;

namespace ADSBackend.Migrations
{
    public partial class AlteredPlayerSchoolId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Player_School_SchoolId",
                table: "Player");

            migrationBuilder.RenameColumn(
                name: "SchoolId",
                table: "Player",
                newName: "PlayerSchoolId");

            migrationBuilder.RenameIndex(
                name: "IX_Player_SchoolId",
                table: "Player",
                newName: "IX_Player_PlayerSchoolId");

            migrationBuilder.AddForeignKey(
                name: "FK_Player_School_PlayerSchoolId",
                table: "Player",
                column: "PlayerSchoolId",
                principalTable: "School",
                principalColumn: "SchoolId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Player_School_PlayerSchoolId",
                table: "Player");

            migrationBuilder.RenameColumn(
                name: "PlayerSchoolId",
                table: "Player",
                newName: "SchoolId");

            migrationBuilder.RenameIndex(
                name: "IX_Player_PlayerSchoolId",
                table: "Player",
                newName: "IX_Player_SchoolId");

            migrationBuilder.AddForeignKey(
                name: "FK_Player_School_SchoolId",
                table: "Player",
                column: "SchoolId",
                principalTable: "School",
                principalColumn: "SchoolId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
