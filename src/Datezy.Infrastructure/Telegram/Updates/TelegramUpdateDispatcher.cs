using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Datezy.Infrastructure.Telegram.Updates;

public sealed class TelegramUpdateDispatcher
{
    private readonly ILogger<TelegramUpdateDispatcher> _logger;

    public TelegramUpdateDispatcher(
        ILogger<TelegramUpdateDispatcher> logger)
    {
        _logger = logger;
    }

    public Task DispatchAsync(
        ITelegramBotClient botClient,
        Update update,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(botClient);
        ArgumentNullException.ThrowIfNull(update);

        return update.Type switch
        {
            UpdateType.Message =>
                HandleMessageAsync(
                    botClient,
                    update,
                    cancellationToken),

            UpdateType.CallbackQuery =>
                HandleCallbackQueryAsync(
                    botClient,
                    update,
                    cancellationToken),

            _ => Task.CompletedTask
        };
    }

    private Task HandleMessageAsync(
        ITelegramBotClient botClient,
        Update update,
        CancellationToken cancellationToken)
    {
        var message = update.Message;

        if (message is null)
        {
            return Task.CompletedTask;
        }

        _logger.LogDebug(
            "Received Telegram message. ChatId: {ChatId}, Type: {MessageType}",
            message.Chat.Id,
            message.Type);

        // /start routing will be added next.
        return Task.CompletedTask;
    }

    private Task HandleCallbackQueryAsync(
        ITelegramBotClient botClient,
        Update update,
        CancellationToken cancellationToken)
    {
        var callbackQuery = update.CallbackQuery;

        if (callbackQuery is null)
        {
            return Task.CompletedTask;
        }

        _logger.LogDebug(
            "Received Telegram callback query. CallbackQueryId: {CallbackQueryId}",
            callbackQuery.Id);

        // Registration callbacks will be routed here later.
        return Task.CompletedTask;
    }
}