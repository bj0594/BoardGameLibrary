using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BoardGameLibrary.Api.Data;

/// <summary>
/// Supplies EF Core CLI tools with a DbContext without starting the whole application.
/// </summary>
public sealed class BoardGameDbContextFactory
    : IDesignTimeDbContextFactory<BoardGameDbContext>
{
    /// <summary>
    /// Creates the SQLite context used by commands such as `dotnet ef database update`.
    /// </summary>
    public BoardGameDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BoardGameDbContext>();

        // Keep design-time database selection aligned with the local development database.
        optionsBuilder.UseSqlite("Data Source=boardgamelibrary.db");

        return new BoardGameDbContext(optionsBuilder.Options);
    }
}
