using Datezy.Application.Localization;
using Datezy.Infrastructure.Localization.Configuration;
using Microsoft.Extensions.Options;

namespace Datezy.Infrastructure.Localization;

public sealed class LanguageResolver : ILanguageResolver
{
    private readonly ILanguageCatalog _catalog;
    private readonly LocalizationOptions _options;

    public LanguageResolver(
        ILanguageCatalog catalog,
        IOptions<LocalizationOptions> options)
    {
        _catalog = catalog;
        _options = options.Value;
    }

    public string Resolve(
        string? preferredLanguageCode,
        string? telegramLanguageCode)
    {
        var preferred = _catalog.FindByCode(
            preferredLanguageCode);

        if (preferred is not null)
        {
            return preferred.Code;
        }

        var telegramLanguage = ResolveTelegramLanguage(
            telegramLanguageCode);

        if (telegramLanguage is not null)
        {
            return telegramLanguage.Code;
        }

        var fallback = _catalog.FindByCode(
            _options.DefaultLanguage);

        if (fallback is null)
        {
            throw new InvalidOperationException(
                $"Default language '{_options.DefaultLanguage}' is not enabled.");
        }

        return fallback.Code;
    }

    private LanguageDefinition? ResolveTelegramLanguage(
        string? telegramLanguageCode)
    {
        if (string.IsNullOrWhiteSpace(telegramLanguageCode))
        {
            return null;
        }

        var normalized = telegramLanguageCode
            .Trim()
            .Replace('_', '-')
            .ToLowerInvariant();

        var languages = _catalog.GetEnabledLanguages();

        var exact = languages.FirstOrDefault(
            language =>
                language.TelegramLanguageCodes.Any(
                    code => string.Equals(
                        code,
                        normalized,
                        StringComparison.OrdinalIgnoreCase)));

        if (exact is not null)
        {
            return exact;
        }

        var separatorIndex = normalized.IndexOf('-');

        var neutralCode = separatorIndex > 0
            ? normalized[..separatorIndex]
            : normalized;

        return languages.FirstOrDefault(
            language =>
                string.Equals(
                    language.Code,
                    neutralCode,
                    StringComparison.OrdinalIgnoreCase));
    }
}