using BoardGameLibrary.Api.Data;
using BoardGameLibrary.Api.SeedData;
using BoardGameLibrary.Api.Services;
using Microsoft.EntityFrameworkCore;

// Application composition root: register infrastructure, persistence, and application services.
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("BoardGameLibrary")
    ?? "Data Source=boardgamelibrary.db";

builder.Services.AddDbContext<BoardGameDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddScoped<BoardGameService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // OpenAPI describes the API contract; Swagger UI provides the human-friendly test surface.
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.RoutePrefix = string.Empty;
        options.SwaggerEndpoint("/openapi/v1.json", "BoardGame Library API v1");
    });

    // Development startup prepares the local schema automatically.
    // The demo database is then populated only when it is empty, so normal user data is never overwritten.
    await using (var scope = app.Services.CreateAsyncScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    await BoardGameSeeder.SeedAsync(app.Services);
}

app.MapControllers();

app.Run();

// WebApplicationFactory needs an accessible Program type for API integration tests.
public partial class Program
{
}
