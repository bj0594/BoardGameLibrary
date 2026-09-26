using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoardGameLibrary.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BoardGames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BggId = table.Column<int>(type: "INTEGER", nullable: true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    MinPlayers = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxPlayers = table.Column<int>(type: "INTEGER", nullable: false),
                    MinPlayTimeMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxPlayTimeMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    BggAverageRating = table.Column<decimal>(type: "TEXT", precision: 4, scale: 3, nullable: true),
                    BggBestWith = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    BggUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoardGames", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlayerCountRatings",
                columns: table => new
                {
                    BoardGameId = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayerCount = table.Column<int>(type: "INTEGER", nullable: false),
                    Rating = table.Column<decimal>(type: "TEXT", precision: 3, scale: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerCountRatings", x => new { x.BoardGameId, x.PlayerCount });
                    table.ForeignKey(
                        name: "FK_PlayerCountRatings_BoardGames_BoardGameId",
                        column: x => x.BoardGameId,
                        principalTable: "BoardGames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BoardGames_BggId",
                table: "BoardGames",
                column: "BggId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCountRatings_PlayerCount",
                table: "PlayerCountRatings",
                column: "PlayerCount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerCountRatings");

            migrationBuilder.DropTable(
                name: "BoardGames");
        }
    }
}
