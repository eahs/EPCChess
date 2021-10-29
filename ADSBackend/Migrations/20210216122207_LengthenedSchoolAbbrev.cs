using Microsoft.EntityFrameworkCore.Migrations;

namespace ADSBackend.Migrations
{
    public partial class LengthenedSchoolAbbrev : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Abbreviation",
                table: "School",
                type: "varchar(3) CHARACTER SET utf8mb4",
                maxLength: 3,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(2) CHARACTER SET utf8mb4",
                oldMaxLength: 2,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Abbreviation",
                table: "School",
                type: "varchar(2) CHARACTER SET utf8mb4",
                maxLength: 2,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(3) CHARACTER SET utf8mb4",
                oldMaxLength: 3,
                oldNullable: true);
        }
    }
}
