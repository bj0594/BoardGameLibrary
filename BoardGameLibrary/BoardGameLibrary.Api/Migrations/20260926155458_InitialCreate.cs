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
                    BggId = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    MinPlayers = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxPlayers = table.Column<int>(type: "INTEGER", nullable: false),
                    BggAverageRating = table.Column<double>(type: "REAL", nullable: true),
                    BggRatingCount = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoardGames", x => x.BggId);
                });

            migrationBuilder.CreateTable(
                name: "PlayerCountRecommendations",
                columns: table => new
                {
                    PlayerCount = table.Column<string>(type: "TEXT", nullable: false),
                    BoardGameId = table.Column<int>(type: "INTEGER", nullable: false),
                    BestVotes = table.Column<int>(type: "INTEGER", nullable: false),
                    RecommendedVotes = table.Column<int>(type: "INTEGER", nullable: false),
                    NotRecommendedVotes = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerCountRecommendations", x => new { x.BoardGameId, x.PlayerCount });
                    table.ForeignKey(
                        name: "FK_PlayerCountRecommendations_BoardGames_BoardGameId",
                        column: x => x.BoardGameId,
                        principalTable: "BoardGames",
                        principalColumn: "BggId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerCountRecommendations");

            migrationBuilder.DropTable(
                name: "BoardGames");
        }
    }
}
