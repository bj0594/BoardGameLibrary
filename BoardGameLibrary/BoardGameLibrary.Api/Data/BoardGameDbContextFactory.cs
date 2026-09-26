using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BoardGameLibrary.Api.Data;

/// <summary>
/// Gives EF Core CLI tooling a database context without requiring the application to start.
/// This keeps migrations independent from development-time features such as Swagger or seed data.
/// </summary>
public sealed class BoardGameDbContextFactory : IDesignTimeDbContextFactory<BoardGameDbContext>
{
    public BoardGameDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BoardGameDbContext>();
        optionsBuilder.UseSqlite("Data Source=boardgamelibrary.db");

        return new BoardGameDbContext(optionsBuilder.Options);
    }
}
