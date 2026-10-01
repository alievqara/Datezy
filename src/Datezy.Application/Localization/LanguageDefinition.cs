namespace Datezy.Application.Localization;

public sealed record LanguageDefinition(
    string Code,
    string NativeName,
    string Flag,
    IReadOnlyCollection<string> TelegramLanguageCodes,
    bool IsEnabled,
    int SortOrder);