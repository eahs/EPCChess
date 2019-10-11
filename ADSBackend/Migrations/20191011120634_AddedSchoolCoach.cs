using Microsoft.EntityFrameworkCore.Migrations;

namespace ADSBackend.Migrations
{
    public partial class AddedSchoolCoach : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CoachId",
                table: "School",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_School_CoachId",
                table: "School",
                column: "CoachId");

            migrationBuilder.AddForeignKey(
                name: "FK_School_AspNetUsers_CoachId",
                table: "School",
                column: "CoachId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_School_AspNetUsers_CoachId",
                table: "School");

            migrationBuilder.DropIndex(
                name: "IX_School_CoachId",
                table: "School");

            migrationBuilder.DropColumn(
                name: "CoachId",
                table: "School");
        }
    }
}
