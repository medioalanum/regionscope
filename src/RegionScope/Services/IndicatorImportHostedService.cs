using Microsoft.Extensions.Options;

namespace RegionScope.Services;

public sealed class ImportScheduleOptions
{
    public bool Enabled { get; set; }
    public int IntervalHours { get; set; } = 24;
}

public sealed class IndicatorImportHostedService(
    IServiceScopeFactory scopeFactory,
    IOptions<ImportScheduleOptions> options,
    ILogger<IndicatorImportHostedService> logger) : BackgroundService
{
    private readonly ImportScheduleOptions _options = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.Enabled)
        {
            logger.LogInformation("Scheduled Eurostat import is disabled");
            return;
        }

        var interval = TimeSpan.FromHours(Math.Max(1, _options.IntervalHours));
        using var timer = new PeriodicTimer(interval);

        await RunImportAsync(stoppingToken);
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await RunImportAsync(stoppingToken);
        }
    }

    private async Task RunImportAsync(CancellationToken cancellationToken)
    {
        try
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var importer = scope.ServiceProvider.GetRequiredService<IndicatorImportService>();
            var imported = await importer.ImportAsync(cancellationToken);
            logger.LogInformation("Scheduled Eurostat import completed: {Imported} new observations", imported);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Scheduled Eurostat import failed");
        }
    }
}
