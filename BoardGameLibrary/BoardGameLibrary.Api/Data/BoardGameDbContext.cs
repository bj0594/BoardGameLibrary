using BoardGameLibrary.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BoardGameLibrary.Api.Data;

/// <summary>
/// EF Core boundary for the local board-game library.
/// </summary>
public class BoardGameDbContext(DbContextOptions<BoardGameDbContext> options) : DbContext(options)
{
    public DbSet<BoardGame> BoardGames => Set<BoardGame>();
    public DbSet<PlayerCountRating> PlayerCountRatings => Set<PlayerCountRating>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var boardGames = modelBuilder.Entity<BoardGame>();
        boardGames.HasKey(game => game.Id);
        boardGames.Property(game => game.Id).ValueGeneratedOnAdd();
        boardGames.Property(game => game.BggId);
        boardGames.Property(game => game.Title).IsRequired().HasMaxLength(200);
        boardGames.Property(game => game.MinPlayTimeMinutes).IsRequired();
        boardGames.Property(game => game.MaxPlayTimeMinutes).IsRequired();
        boardGames.Property(game => game.BggAverageRating).HasPrecision(4, 3);
        boardGames.Property(game => game.BggBestWith).HasMaxLength(50);
        boardGames.Property(game => game.BggUrl).HasMaxLength(500);
        boardGames.Property(game => game.CreatedAt).IsRequired();
        boardGames.HasIndex(game => game.BggId).IsUnique();

        var playerRatings = modelBuilder.Entity<PlayerCountRating>();
        playerRatings.HasKey(rating => new
        {
            rating.BoardGameId,
            rating.PlayerCount
        });
        playerRatings.Property(rating => rating.Rating)
            .HasPrecision(3, 1)
            .IsRequired();
        playerRatings.HasOne(rating => rating.BoardGame)
            .WithMany(game => game.PlayerCountRatings)
            .HasForeignKey(rating => rating.BoardGameId)
            .OnDelete(DeleteBehavior.Cascade);
        playerRatings.HasIndex(rating => rating.PlayerCount);
    }
}
