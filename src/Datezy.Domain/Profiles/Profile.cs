using Datezy.Domain.Common;

namespace Datezy.Domain.Profiles;

public sealed class Profile : AggregateRoot
{
    private readonly List<ProfilePhoto> _photos = [];

    public Guid UserId { get; private set; }

    public string DisplayName { get; private set; } = null!;

    public DateOnly BirthDate { get; private set; }

    public Gender Gender { get; private set; }

    public GenderPreference LookingFor { get; private set; }

    public string CountryCode { get; private set; } = null!;

    public string City { get; private set; } = null!;

    public string? Bio { get; private set; }

    public ProfileVisibility Visibility { get; private set; }

    public IReadOnlyCollection<ProfilePhoto> Photos =>
        _photos.AsReadOnly();

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime UpdatedAtUtc { get; private set; }

    private Profile()
    {
    }

    private Profile(
        Guid id,
        Guid userId,
        string displayName,
        DateOnly birthDate,
        Gender gender,
        GenderPreference lookingFor,
        string countryCode,
        string city,
        string? bio,
        DateOnly today,
        DateTime utcNow)
        : base(id)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException(
                "User identifier cannot be empty.",
                nameof(userId));
        }

        ProfileRules.EnsureValidBirthDate(
            birthDate,
            today);

        ProfileRules.EnsureValidGender(
            gender);

        ProfileRules.EnsureValidGenderPreference(
            lookingFor);

        UserId = userId;

        DisplayName =
            ProfileRules.NormalizeDisplayName(
                displayName);

        BirthDate = birthDate;
        Gender = gender;
        LookingFor = lookingFor;

        CountryCode =
            ProfileRules.NormalizeCountryCode(
                countryCode);

        City =
            ProfileRules.NormalizeCity(
                city);

        Bio =
            ProfileRules.NormalizeBio(
                bio);

        Visibility = ProfileVisibility.Hidden;

        CreatedAtUtc = utcNow;
        UpdatedAtUtc = utcNow;
    }

    public static Profile Create(
        Guid userId,
        string displayName,
        DateOnly birthDate,
        Gender gender,
        GenderPreference lookingFor,
        string countryCode,
        string city,
        string? bio,
        DateOnly today,
        DateTime utcNow)
    {
        return new Profile(
            Guid.NewGuid(),
            userId,
            displayName,
            birthDate,
            gender,
            lookingFor,
            countryCode,
            city,
            bio,
            today,
            utcNow);
    }

    public void UpdateDisplayName(
        string displayName,
        DateTime utcNow)
    {
        DisplayName =
            ProfileRules.NormalizeDisplayName(
                displayName);

        Touch(utcNow);
    }

    public void UpdateBirthDate(
        DateOnly birthDate,
        DateOnly today,
        DateTime utcNow)
    {
        ProfileRules.EnsureValidBirthDate(
            birthDate,
            today);

        BirthDate = birthDate;

        Touch(utcNow);
    }

    public void UpdateGender(
        Gender gender,
        DateTime utcNow)
    {
        ProfileRules.EnsureValidGender(
            gender);

        Gender = gender;

        Touch(utcNow);
    }

    public void UpdateLookingFor(
        GenderPreference lookingFor,
        DateTime utcNow)
    {
        ProfileRules.EnsureValidGenderPreference(
            lookingFor);

        LookingFor = lookingFor;

        Touch(utcNow);
    }

    public void UpdateLocation(
        string countryCode,
        string city,
        DateTime utcNow)
    {
        CountryCode =
            ProfileRules.NormalizeCountryCode(
                countryCode);

        City =
            ProfileRules.NormalizeCity(
                city);

        Touch(utcNow);
    }

    public void UpdateBio(
        string? bio,
        DateTime utcNow)
    {
        Bio =
            ProfileRules.NormalizeBio(
                bio);

        Touch(utcNow);
    }

    public ProfilePhoto AddPhoto(
        string telegramFileId,
        string telegramFileUniqueId,
        DateTime utcNow)
    {
        if (_photos.Count >=
            ProfileRules.MaximumPhotoCount)
        {
            throw new InvalidOperationException(
                $"A profile can contain at most " +
                $"{ProfileRules.MaximumPhotoCount} photos.");
        }

        if (_photos.Any(
                photo =>
                    string.Equals(
                        photo.TelegramFileUniqueId,
                        telegramFileUniqueId?.Trim(),
                        StringComparison.Ordinal)))
        {
            throw new InvalidOperationException(
                "This photo has already been added.");
        }

        var photo =
            ProfilePhoto.Create(
                Id,
                telegramFileId,
                telegramFileUniqueId,
                _photos.Count + 1,
                utcNow);

        _photos.Add(photo);

        Touch(utcNow);

        return photo;
    }

    public void RemovePhoto(
        Guid photoId,
        DateTime utcNow)
    {
        var photo =
            _photos.FirstOrDefault(
                item => item.Id == photoId)
            ?? throw new InvalidOperationException(
                "Profile photo was not found.");

        _photos.Remove(photo);

        NormalizePhotoOrder();

        if (_photos.Count <
            ProfileRules.MinimumPhotoCount)
        {
            Visibility = ProfileVisibility.Hidden;
        }

        Touch(utcNow);
    }

    public void SetPrimaryPhoto(
        Guid photoId,
        DateTime utcNow)
    {
        var selectedPhoto =
            _photos.FirstOrDefault(
                photo => photo.Id == photoId)
            ?? throw new InvalidOperationException(
                "Profile photo was not found.");

        var ordered =
            _photos
                .OrderBy(photo =>
                    photo.Id == selectedPhoto.Id
                        ? 0
                        : photo.SortOrder)
                .ToArray();

        for (var index = 0;
             index < ordered.Length;
             index++)
        {
            ordered[index]
                .ChangeSortOrder(index + 1);
        }

        SortPhotos();

        Touch(utcNow);
    }

    public void Show(
        DateTime utcNow)
    {
        if (_photos.Count <
            ProfileRules.MinimumPhotoCount)
        {
            throw new InvalidOperationException(
                "Profile must contain at least one photo before it can be visible.");
        }

        if (Visibility ==
            ProfileVisibility.UnderReview)
        {
            throw new InvalidOperationException(
                "A profile under review cannot be made visible.");
        }

        Visibility = ProfileVisibility.Visible;

        Touch(utcNow);
    }

    public void Hide(
        DateTime utcNow)
    {
        Visibility = ProfileVisibility.Hidden;

        Touch(utcNow);
    }

    public void Pause(
        DateTime utcNow)
    {
        if (Visibility ==
            ProfileVisibility.UnderReview)
        {
            throw new InvalidOperationException(
                "A profile under review cannot be paused.");
        }

        Visibility = ProfileVisibility.Paused;

        Touch(utcNow);
    }

    public void PlaceUnderReview(
        DateTime utcNow)
    {
        Visibility =
            ProfileVisibility.UnderReview;

        Touch(utcNow);
    }

    public void ReleaseFromReview(
        DateTime utcNow)
    {
        if (Visibility !=
            ProfileVisibility.UnderReview)
        {
            throw new InvalidOperationException(
                "Profile is not under review.");
        }

        Visibility =
            ProfileVisibility.Hidden;

        Touch(utcNow);
    }

    private void NormalizePhotoOrder()
    {
        var ordered =
            _photos
                .OrderBy(photo => photo.SortOrder)
                .ToArray();

        for (var index = 0;
             index < ordered.Length;
             index++)
        {
            ordered[index]
                .ChangeSortOrder(index + 1);
        }

        SortPhotos();
    }

    private void SortPhotos()
    {
        _photos.Sort(
            static (left, right) =>
                left.SortOrder.CompareTo(
                    right.SortOrder));
    }

    private void Touch(
        DateTime utcNow)
    {
        UpdatedAtUtc = utcNow;
    }
}