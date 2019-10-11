using Microsoft.EntityFrameworkCore.Migrations;

namespace ADSBackend.Migrations
{
    public partial class RemoveAdvisor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CoachId",
                table: "School",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_School_CoachId",
                table: "School",
                column: "CoachId");

            migrationBuilder.AddForeignKey(
                name: "FK_School_AspNetUsers_CoachId",
                table: "School",
                column: "CoachId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
