using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace Datezy.Infrastructure.Telegram.Updates;

public sealed class TelegramUpdateReceiver
{
    private readonly ITelegramBotClient _botClient;
    private readonly TelegramUpdateDispatcher _dispatcher;
    private readonly ILogger<TelegramUpdateReceiver> _logger;

    public TelegramUpdateReceiver(
        ITelegramBotClient botClient,
        TelegramUpdateDispatcher dispatcher,
        ILogger<TelegramUpdateReceiver> logger)
    {
        _botClient = botClient;
        _dispatcher = dispatcher;
        _logger = logger;
    }

    public Task RunAsync(
        CancellationToken cancellationToken)
    {
        var receiverOptions = new ReceiverOptions
        {
            DropPendingUpdates = true
        };

        _logger.LogInformation(
            "Starting Telegram long polling.");

        _botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            errorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cancellationToken);

        return Task.CompletedTask;
    }

    private async Task HandleUpdateAsync(
        ITelegramBotClient botClient,
        Update update,
        CancellationToken cancellationToken)
    {
        try
        {
            await _dispatcher.DispatchAsync(
                botClient,
                update,
                cancellationToken);
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            // Normal shutdown.
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Unhandled exception while processing Telegram update {UpdateId}.",
                update.Id);
        }
    }

    private Task HandlePollingErrorAsync(
        ITelegramBotClient botClient,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is OperationCanceledException &&
            cancellationToken.IsCancellationRequested)
        {
            return Task.CompletedTask;
        }

        if (exception is ApiRequestException apiException)
        {
            _logger.LogError(
                apiException,
                "Telegram API error {ErrorCode}: {Message}",
                apiException.ErrorCode,
                apiException.Message);

            return Task.CompletedTask;
        }

        _logger.LogError(
            exception,
            "Telegram polling error.");

        return Task.CompletedTask;
    }
}