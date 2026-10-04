using Microsoft.AspNetCore.Mvc.Testing;

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

        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();

        Assert.Contains("healthy", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Ready_returns_success()
    {
        using var response = await _client.GetAsync("/ready");

        response.EnsureSuccessStatusCode();
    }
}
