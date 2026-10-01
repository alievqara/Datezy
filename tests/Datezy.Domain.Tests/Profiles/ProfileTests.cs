using Datezy.Domain.Profiles;

namespace Datezy.Domain.Tests.Profiles;

public sealed class ProfileTests
{
    private static readonly Guid UserId =
        Guid.Parse("22222222-2222-2222-2222-222222222222");

    private static readonly DateTime UtcNow =
        new(2026, 10, 1, 12, 0, 0, DateTimeKind.Utc);

    private static readonly DateOnly Today =
        new(2026, 10, 1);

    [Fact]
    public void Create_ShouldCreateHiddenProfile()
    {
        var profile = CreateProfile();

        Assert.NotEqual(Guid.Empty, profile.Id);
        Assert.Equal(UserId, profile.UserId);
        Assert.Equal("Barlas", profile.DisplayName);
        Assert.Equal(new DateOnly(2000, 1, 1), profile.BirthDate);
        Assert.Equal(Gender.Male, profile.Gender);
        Assert.Equal(GenderPreference.Female, profile.LookingFor);
        Assert.Equal("AZ", profile.CountryCode);
        Assert.Equal("Baku", profile.City);
        Assert.Equal("Hello", profile.Bio);
        Assert.Equal(ProfileVisibility.Hidden, profile.Visibility);
        Assert.Empty(profile.Photos);
        Assert.Equal(UtcNow, profile.CreatedAtUtc);
        Assert.Equal(UtcNow, profile.UpdatedAtUtc);
    }

    [Fact]
    public void Create_ShouldNormalizeValues()
    {
        var profile = Profile.Create(
            UserId,
            "  Barlas  ",
            new DateOnly(2000, 1, 1),
            Gender.Male,
            GenderPreference.Female,
            " az ",
            "  Baku  ",
            "  Hello world  ",
            Today,
            UtcNow);

        Assert.Equal("Barlas", profile.DisplayName);
        Assert.Equal("AZ", profile.CountryCode);
        Assert.Equal("Baku", profile.City);
        Assert.Equal("Hello world", profile.Bio);
    }

    [Fact]
    public void Create_ShouldRejectEmptyUserId()
    {
        Assert.Throws<ArgumentException>(
            () => Profile.Create(
                Guid.Empty,
                "Barlas",
                new DateOnly(2000, 1, 1),
                Gender.Male,
                GenderPreference.Female,
                "AZ",
                "Baku",
                null,
                Today,
                UtcNow));
    }

    [Fact]
    public void UpdateDisplayName_ShouldChangeName()
    {
        var profile = CreateProfile();
        var changedAt = UtcNow.AddMinutes(1);

        profile.UpdateDisplayName(
            "New Name",
            changedAt);

        Assert.Equal("New Name", profile.DisplayName);
        Assert.Equal(changedAt, profile.UpdatedAtUtc);
    }

    [Fact]
    public void UpdateBirthDate_ShouldRejectUnderageUser()
    {
        var profile = CreateProfile();

        Assert.Throws<InvalidOperationException>(
            () => profile.UpdateBirthDate(
                new DateOnly(2008, 10, 2),
                Today,
                UtcNow.AddMinutes(1)));
    }

    [Fact]
    public void UpdateGender_ShouldChangeGender()
    {
        var profile = CreateProfile();

        profile.UpdateGender(
            Gender.Female,
            UtcNow.AddMinutes(1));

        Assert.Equal(
            Gender.Female,
            profile.Gender);
    }

    [Fact]
    public void UpdateLookingFor_ShouldSupportMultiplePreferences()
    {
        var profile = CreateProfile();

        var preference =
            GenderPreference.Male |
            GenderPreference.Female;

        profile.UpdateLookingFor(
            preference,
            UtcNow.AddMinutes(1));

        Assert.Equal(
            preference,
            profile.LookingFor);
    }

    [Fact]
    public void UpdateLocation_ShouldNormalizeValues()
    {
        var profile = CreateProfile();

        profile.UpdateLocation(
            " tr ",
            "  Istanbul  ",
            UtcNow.AddMinutes(1));

        Assert.Equal("TR", profile.CountryCode);
        Assert.Equal("Istanbul", profile.City);
    }

