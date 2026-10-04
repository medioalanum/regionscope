using Microsoft.EntityFrameworkCore;
using RegionScope.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<RegionScopeDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("RegionScope")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    service = "regionscope-api"
}));

app.MapHealthChecks("/ready");

app.MapGet("/api/countries", async (RegionScopeDbContext db, CancellationToken cancellationToken) =>
    Results.Ok(await db.Countries
        .AsNoTracking()
        .OrderBy(country => country.Name)
        .ToListAsync(cancellationToken)));

app.Run();

public partial class Program;
