using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Xunit;

namespace RegionScope.Tests;

public sealed class HealthEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public HealthEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Health_returns_healthy()
    {
        using var response = await _client.GetAsync("/health");

        var body = await response.Content.ReadAsStringAsync();

        Assert.True(response.IsSuccessStatusCode, body);

        Assert.Contains("healthy", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Ready_returns_success()
    {
        using var response = await _client.GetAsync("/ready");

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Countries_returns_not_found_for_unknown_country()
    {
        using var response = await _client.GetAsync("/api/countries/XX");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Compare_returns_observations_for_requested_countries()
    {
        using var response = await _client.GetAsync("/api/compare?countries=IT,DE&indicator=population");

        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();

        Assert.Contains("\"indicator\":\"population\"", body, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("\"country\":\"IT\"", body, StringComparison.OrdinalIgnoreCase);
    }
}
