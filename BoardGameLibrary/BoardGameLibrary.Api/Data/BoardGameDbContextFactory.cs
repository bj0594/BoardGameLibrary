using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BoardGameLibrary.Api.Data;

public sealed class BoardGameDbContextFactory
    : IDesignTimeDbContextFactory<BoardGameDbContext>
{
    public BoardGameDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BoardGameDbContext>();

        optionsBuilder.UseSqlite(
            "Data Source=boardgamelibrary.db");

        return new BoardGameDbContext(optionsBuilder.Options);
    }
}