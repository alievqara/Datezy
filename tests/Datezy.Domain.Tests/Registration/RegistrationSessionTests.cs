using Datezy.Domain.Profiles;
using Datezy.Domain.Registration;

namespace Datezy.Domain.Tests.Registration;

public sealed class RegistrationSessionTests
{
    private static readonly Guid UserId =
        Guid.Parse("11111111-1111-1111-1111-111111111111");

    private static readonly DateTime UtcNow =
        new(2026, 10, 1, 12, 0, 0, DateTimeKind.Utc);

    private static readonly DateOnly Today =
        new(2026, 10, 1);

    [Fact]
    public void Start_ShouldCreateSessionAtAgeConfirmation()
    {
        var session = RegistrationSession.Start(
            UserId,
            UtcNow);

        Assert.NotEqual(Guid.Empty, session.Id);
        Assert.Equal(UserId, session.UserId);

        Assert.Equal(
            RegistrationStep.AgeConfirmation,
            session.CurrentStep);

        Assert.False(session.IsAdultConfirmed);
        Assert.Equal(UtcNow, session.CreatedAtUtc);
        Assert.Equal(UtcNow, session.UpdatedAtUtc);
        Assert.Null(session.CompletedAtUtc);
    }

    [Fact]
    public void Start_ShouldRejectEmptyUserId()
    {
        Assert.Throws<ArgumentException>(
            () => RegistrationSession.Start(
                Guid.Empty,
                UtcNow));
    }

    [Fact]
    public void ConfirmAdult_ShouldMoveToDisplayName()
    {
        var session = CreateSession();

        var changedAt =
            UtcNow.AddMinutes(1);

        session.ConfirmAdult(changedAt);

        Assert.True(session.IsAdultConfirmed);

        Assert.Equal(
            RegistrationStep.DisplayName,
            session.CurrentStep);

        Assert.Equal(
            changedAt,
            session.UpdatedAtUtc);
    }

    [Fact]
    public void SetDisplayName_ShouldNormalizeValue()
    {
        var session =
            CreateAtDisplayNameStep();

        session.SetDisplayName(
            "   Barlas   ",
            UtcNow.AddMinutes(2));

        Assert.Equal(
            "Barlas",
            session.DisplayName);

        Assert.Equal(
            RegistrationStep.BirthDate,
            session.CurrentStep);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("A")]
    public void SetDisplayName_ShouldRejectInvalidValue(
        string displayName)
    {
        var session =
            CreateAtDisplayNameStep();

        Assert.ThrowsAny<ArgumentException>(
            () => session.SetDisplayName(
                displayName,
                UtcNow));
    }

    [Fact]
    public void SetDisplayName_ShouldRejectNameLongerThanFiftyCharacters()
    {
        var session =
            CreateAtDisplayNameStep();

        var displayName =
            new string('A', 51);

        Assert.Throws<ArgumentOutOfRangeException>(
            () => session.SetDisplayName(
                displayName,
                UtcNow));
    }

    [Fact]
    public void SetBirthDate_ShouldAcceptUserWhoTurnsEighteenToday()
    {
        var session =
            CreateAtBirthDateStep();

        var birthDate =
            new DateOnly(2008, 10, 1);

        session.SetBirthDate(
            birthDate,
            Today,
            UtcNow);

        Assert.Equal(
            birthDate,
            session.BirthDate);

        Assert.Equal(
            RegistrationStep.Gender,
            session.CurrentStep);
    }

    [Fact]
    public void SetBirthDate_ShouldRejectUserWhoTurnsEighteenTomorrow()
    {
        var session =
            CreateAtBirthDateStep();

        var birthDate =
            new DateOnly(2008, 10, 2);

        Assert.Throws<InvalidOperationException>(
            () => session.SetBirthDate(
                birthDate,
                Today,
                UtcNow));
    }

    [Fact]
    public void SetBirthDate_ShouldRejectFutureDate()
    {
        var session =
            CreateAtBirthDateStep();

        Assert.Throws<ArgumentOutOfRangeException>(
            () => session.SetBirthDate(
                Today.AddDays(1),
                Today,
                UtcNow));
    }

    [Fact]
    public void SetBirthDate_ShouldRejectAgeGreaterThanOneHundredTwenty()
    {
        var session =
            CreateAtBirthDateStep();

        var birthDate =
            new DateOnly(1905, 9, 30);

        Assert.Throws<InvalidOperationException>(
            () => session.SetBirthDate(
                birthDate,
                Today,
                UtcNow));
    }

    [Fact]
    public void SetGender_ShouldMoveToLookingFor()
    {
        var session =
            CreateAtGenderStep();

        session.SetGender(
            Gender.Female,
            UtcNow);

        Assert.Equal(
            Gender.Female,
            session.Gender);

        Assert.Equal(
            RegistrationStep.LookingFor,
            session.CurrentStep);
    }

    [Fact]
    public void SetGender_ShouldRejectUnknownGender()
    {
        var session =
            CreateAtGenderStep();

        var invalidGender =
            (Gender)999;

        Assert.Throws<ArgumentOutOfRangeException>(
            () => session.SetGender(
                invalidGender,
                UtcNow));
    }

