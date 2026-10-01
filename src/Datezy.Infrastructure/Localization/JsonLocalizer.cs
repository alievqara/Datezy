using System.Globalization;
using System.Text.Json;
using Datezy.Application.Localization;
using Datezy.Infrastructure.Localization.Configuration;
using Microsoft.Extensions.Options;

namespace Datezy.Infrastructure.Localization;

public sealed class JsonLocalizer : ILocalizer
{
    private readonly string _defaultLanguage;
    private readonly IReadOnlyDictionary<
        string,
        IReadOnlyDictionary<string, string>> _resources;

    public JsonLocalizer(
        IOptions<LocalizationOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _defaultLanguage = NormalizeLanguageCode(
            options.Value.DefaultLanguage);

        _resources = LoadResources();

        if (!_resources.ContainsKey(_defaultLanguage))
        {
            throw new InvalidOperationException(
                $"Default localization resource " +
                $"'{_defaultLanguage}.json' was not found.");
        }
    }

    public string Get(
        string key,
        string languageCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        var normalizedLanguageCode =
            NormalizeLanguageCode(languageCode);

        if (TryGetValue(
                normalizedLanguageCode,
                key,
                out var localizedValue))
        {
            return localizedValue;
        }

        if (TryGetValue(
                _defaultLanguage,
                key,
                out var fallbackValue))
        {
            return fallbackValue;
        }

        throw new KeyNotFoundException(
            $"Localization key '{key}' was not found " +
            $"for language '{normalizedLanguageCode}' " +
            $"or fallback language '{_defaultLanguage}'.");
    }

    public string Get(
        string key,
        string languageCode,
        params object[] arguments)
    {
        var format = Get(
            key,
            languageCode);

        return string.Format(
            CultureInfo.InvariantCulture,
            format,
            arguments);
    }

    private bool TryGetValue(
        string languageCode,
        string key,
        out string value)
    {
        value = string.Empty;

        if (!_resources.TryGetValue(
                languageCode,
                out var languageResources))
        {
            return false;
        }

        if (!languageResources.TryGetValue(
                key,
                out var localizedValue) ||
            localizedValue is null)
        {
            return false;
        }

        value = localizedValue;

        return true;
    }

    private static IReadOnlyDictionary<
        string,
        IReadOnlyDictionary<string, string>> LoadResources()
    {
        var assembly = typeof(JsonLocalizer).Assembly;

        var resources = new Dictionary<
            string,
            IReadOnlyDictionary<string, string>>(
                StringComparer.OrdinalIgnoreCase);

        var resourceNames = assembly
            .GetManifestResourceNames()
            .Where(name =>
                name.Contains(
                    ".Localization.Resources.",
                    StringComparison.Ordinal)
                &&
                name.EndsWith(
                    ".json",
                    StringComparison.OrdinalIgnoreCase));

        foreach (var resourceName in resourceNames)
        {
            var languageCode =
                ExtractLanguageCode(resourceName);

            using var stream =
                assembly.GetManifestResourceStream(resourceName)
                ?? throw new InvalidOperationException(
                    $"Embedded localization resource " +
                    $"'{resourceName}' could not be opened.");

            using var document =
                JsonDocument.Parse(stream);

            var flattened =
                new Dictionary<string, string>(
                    StringComparer.OrdinalIgnoreCase);

            Flatten(
                document.RootElement,
                prefix: null,
                flattened);

            resources[languageCode] = flattened;
        }

        return resources;
    }

    private static void Flatten(
        JsonElement element,
        string? prefix,
        IDictionary<string, string> destination)
    {
        foreach (var property in element.EnumerateObject())
        {
            var key = string.IsNullOrWhiteSpace(prefix)
                ? property.Name
                : $"{prefix}.{property.Name}";

            if (property.Value.ValueKind ==
                JsonValueKind.Object)
            {
                Flatten(
                    property.Value,
                    key,
                    destination);

                continue;
            }

            if (property.Value.ValueKind !=
                JsonValueKind.String)
            {
                throw new InvalidOperationException(
                    $"Localization value '{key}' must be a string.");
            }

            destination[key] =
                property.Value.GetString()
                ?? string.Empty;
        }
    }

    private static string ExtractLanguageCode(
        string resourceName)
    {
        const string marker =
            ".Localization.Resources.";

        var markerIndex =
            resourceName.IndexOf(
                marker,
                StringComparison.Ordinal);

        if (markerIndex < 0)
        {
            throw new InvalidOperationException(
                $"Invalid localization resource name " +
                $"'{resourceName}'.");
        }

        var start =
            markerIndex + marker.Length;

        return resourceName[start..^".json".Length]
            .ToLowerInvariant();
    }

    private static string NormalizeLanguageCode(
        string languageCode)
    {
        if (string.IsNullOrWhiteSpace(languageCode))
        {
            throw new ArgumentException(
                "Language code cannot be empty.",
                nameof(languageCode));
        }

        return languageCode
            .Trim()
            .ToLowerInvariant();
    }
}