using BoardGameLibrary.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BoardGameLibrary.Api.Data;

/// <summary>
/// EF Core persistence boundary for the local board-game collection.
/// </summary>
public class BoardGameDbContext(DbContextOptions<BoardGameDbContext> options) : DbContext(options)
{
    /// <summary>The board games persisted in the local SQLite database.</summary>
    public DbSet<BoardGame> BoardGames => Set<BoardGame>();

    /// <summary>
    /// Defines database keys, required fields, and the title length constraint used by the model.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var boardGames = modelBuilder.Entity<BoardGame>();

        boardGames.HasKey(game => game.Id);
        boardGames.Property(game => game.Id).ValueGeneratedOnAdd();
        boardGames.Property(game => game.Title).IsRequired().HasMaxLength(200);
        boardGames.Property(game => game.CreatedAt).IsRequired();
    }
}
