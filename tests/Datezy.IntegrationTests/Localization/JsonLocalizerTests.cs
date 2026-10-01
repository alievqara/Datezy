using Datezy.Application.Localization;
using Datezy.Infrastructure.Localization;
using Datezy.Infrastructure.Localization.Configuration;
using Microsoft.Extensions.Options;

namespace Datezy.IntegrationTests.Localization;

public sealed class JsonLocalizerTests
{
    private readonly ILocalizer _localizer;

    public JsonLocalizerTests()
    {
        var options =
            Options.Create(
                new LocalizationOptions
                {
                    DefaultLanguage = "en"
                });

        _localizer =
            new JsonLocalizer(options);
    }

    [Theory]
    [InlineData("en", "🌍 Choose your language")]
    [InlineData("az", "🌍 Dilinizi seçin")]
    [InlineData("tr", "🌍 Dilinizi seçin")]
    [InlineData("ru", "🌍 Выберите язык")]
    public void Get_ShouldReturnLocalizedValue(
        string languageCode,
        string expected)
    {
        var result = _localizer.Get(
            LocalizationKeys
                .Registration
                .Language
                .Title,
            languageCode);

        Assert.Equal(
            expected,
            result);
    }

    [Fact]
    public void Get_ShouldFormatArguments()
    {
        var result = _localizer.Get(
            LocalizationKeys
                .Registration
                .Language
                .Changed,
            "en",
            "English");

        Assert.Equal(
            "✅ Language changed to English.",
            result);
    }

    [Fact]
    public void Get_ShouldFallbackToDefaultLanguage_WhenLanguageDoesNotExist()
    {
        var result = _localizer.Get(
            LocalizationKeys
                .Registration
                .Language
                .Title,
            "ja");

        Assert.Equal(
            "🌍 Choose your language",
            result);
    }

    [Fact]
    public void Get_ShouldThrow_WhenKeyDoesNotExist()
    {
        Assert.Throws<KeyNotFoundException>(
            () => _localizer.Get(
                "This.Key.Does.Not.Exist",
                "en"));
    }
}