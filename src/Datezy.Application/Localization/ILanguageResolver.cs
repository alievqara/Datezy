namespace Datezy.Application.Localization;

public interface ILanguageResolver
{
    string Resolve(
        string? preferredLanguageCode,
        string? telegramLanguageCode);
}