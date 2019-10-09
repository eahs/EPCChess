using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ADSBackend.Migrations
{
    public partial class AlteredSeasonDateSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Active",
                table: "Season");

            migrationBuilder.DropColumn(
                name: "StartingYear",
                table: "Season");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Season",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Season",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Season");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Season");

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "Season",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "StartingYear",
                table: "Season",
                nullable: false,
                defaultValue: 0);
        }
    }
}
