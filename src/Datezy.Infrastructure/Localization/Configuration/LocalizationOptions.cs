namespace Datezy.Infrastructure.Localization.Configuration;

public sealed class LocalizationOptions
{
    public const string SectionName = "Localization";

    public string DefaultLanguage { get; init; } = "en";

    public List<LanguageOptions> Languages { get; init; } = [];
}