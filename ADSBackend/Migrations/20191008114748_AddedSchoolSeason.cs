using Microsoft.EntityFrameworkCore.Migrations;

namespace ADSBackend.Migrations
{
    public partial class AddedSchoolSeason : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SeasonId",
                table: "School",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_School_SeasonId",
                table: "School",
                column: "SeasonId");

            migrationBuilder.AddForeignKey(
                name: "FK_School_Season_SeasonId",
                table: "School",
                column: "SeasonId",
                principalTable: "Season",
                principalColumn: "SeasonID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_School_Season_SeasonId",
                table: "School");

            migrationBuilder.DropIndex(
                name: "IX_School_SeasonId",
                table: "School");

            migrationBuilder.DropColumn(
                name: "SeasonId",
                table: "School");
        }
    }
}