    [Fact]
    public void SetLookingFor_ShouldAcceptMultiplePreferences()
    {
        var session =
            CreateAtLookingForStep();

        var preference =
            GenderPreference.Male |
            GenderPreference.Female;

        session.SetLookingFor(
            preference,
            UtcNow);

        Assert.Equal(
            preference,
            session.LookingFor);

        Assert.Equal(
            RegistrationStep.Location,
            session.CurrentStep);
    }

    [Fact]
    public void SetLookingFor_ShouldRejectNone()
    {
        var session =
            CreateAtLookingForStep();

        Assert.Throws<ArgumentException>(
            () => session.SetLookingFor(
                GenderPreference.None,
                UtcNow));
    }

    [Fact]
    public void SetLookingFor_ShouldRejectUnsupportedFlags()
    {
        var session =
            CreateAtLookingForStep();

        var invalid =
            (GenderPreference)8;

        Assert.Throws<ArgumentOutOfRangeException>(
            () => session.SetLookingFor(
                invalid,
                UtcNow));
    }

    [Fact]
    public void SetLocation_ShouldNormalizeCountryAndCity()
    {
        var session =
            CreateAtLocationStep();

        session.SetLocation(
            " az ",
            "  Baku  ",
            UtcNow);

        Assert.Equal(
            "AZ",
            session.CountryCode);

        Assert.Equal(
            "Baku",
            session.City);

        Assert.Equal(
            RegistrationStep.Bio,
            session.CurrentStep);
    }

    [Fact]
    public void SetBio_ShouldAllowEmptyBio()
    {
        var session =
            CreateAtBioStep();

        session.SetBio(
            null,
            UtcNow);

        Assert.Null(session.Bio);

        Assert.Equal(
            RegistrationStep.Photos,
            session.CurrentStep);
    }

    [Fact]
    public void SetBio_ShouldRejectBioLongerThanFiveHundredCharacters()
    {
        var session =
            CreateAtBioStep();

        var bio =
            new string('A', 501);

        Assert.Throws<ArgumentOutOfRangeException>(
            () => session.SetBio(
                bio,
                UtcNow));
    }

    [Fact]
    public void CallingStepOutOfOrder_ShouldFail()
    {
        var session =
            CreateSession();

        Assert.Throws<InvalidOperationException>(
            () => session.SetGender(
                Gender.Male,
                UtcNow));
    }

    [Fact]
    public void Complete_ShouldRejectSessionBeforeReview()
    {
        var session =
            CreateSession();

        Assert.Throws<InvalidOperationException>(
            () => session.Complete(
                UtcNow));
    }

    [Fact]
    public void Complete_ShouldCompleteValidRegistration()
    {
        var session =
            CreateAtReviewStep();

        var completedAt =
            UtcNow.AddMinutes(10);

        session.Complete(completedAt);

        Assert.Equal(
            RegistrationStep.Completed,
            session.CurrentStep);

        Assert.Equal(
            completedAt,
            session.CompletedAtUtc);

        Assert.Equal(
            completedAt,
            session.UpdatedAtUtc);
    }

    private static RegistrationSession CreateSession()
    {
        return RegistrationSession.Start(
            UserId,
            UtcNow);
    }

    private static RegistrationSession
        CreateAtDisplayNameStep()
    {
        var session =
            CreateSession();

        session.ConfirmAdult(
            UtcNow.AddMinutes(1));

        return session;
    }

    private static RegistrationSession
        CreateAtBirthDateStep()
    {
        var session =
            CreateAtDisplayNameStep();

        session.SetDisplayName(
            "Barlas",
            UtcNow.AddMinutes(2));

        return session;
    }

    private static RegistrationSession
        CreateAtGenderStep()
    {
        var session =
            CreateAtBirthDateStep();

        session.SetBirthDate(
            new DateOnly(2000, 1, 1),
            Today,
            UtcNow.AddMinutes(3));

        return session;
    }

    private static RegistrationSession
        CreateAtLookingForStep()
    {
        var session =
            CreateAtGenderStep();

        session.SetGender(
            Gender.Male,
            UtcNow.AddMinutes(4));

        return session;
    }

    private static RegistrationSession
        CreateAtLocationStep()
    {
        var session =
            CreateAtLookingForStep();

        session.SetLookingFor(
            GenderPreference.Female,
            UtcNow.AddMinutes(5));

        return session;
    }

    private static RegistrationSession
        CreateAtBioStep()
    {
        var session =
            CreateAtLocationStep();

        session.SetLocation(
            "TR",
            "Istanbul",
            UtcNow.AddMinutes(6));

        return session;
    }

    private static RegistrationSession
        CreateAtPhotosStep()
    {
        var session =
            CreateAtBioStep();

        session.SetBio(
            "Hello",
            UtcNow.AddMinutes(7));

        return session;
    }

    private static RegistrationSession
        CreateAtReviewStep()
    {
        var session =
            CreateAtPhotosStep();

        session.CompletePhotos(
            UtcNow.AddMinutes(8));

        return session;
    }
}