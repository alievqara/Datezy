using Datezy.Application.Localization;
using Datezy.Infrastructure.Localization.Configuration;
using Microsoft.Extensions.Options;

namespace Datezy.Infrastructure.Localization;

public sealed class LanguageCatalog : ILanguageCatalog
{
    private readonly IReadOnlyCollection<LanguageDefinition> _languages;

    public LanguageCatalog(
        IOptions<LocalizationOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _languages = options.Value.Languages
            .Select(language => new LanguageDefinition(
                Normalize(language.Code),
                language.NativeName.Trim(),
                language.Flag.Trim(),
                language.TelegramLanguageCodes
                    .Where(code => !string.IsNullOrWhiteSpace(code))
                    .Select(Normalize)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToArray(),
                language.Enabled,
                language.SortOrder))
            .OrderBy(language => language.SortOrder)
            .ToArray();
    }

    public IReadOnlyCollection<LanguageDefinition> GetEnabledLanguages()
    {
        return _languages
            .Where(language => language.IsEnabled)
            .ToArray();
    }

    public LanguageDefinition? FindByCode(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return null;
        }

        var normalizedCode = Normalize(code);

        return _languages.FirstOrDefault(
            language =>
                language.IsEnabled &&
                string.Equals(
                    language.Code,
                    normalizedCode,
                    StringComparison.OrdinalIgnoreCase));
    }

    public bool IsSupported(string? code)
    {
        return FindByCode(code) is not null;
    }

    private static string Normalize(string value)
    {
        return value.Trim().ToLowerInvariant();
    }
}