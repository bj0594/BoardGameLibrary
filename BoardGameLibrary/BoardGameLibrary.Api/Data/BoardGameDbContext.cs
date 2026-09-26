using BoardGameLibrary.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BoardGameLibrary.Api.Data;

public class BoardGameDbContext(DbContextOptions<BoardGameDbContext> options) : DbContext(options)
{
    public DbSet<BoardGame> BoardGames => Set<BoardGame>();
    public DbSet<PlayerCountRecommendation> PlayerCountRecommendations => Set<PlayerCountRecommendation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BoardGame>()
            .HasKey(game => game.BggId);

        modelBuilder.Entity<BoardGame>()
            .Property(game => game.BggId)
            .ValueGeneratedNever();

        modelBuilder.Entity<BoardGame>()
            .Property(game => game.Title)
            .IsRequired();

        modelBuilder.Entity<BoardGame>()
            .HasMany(game => game.PlayerCountRecommendations)
            .WithOne(recommendation => recommendation.BoardGame)
            .HasForeignKey(recommendation => recommendation.BoardGameId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PlayerCountRecommendation>()
            .HasKey(recommendation => new
            {
                recommendation.BoardGameId,
                recommendation.PlayerCount
            });
    }
}