    [Fact]
    public void UpdateBio_ShouldAllowRemovingBio()
    {
        var profile = CreateProfile();

        profile.UpdateBio(
            "   ",
            UtcNow.AddMinutes(1));

        Assert.Null(profile.Bio);
    }

    [Fact]
    public void AddPhoto_ShouldAddFirstPhoto()
    {
        var profile = CreateProfile();

        var photo = profile.AddPhoto(
            "telegram-file-1",
            "unique-file-1",
            UtcNow.AddMinutes(1));

        Assert.Single(profile.Photos);
        Assert.Equal(profile.Id, photo.ProfileId);
        Assert.Equal("telegram-file-1", photo.TelegramFileId);
        Assert.Equal("unique-file-1", photo.TelegramFileUniqueId);
        Assert.Equal(1, photo.SortOrder);
    }

    [Fact]
    public void AddPhoto_ShouldAssignSequentialSortOrder()
    {
        var profile = CreateProfile();

        var first = AddPhoto(
            profile,
            1);

        var second = AddPhoto(
            profile,
            2);

        var third = AddPhoto(
            profile,
            3);

        Assert.Equal(1, first.SortOrder);
        Assert.Equal(2, second.SortOrder);
        Assert.Equal(3, third.SortOrder);
    }

    [Fact]
    public void AddPhoto_ShouldRejectMoreThanThreePhotos()
    {
        var profile = CreateProfile();

        AddPhoto(profile, 1);
        AddPhoto(profile, 2);
        AddPhoto(profile, 3);

        Assert.Throws<InvalidOperationException>(
            () => AddPhoto(profile, 4));

        Assert.Equal(
            ProfileRules.MaximumPhotoCount,
            profile.Photos.Count);
    }

    [Fact]
    public void AddPhoto_ShouldRejectDuplicateUniqueFileId()
    {
        var profile = CreateProfile();

        profile.AddPhoto(
            "telegram-file-1",
            "same-unique-id",
            UtcNow.AddMinutes(1));

        Assert.Throws<InvalidOperationException>(
            () => profile.AddPhoto(
                "telegram-file-2",
                "same-unique-id",
                UtcNow.AddMinutes(2)));

        Assert.Single(profile.Photos);
    }

    [Fact]
    public void RemovePhoto_ShouldNormalizeRemainingSortOrder()
    {
        var profile = CreateProfile();

        var first = AddPhoto(profile, 1);
        var second = AddPhoto(profile, 2);
        var third = AddPhoto(profile, 3);

        profile.RemovePhoto(
            second.Id,
            UtcNow.AddMinutes(4));

        Assert.Equal(2, profile.Photos.Count);

        var photos = profile.Photos
            .OrderBy(photo => photo.SortOrder)
            .ToArray();

        Assert.Equal(first.Id, photos[0].Id);
        Assert.Equal(1, photos[0].SortOrder);

        Assert.Equal(third.Id, photos[1].Id);
        Assert.Equal(2, photos[1].SortOrder);
    }

    [Fact]
    public void RemovePhoto_ShouldHideProfile_WhenLastPhotoIsRemoved()
    {
        var profile = CreateProfile();

        var photo = AddPhoto(profile, 1);

        profile.Show(
            UtcNow.AddMinutes(2));

        Assert.Equal(
            ProfileVisibility.Visible,
            profile.Visibility);

        profile.RemovePhoto(
            photo.Id,
            UtcNow.AddMinutes(3));

        Assert.Empty(profile.Photos);

        Assert.Equal(
            ProfileVisibility.Hidden,
            profile.Visibility);
    }

    [Fact]
    public void RemovePhoto_ShouldRejectUnknownPhoto()
    {
        var profile = CreateProfile();

        Assert.Throws<InvalidOperationException>(
            () => profile.RemovePhoto(
                Guid.NewGuid(),
                UtcNow));
    }

