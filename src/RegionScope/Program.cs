using Microsoft.EntityFrameworkCore;
using RegionScope.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks()
    .AddDbContextCheck<RegionScopeDbContext>();
builder.Services.AddDbContext<RegionScopeDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("RegionScope")));

var app = builder.Build();

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

app.MapGet("/api/countries/{code}", async (string code, RegionScopeDbContext db, CancellationToken cancellationToken) =>
{
    var country = await db.Countries
        .AsNoTracking()
        .SingleOrDefaultAsync(item => item.Code == code.ToUpperInvariant(), cancellationToken);

    return country is null ? Results.NotFound() : Results.Ok(country);
});

app.MapGet("/api/countries/{code}/indicators/{indicator}", async (
    string code, string indicator, RegionScopeDbContext db, CancellationToken cancellationToken) =>
{
    var countryCode = code.ToUpperInvariant();
    var indicatorCode = indicator.ToLowerInvariant();
    var exists = await db.Countries.AnyAsync(country => country.Code == countryCode, cancellationToken)
        && await db.Indicators.AnyAsync(item => item.Code == indicatorCode, cancellationToken);

    if (!exists)
    {
        return Results.NotFound();
    }

    var observations = await db.Observations.AsNoTracking()
        .Where(observation => observation.CountryCode == countryCode && observation.IndicatorCode == indicatorCode)
        .OrderBy(observation => observation.Year)
        .ToListAsync(cancellationToken);

    return Results.Ok(observations);
});

app.Run();

public partial class Program;
