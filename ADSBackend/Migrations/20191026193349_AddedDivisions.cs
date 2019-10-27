using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ADSBackend.Migrations
{
    public partial class AddedDivisions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DivisionId",
                table: "School",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Division",
                columns: table => new
                {
                    DivisionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Division", x => x.DivisionId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_School_DivisionId",
                table: "School",
                column: "DivisionId");

            migrationBuilder.AddForeignKey(
                name: "FK_School_Division_DivisionId",
                table: "School",
                column: "DivisionId",
                principalTable: "Division",
                principalColumn: "DivisionId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_School_Division_DivisionId",
                table: "School");

            migrationBuilder.DropTable(
                name: "Division");

            migrationBuilder.DropIndex(
                name: "IX_School_DivisionId",
                table: "School");

            migrationBuilder.DropColumn(
                name: "DivisionId",
                table: "School");
        }
    }
}
