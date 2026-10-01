namespace Datezy.Domain.Profiles;

public static class ProfileRules
{
    public const int MinimumAge = 18;
    public const int MaximumAge = 120;

    public const int MinimumDisplayNameLength = 2;
    public const int MaximumDisplayNameLength = 50;

    public const int MaximumCityLength = 100;
    public const int MaximumBioLength = 500;

    public const int MinimumPhotoCount = 1;
    public const int MaximumPhotoCount = 3;

    public static string NormalizeDisplayName(
        string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException(
                "Display name is required.",
                nameof(displayName));
        }

        var normalized = displayName.Trim();

        if (normalized.Length is
            < MinimumDisplayNameLength
            or > MaximumDisplayNameLength)
        {
            throw new ArgumentOutOfRangeException(
                nameof(displayName),
                $"Display name must contain between " +
                $"{MinimumDisplayNameLength} and " +
                $"{MaximumDisplayNameLength} characters.");
        }

        return normalized;
    }

    public static void EnsureValidBirthDate(
        DateOnly birthDate,
        DateOnly today)
    {
        if (birthDate > today)
        {
            throw new ArgumentOutOfRangeException(
                nameof(birthDate),
                "Birth date cannot be in the future.");
        }

        var age = CalculateAge(
            birthDate,
            today);

        if (age < MinimumAge)
        {
            throw new InvalidOperationException(
                $"User must be at least {MinimumAge} years old.");
        }

        if (age > MaximumAge)
        {
            throw new InvalidOperationException(
                "Birth date is outside the supported range.");
        }
    }

    public static string NormalizeCountryCode(
        string countryCode)
    {
        if (string.IsNullOrWhiteSpace(countryCode))
        {
            throw new ArgumentException(
                "Country code is required.",
                nameof(countryCode));
        }

        var normalized =
            countryCode.Trim().ToUpperInvariant();

        if (normalized.Length != 2 ||
            !normalized.All(char.IsLetter))
        {
            throw new ArgumentException(
                "Country code must be an ISO 3166-1 alpha-2 code.",
                nameof(countryCode));
        }

        return normalized;
    }

    public static string NormalizeCity(
        string city)
    {
        if (string.IsNullOrWhiteSpace(city))
        {
            throw new ArgumentException(
                "City is required.",
                nameof(city));
        }

        var normalized = city.Trim();

        if (normalized.Length > MaximumCityLength)
        {
            throw new ArgumentOutOfRangeException(
                nameof(city),
                $"City cannot exceed " +
                $"{MaximumCityLength} characters.");
        }

        return normalized;
    }

    public static string? NormalizeBio(
        string? bio)
    {
        if (string.IsNullOrWhiteSpace(bio))
        {
            return null;
        }

        var normalized = bio.Trim();

        if (normalized.Length > MaximumBioLength)
        {
            throw new ArgumentOutOfRangeException(
                nameof(bio),
                $"Bio cannot exceed " +
                $"{MaximumBioLength} characters.");
        }

        return normalized;
    }

    public static void EnsureValidGender(
        Gender gender)
    {
        if (!Enum.IsDefined(gender))
        {
            throw new ArgumentOutOfRangeException(
                nameof(gender));
        }
    }

    public static void EnsureValidGenderPreference(
        GenderPreference preference)
    {
        if (preference == GenderPreference.None)
        {
            throw new ArgumentException(
                "At least one gender preference must be selected.",
                nameof(preference));
        }

        const GenderPreference supported =
            GenderPreference.Male |
            GenderPreference.Female |
            GenderPreference.Other;

        if ((preference & ~supported) != 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(preference));
        }
    }

    public static int CalculateAge(
        DateOnly birthDate,
        DateOnly today)
    {
        var age =
            today.Year - birthDate.Year;

        if (birthDate > today.AddYears(-age))
        {
            age--;
        }

        return age;
    }
}