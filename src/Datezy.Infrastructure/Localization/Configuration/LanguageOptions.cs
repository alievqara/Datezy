namespace Datezy.Infrastructure.Localization.Configuration;

public sealed class LanguageOptions
{
    public string Code { get; init; } = string.Empty;

    public string NativeName { get; init; } = string.Empty;

    public string Flag { get; init; } = string.Empty;

    public string[] TelegramLanguageCodes { get; init; } = [];

    public bool Enabled { get; init; } = true;

    public int SortOrder { get; init; }
}