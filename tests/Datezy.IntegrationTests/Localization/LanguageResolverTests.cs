using Datezy.Application.Localization;
using Datezy.Infrastructure.Localization;
using Datezy.Infrastructure.Localization.Configuration;
using Microsoft.Extensions.Options;

namespace Datezy.IntegrationTests.Localization;

public sealed class LanguageResolverTests
{
    private readonly ILanguageCatalog _catalog;
    private readonly ILanguageResolver _resolver;

    public LanguageResolverTests()
    {
        var options = Options.Create(
            CreateLocalizationOptions());

        _catalog = new LanguageCatalog(options);

        _resolver = new LanguageResolver(
            _catalog,
            options);
    }

    [Theory]
    [InlineData("az-AZ", "az")]
    [InlineData("tr-TR", "tr")]
    [InlineData("ru-RU", "ru")]
    [InlineData("en-US", "en")]
    [InlineData("en-GB", "en")]
    [InlineData("pt-BR", "pt")]
    [InlineData("uk-UA", "uk")]
    public void Resolve_ShouldResolveTelegramLanguage(
        string telegramLanguageCode,
        string expectedLanguageCode)
    {
        var result = _resolver.Resolve(
            preferredLanguageCode: null,
            telegramLanguageCode);

        Assert.Equal(
            expectedLanguageCode,
            result);
    }

    [Fact]
    public void Resolve_ShouldPreferExplicitUserLanguage()
    {
        var result = _resolver.Resolve(
            preferredLanguageCode: "az",
            telegramLanguageCode: "ru-RU");

        Assert.Equal(
            "az",
            result);
    }

    [Fact]
    public void Resolve_ShouldFallbackToDefaultLanguage()
    {
        var result = _resolver.Resolve(
            preferredLanguageCode: null,
            telegramLanguageCode: "ja-JP");

        Assert.Equal(
            "en",
            result);
    }

    [Fact]
    public void Catalog_ShouldContainFifteenEnabledLanguages()
    {
        var languages =
            _catalog.GetEnabledLanguages();

        Assert.Equal(
            15,
            languages.Count);
    }

    private static LocalizationOptions
        CreateLocalizationOptions()
    {
        return new LocalizationOptions
        {
            DefaultLanguage = "en",

            Languages =
            [
                Create("az", "Azərbaycanca", "🇦🇿", 10, "az", "az-AZ"),
                Create("tr", "Türkçe", "🇹🇷", 20, "tr", "tr-TR"),
                Create("ru", "Русский", "🇷🇺", 30, "ru", "ru-RU"),
                Create("en", "English", "🇬🇧", 40, "en", "en-US", "en-GB"),
                Create("uk", "Українська", "🇺🇦", 50, "uk", "uk-UA"),
                Create("kk", "Қазақша", "🇰🇿", 60, "kk", "kk-KZ"),
                Create("uz", "O‘zbekcha", "🇺🇿", 70, "uz", "uz-UZ"),
                Create("ka", "ქართული", "🇬🇪", 80, "ka", "ka-GE"),
                Create("hy", "Հայերեն", "🇦🇲", 90, "hy", "hy-AM"),
                Create("pl", "Polski", "🇵🇱", 100, "pl", "pl-PL"),
                Create("de", "Deutsch", "🇩🇪", 110, "de", "de-DE"),
                Create("es", "Español", "🇪🇸", 120, "es", "es-ES"),
                Create("fr", "Français", "🇫🇷", 130, "fr", "fr-FR"),
                Create("it", "Italiano", "🇮🇹", 140, "it", "it-IT"),
                Create("pt", "Português", "🇵🇹", 150, "pt", "pt-PT", "pt-BR")
            ]
        };
    }

    private static LanguageOptions Create(
        string code,
        string nativeName,
        string flag,
        int sortOrder,
        params string[] telegramLanguageCodes)
    {
        return new LanguageOptions
        {
            Code = code,
            NativeName = nativeName,
            Flag = flag,
            SortOrder = sortOrder,
            Enabled = true,
            TelegramLanguageCodes =
                telegramLanguageCodes
        };
    }
}