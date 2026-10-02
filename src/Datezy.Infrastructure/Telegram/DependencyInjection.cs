using Datezy.Infrastructure.Telegram.Updates;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace Datezy.Infrastructure.Telegram;

public static class DependencyInjection
{
    public static IServiceCollection AddTelegram(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services
            .AddOptions<TelegramOptions>()
            .Bind(
                configuration.GetSection(
                    TelegramOptions.SectionName))
            .Validate(
                options =>
                    !string.IsNullOrWhiteSpace(
                        options.BotToken),
                "Telegram bot token is not configured.")
            .ValidateOnStart();

        services.AddSingleton<ITelegramBotClient>(
            serviceProvider =>
            {
                var options = serviceProvider
                    .GetRequiredService<
                        IOptions<TelegramOptions>>()
                    .Value;

                return new TelegramBotClient(
                    options.BotToken);
            });

        services.AddSingleton<TelegramUpdateDispatcher>();

        services.AddSingleton<TelegramUpdateReceiver>();

        return services;
    }
}