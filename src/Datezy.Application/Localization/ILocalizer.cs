namespace Datezy.Application.Localization;

public interface ILocalizer
{
    string Get(
        string key,
        string languageCode);

    string Get(
        string key,
        string languageCode,
        params object[] arguments);
}