using System.Text.Json;

namespace RegionScope.Clients;

public sealed record EurostatObservation(string Geo, int Year, decimal Value);

public sealed class EurostatClient(HttpClient httpClient, ILogger<EurostatClient> logger)
{
    private const string BasePath = "dissemination/statistics/1.0/data/";

    public async Task<IReadOnlyCollection<EurostatObservation>> GetAnnualObservationsAsync(
        string dataset,
        IReadOnlyDictionary<string, string> filters,
        CancellationToken cancellationToken)
    {
        var query = string.Join("&", filters.Select(pair =>
            $"{Uri.EscapeDataString(pair.Key)}={Uri.EscapeDataString(pair.Value)}"));
        var uri = $"{BasePath}{dataset}?format=JSON&lang=EN&{query}";

        using var response = await httpClient.GetAsync(uri, cancellationToken);
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

        return Parse(document.RootElement);
    }

    private IReadOnlyCollection<EurostatObservation> Parse(JsonElement root)
    {
        var ids = root.GetProperty("id").EnumerateArray().Select(item => item.GetString()!).ToArray();
        var sizes = root.GetProperty("size").EnumerateArray().Select(item => item.GetInt32()).ToArray();
        var values = root.GetProperty("value").EnumerateArray().ToArray();
        var dimensions = ids.Select(id => ReadDimension(root.GetProperty("dimension").GetProperty(id))).ToArray();
        var results = new List<EurostatObservation>();

        for (var flatIndex = 0; flatIndex < values.Length; flatIndex++)
        {
            if (values[flatIndex].ValueKind == JsonValueKind.Null)
            {
                continue;
            }

            var coordinates = DecodeIndex(flatIndex, sizes);
            var geoIndex = Array.IndexOf(ids, "geo");
            var timeIndex = Array.IndexOf(ids, "time");
            if (geoIndex < 0 || timeIndex < 0)
            {
                throw new InvalidOperationException("Eurostat response does not contain geo and time dimensions.");
            }

            if (!int.TryParse(dimensions[timeIndex][coordinates[timeIndex]], out var year))
            {
                continue;
            }

            results.Add(new EurostatObservation(
                dimensions[geoIndex][coordinates[geoIndex]],
                year,
                values[flatIndex].GetDecimal()));
        }

        logger.LogInformation("Parsed {Count} observations from Eurostat", results.Count);
        return results;
    }

    private static string[] ReadDimension(JsonElement dimension)
    {
        var index = dimension.GetProperty("category").GetProperty("index");
        return index.ValueKind == JsonValueKind.Object
            ? index.EnumerateObject().OrderBy(item => item.Value.GetInt32()).Select(item => item.Name).ToArray()
            : index.EnumerateArray().Select(item => item.GetString()!).ToArray();
    }

    private static int[] DecodeIndex(int flatIndex, IReadOnlyList<int> sizes)
    {
        var coordinates = new int[sizes.Count];
        for (var dimension = sizes.Count - 1; dimension >= 0; dimension--)
        {
            coordinates[dimension] = flatIndex % sizes[dimension];
            flatIndex /= sizes[dimension];
        }

        return coordinates;
    }
}
