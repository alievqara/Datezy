using Datezy.Application.Common.Abstractions;
using Datezy.Application.Localization;
using Datezy.Infrastructure.Localization;
using Datezy.Infrastructure.Localization.Configuration;
using Datezy.Infrastructure.Telegram;
using Datezy.Infrastructure.Time;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Datezy.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services
            .AddOptions<LocalizationOptions>()
            .Bind(
                configuration.GetSection(
                    LocalizationOptions.SectionName))
            .Validate(
                options =>
                    !string.IsNullOrWhiteSpace(
                        options.DefaultLanguage),
                "Default localization language is required.")
            .Validate(
                options =>
                    options.Languages.Count > 0,
                "At least one localization language is required.")
            .ValidateOnStart();

        services.AddSingleton<
            ILanguageCatalog,
            LanguageCatalog>();

        services.AddSingleton<
            ILanguageResolver,
            LanguageResolver>();

        services.AddSingleton<
            ILocalizer,
            JsonLocalizer>();

        services.AddSingleton<IClock, SystemClock>();

        services.AddTelegram(configuration);

        return services;
    }
}