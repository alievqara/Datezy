namespace Datezy.Application.Localization;

public interface ILanguageCatalog
{
    IReadOnlyCollection<LanguageDefinition> GetEnabledLanguages();

    LanguageDefinition? FindByCode(string? code);

    bool IsSupported(string? code);
}