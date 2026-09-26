using BoardGameLibrary.Api.Data;
using BoardGameLibrary.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Controllers provide the required HTTP boundary for the API.
builder.Services.AddControllers();

// ASP.NET Core generates the OpenAPI document; Swagger UI consumes it in Development.
builder.Services.AddOpenApi();

// SQLite keeps persistence local and requires no external database service for the assignment.
var connectionString = builder.Configuration.GetConnectionString("BoardGameLibrary")
    ?? "Data Source=boardgamelibrary.db";

builder.Services.AddDbContext<BoardGameDbContext>(options =>
    options.UseSqlite(connectionString));

// Keep application/database behaviour out of the Controller.
builder.Services.AddScoped<BoardGameService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Expose the generated OpenAPI document only during local development.
    app.MapOpenApi();

    // Serve Swagger UI directly at the application root for a simple local testing experience.
    app.UseSwaggerUI(options =>
    {
        options.RoutePrefix = string.Empty;
        options.DocumentTitle = "BoardGame Library API";
        options.SwaggerEndpoint("/openapi/v1.json", "BoardGame Library API v1");
    });
}

// All assignment API operations are Controller-based under /api/games.
app.MapControllers();

app.Run();

// WebApplicationFactory<Program> uses this public marker to create the real API host in tests.
public partial class Program
{
}
