using Microsoft.EntityFrameworkCore.Migrations;

namespace ADSBackend.Migrations
{
    public partial class AddedDivisionSeasonId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SeasonId",
                table: "Division",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Division_SeasonId",
                table: "Division",
                column: "SeasonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Division_Season_SeasonId",
                table: "Division",
                column: "SeasonId",
                principalTable: "Season",
                principalColumn: "SeasonId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Division_Season_SeasonId",
                table: "Division");

            migrationBuilder.DropIndex(
                name: "IX_Division_SeasonId",
                table: "Division");

            migrationBuilder.DropColumn(
                name: "SeasonId",
                table: "Division");
        }
    }
}
