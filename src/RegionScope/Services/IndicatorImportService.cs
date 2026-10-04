using Microsoft.EntityFrameworkCore;
using RegionScope.Clients;
using RegionScope.Data;
using RegionScope.Models;

namespace RegionScope.Services;

public sealed class IndicatorImportService(
    RegionScopeDbContext db,
    EurostatClient eurostatClient,
    ILogger<IndicatorImportService> logger)
{
    private static readonly IReadOnlyDictionary<string, (string Dataset, IReadOnlyDictionary<string, string> Filters)> Sources =
        new Dictionary<string, (string, IReadOnlyDictionary<string, string>)>
        {
            ["population"] = ("tps00001", new Dictionary<string, string>
            {
                ["freq"] = "A", ["indic_de"] = "JAN", ["lastTimePeriod"] = "10"
            }),
            ["gdp_per_capita"] = ("nama_10_pc", new Dictionary<string, string>
            {
                ["freq"] = "A", ["unit"] = "CP_EUR_HAB", ["na_item"] = "B1GQ", ["lastTimePeriod"] = "10"
            }),
            ["unemployment_rate"] = ("une_rt_a", new Dictionary<string, string>
            {
                ["freq"] = "A", ["unit"] = "PC_ACT", ["age"] = "Y15-74", ["sex"] = "T", ["lastTimePeriod"] = "10"
            })
        };

    public async Task<int> ImportAsync(CancellationToken cancellationToken)
    {
        var countryCodes = await db.Countries.AsNoTracking().Select(country => country.Code).ToListAsync(cancellationToken);
        var imported = 0;

        foreach (var (indicatorCode, source) in Sources)
        {
            var observations = await eurostatClient.GetAnnualObservationsAsync(source.Dataset, source.Filters, cancellationToken);
            var allowed = observations.Where(observation => countryCodes.Contains(observation.Geo)).ToList();
            foreach (var observation in allowed)
            {
                var entity = await db.Observations.FindAsync(
                    [observation.Geo, indicatorCode, observation.Year], cancellationToken);

                if (entity is null)
                {
                    db.Observations.Add(new Observation
                    {
                        CountryCode = observation.Geo,
                        IndicatorCode = indicatorCode,
                        Year = observation.Year,
                        Value = observation.Value,
                        Unit = indicatorCode == "population" ? "persons" : indicatorCode == "gdp_per_capita" ? "EUR per person" : "percentage",
                        Source = "Eurostat",
                        RetrievedAtUtc = DateTime.UtcNow
                    });
                    imported++;
                }
                else
                {
                    entity.Value = observation.Value;
                    entity.RetrievedAtUtc = DateTime.UtcNow;
                }
            }

            await db.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Imported {Count} {Indicator} observations", allowed.Count, indicatorCode);
        }

        return imported;
    }
}
