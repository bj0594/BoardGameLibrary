using BoardGameLibrary.Api.Data;
using BoardGameLibrary.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("BoardGameLibrary")
    ?? "Data Source=boardgamelibrary.db";

builder.Services.AddDbContext<BoardGameDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddHttpClient<IBoardGameGeekClient, BoardGameGeekClient>(client =>
{
    client.BaseAddress = new Uri("https://boardgamegeek.com/");
});

builder.Services.AddScoped<BoardGameService>();

if (!builder.Environment.IsEnvironment("Testing") &&
    string.IsNullOrWhiteSpace(
        builder.Configuration["BoardGameGeek:AuthorizationToken"]))
{
    throw new InvalidOperationException(
        "BoardGameGeek authorization is required. Configure BoardGameGeek:AuthorizationToken locally.");
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

if (!app.Environment.IsEnvironment("Testing"))
{
    await using var scope = app.Services.CreateAsyncScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();
    await dbContext.Database.MigrateAsync();

    app.UseHttpsRedirection();
}


app.MapControllers();

app.Run();

public partial class Program
{
    
}