    [Fact]
    public void SetPrimaryPhoto_ShouldMoveSelectedPhotoToFirstPosition()
    {
        var profile = CreateProfile();

        var first = AddPhoto(profile, 1);
        var second = AddPhoto(profile, 2);
        var third = AddPhoto(profile, 3);

        profile.SetPrimaryPhoto(
            third.Id,
            UtcNow.AddMinutes(4));

        var photos = profile.Photos
            .OrderBy(photo => photo.SortOrder)
            .ToArray();

        Assert.Equal(third.Id, photos[0].Id);
        Assert.Equal(1, photos[0].SortOrder);

        Assert.Equal(first.Id, photos[1].Id);
        Assert.Equal(2, photos[1].SortOrder);

        Assert.Equal(second.Id, photos[2].Id);
        Assert.Equal(3, photos[2].SortOrder);
    }

    [Fact]
    public void Show_ShouldRejectProfileWithoutPhoto()
    {
        var profile = CreateProfile();

        Assert.Throws<InvalidOperationException>(
            () => profile.Show(
                UtcNow.AddMinutes(1)));

        Assert.Equal(
            ProfileVisibility.Hidden,
            profile.Visibility);
    }

    [Fact]
    public void Show_ShouldMakeProfileVisible_WhenPhotoExists()
    {
        var profile = CreateProfile();

        AddPhoto(profile, 1);

        profile.Show(
            UtcNow.AddMinutes(2));

        Assert.Equal(
            ProfileVisibility.Visible,
            profile.Visibility);
    }

    [Fact]
    public void Hide_ShouldHideVisibleProfile()
    {
        var profile = CreateVisibleProfile();

        profile.Hide(
            UtcNow.AddMinutes(3));

        Assert.Equal(
            ProfileVisibility.Hidden,
            profile.Visibility);
    }

    [Fact]
    public void Pause_ShouldPauseVisibleProfile()
    {
        var profile = CreateVisibleProfile();

        profile.Pause(
            UtcNow.AddMinutes(3));

        Assert.Equal(
            ProfileVisibility.Paused,
            profile.Visibility);
    }

    [Fact]
    public void PlaceUnderReview_ShouldPreventUserFromShowingProfile()
    {
        var profile = CreateProfile();

        AddPhoto(profile, 1);

        profile.PlaceUnderReview(
            UtcNow.AddMinutes(2));

        Assert.Equal(
            ProfileVisibility.UnderReview,
            profile.Visibility);

        Assert.Throws<InvalidOperationException>(
            () => profile.Show(
                UtcNow.AddMinutes(3)));
    }

    [Fact]
    public void Pause_ShouldRejectProfileUnderReview()
    {
        var profile = CreateProfile();

        profile.PlaceUnderReview(
            UtcNow.AddMinutes(1));

        Assert.Throws<InvalidOperationException>(
            () => profile.Pause(
                UtcNow.AddMinutes(2)));
    }

    [Fact]
    public void ReleaseFromReview_ShouldReturnProfileToHidden()
    {
        var profile = CreateProfile();

        profile.PlaceUnderReview(
            UtcNow.AddMinutes(1));

        profile.ReleaseFromReview(
            UtcNow.AddMinutes(2));

        Assert.Equal(
            ProfileVisibility.Hidden,
            profile.Visibility);
    }

    [Fact]
    public void ReleaseFromReview_ShouldRejectProfileNotUnderReview()
    {
        var profile = CreateProfile();

        Assert.Throws<InvalidOperationException>(
            () => profile.ReleaseFromReview(
                UtcNow.AddMinutes(1)));
    }

    private static Profile CreateProfile()
    {
        return Profile.Create(
            UserId,
            "Barlas",
            new DateOnly(2000, 1, 1),
            Gender.Male,
            GenderPreference.Female,
            "AZ",
            "Baku",
            "Hello",
            Today,
            UtcNow);
    }

    private static Profile CreateVisibleProfile()
    {
        var profile = CreateProfile();

        AddPhoto(profile, 1);

        profile.Show(
            UtcNow.AddMinutes(2));

        return profile;
    }

    private static ProfilePhoto AddPhoto(
        Profile profile,
        int number)
    {
        return profile.AddPhoto(
            $"telegram-file-{number}",
            $"unique-file-{number}",
            UtcNow.AddMinutes(number));
    }
}