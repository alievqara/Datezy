using Datezy.Infrastructure.Telegram.Updates;

namespace Datezy.Worker;

public sealed class Worker : BackgroundService
{
    private readonly TelegramUpdateReceiver _telegramUpdateReceiver;
    private readonly ILogger<Worker> _logger;

    public Worker(
        TelegramUpdateReceiver telegramUpdateReceiver,
        ILogger<Worker> logger)
    {
        _telegramUpdateReceiver = telegramUpdateReceiver;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Datezy Worker is starting.");

        await _telegramUpdateReceiver.RunAsync(
            stoppingToken);

        try
        {
            await Task.Delay(
                Timeout.Infinite,
                stoppingToken);
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
            // Expected during graceful shutdown.
        }

        _logger.LogInformation(
            "Datezy Worker is stopping.");
    }
}