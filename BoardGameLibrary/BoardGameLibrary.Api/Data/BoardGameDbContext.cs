using BoardGameLibrary.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BoardGameLibrary.Api.Data;

public class BoardGameDbContext(DbContextOptions<BoardGameDbContext> options) : DbContext(options)
{
    public DbSet<BoardGame> BoardGames => Set<BoardGame>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var boardGames = modelBuilder.Entity<BoardGame>();

        boardGames.HasKey(game => game.Id);
        boardGames.Property(game => game.Id).ValueGeneratedOnAdd();
        boardGames.Property(game => game.Title).IsRequired().HasMaxLength(200);
        boardGames.Property(game => game.CreatedAt).IsRequired();
    }
}
