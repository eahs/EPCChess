using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ADSBackend.Migrations
{
    public partial class ChessModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "School",
                columns: table => new
                {
                    SchoolId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_School", x => x.SchoolId);
                });

            migrationBuilder.CreateTable(
                name: "Season",
                columns: table => new
                {
                    SeasonID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    StartingYear = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Season", x => x.SeasonID);
                });

            migrationBuilder.CreateTable(
                name: "Match",
                columns: table => new
                {
                    MatchID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MatchDate = table.Column<DateTime>(nullable: false),
                    HomeSchoolSchoolId = table.Column<int>(nullable: true),
                    AwaySchoolSchoolId = table.Column<int>(nullable: true),
                    Completed = table.Column<bool>(nullable: false),
                    HomePoints = table.Column<double>(nullable: false),
                    AwayPoints = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Match", x => x.MatchID);
                    table.ForeignKey(
                        name: "FK_Match_School_AwaySchoolSchoolId",
                        column: x => x.AwaySchoolSchoolId,
                        principalTable: "School",
                        principalColumn: "SchoolId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Match_School_HomeSchoolSchoolId",
                        column: x => x.HomeSchoolSchoolId,
                        principalTable: "School",
                        principalColumn: "SchoolId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Player",
                columns: table => new
                {
                    PlayerID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PlayerSchoolSchoolId = table.Column<int>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Rating = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Player", x => x.PlayerID);
                    table.ForeignKey(
                        name: "FK_Player_School_PlayerSchoolSchoolId",
                        column: x => x.PlayerSchoolSchoolId,
                        principalTable: "School",
                        principalColumn: "SchoolId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Game",
                columns: table => new
                {
                    GameId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WhitePlayerID = table.Column<int>(nullable: true),
                    BlackPlayerID = table.Column<int>(nullable: true),
                    Completed = table.Column<bool>(nullable: false),
                    Result = table.Column<int>(nullable: false),
                    MatchID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Game", x => x.GameId);
                    table.ForeignKey(
                        name: "FK_Game_Player_BlackPlayerID",
                        column: x => x.BlackPlayerID,
                        principalTable: "Player",
                        principalColumn: "PlayerID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Game_Match_MatchID",
                        column: x => x.MatchID,
                        principalTable: "Match",
                        principalColumn: "MatchID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Game_Player_WhitePlayerID",
                        column: x => x.WhitePlayerID,
                        principalTable: "Player",
                        principalColumn: "PlayerID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Game_BlackPlayerID",
                table: "Game",
                column: "BlackPlayerID");

            migrationBuilder.CreateIndex(
                name: "IX_Game_MatchID",
                table: "Game",
                column: "MatchID");

            migrationBuilder.CreateIndex(
                name: "IX_Game_WhitePlayerID",
                table: "Game",
                column: "WhitePlayerID");

            migrationBuilder.CreateIndex(
                name: "IX_Match_AwaySchoolSchoolId",
                table: "Match",
                column: "AwaySchoolSchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_Match_HomeSchoolSchoolId",
                table: "Match",
                column: "HomeSchoolSchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_Player_PlayerSchoolSchoolId",
                table: "Player",
                column: "PlayerSchoolSchoolId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Game");

            migrationBuilder.DropTable(
                name: "Season");

            migrationBuilder.DropTable(
                name: "Player");

            migrationBuilder.DropTable(
                name: "Match");

            migrationBuilder.DropTable(
                name: "School");
        }
    }
}
