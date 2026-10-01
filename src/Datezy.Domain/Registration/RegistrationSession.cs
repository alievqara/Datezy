using Datezy.Domain.Common;
using Datezy.Domain.Profiles;

namespace Datezy.Domain.Registration;

public sealed class RegistrationSession : AggregateRoot
{
    public Guid UserId { get; private set; }

    public RegistrationStep CurrentStep { get; private set; }

    public bool IsAdultConfirmed { get; private set; }

    public string? DisplayName { get; private set; }

    public DateOnly? BirthDate { get; private set; }

    public Gender? Gender { get; private set; }

    public GenderPreference? LookingFor { get; private set; }

    public string? CountryCode { get; private set; }

    public string? City { get; private set; }

    public string? Bio { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime UpdatedAtUtc { get; private set; }

    public DateTime? CompletedAtUtc { get; private set; }

    private RegistrationSession()
    {
    }

    private RegistrationSession(
        Guid id,
        Guid userId,
        DateTime utcNow)
        : base(id)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException(
                "User identifier cannot be empty.",
                nameof(userId));
        }

        UserId = userId;
        CurrentStep = RegistrationStep.AgeConfirmation;

        CreatedAtUtc = utcNow;
        UpdatedAtUtc = utcNow;
    }

    public static RegistrationSession Start(
        Guid userId,
        DateTime utcNow)
    {
        return new RegistrationSession(
            Guid.NewGuid(),
            userId,
            utcNow);
    }

    public void ConfirmAdult(
        DateTime utcNow)
    {
        EnsureCurrentStep(
            RegistrationStep.AgeConfirmation);

        IsAdultConfirmed = true;

        MoveTo(
            RegistrationStep.DisplayName,
            utcNow);
    }

    public void SetDisplayName(
        string displayName,
        DateTime utcNow)
    {
        EnsureCurrentStep(
            RegistrationStep.DisplayName);

        DisplayName =
            ProfileRules.NormalizeDisplayName(
                displayName);

        MoveTo(
            RegistrationStep.BirthDate,
            utcNow);
    }

    public void SetBirthDate(
        DateOnly birthDate,
        DateOnly today,
        DateTime utcNow)
    {
        EnsureCurrentStep(
            RegistrationStep.BirthDate);

        ProfileRules.EnsureValidBirthDate(
            birthDate,
            today);

        BirthDate = birthDate;

        MoveTo(
            RegistrationStep.Gender,
            utcNow);
    }

    public void SetGender(
        Gender gender,
        DateTime utcNow)
    {
        EnsureCurrentStep(
            RegistrationStep.Gender);

        ProfileRules.EnsureValidGender(
            gender);

        Gender = gender;

        MoveTo(
            RegistrationStep.LookingFor,
            utcNow);
    }

    public void SetLookingFor(
        GenderPreference preference,
        DateTime utcNow)
    {
        EnsureCurrentStep(
            RegistrationStep.LookingFor);

        ProfileRules.EnsureValidGenderPreference(
            preference);

        LookingFor = preference;

        MoveTo(
            RegistrationStep.Location,
            utcNow);
    }

    public void SetLocation(
        string countryCode,
        string city,
        DateTime utcNow)
    {
        EnsureCurrentStep(
            RegistrationStep.Location);

        CountryCode =
            ProfileRules.NormalizeCountryCode(
                countryCode);

        City =
            ProfileRules.NormalizeCity(
                city);

        MoveTo(
            RegistrationStep.Bio,
            utcNow);
    }

    public void SetBio(
        string? bio,
        DateTime utcNow)
    {
        EnsureCurrentStep(
            RegistrationStep.Bio);

        Bio =
            ProfileRules.NormalizeBio(
                bio);

        MoveTo(
            RegistrationStep.Photos,
            utcNow);
    }

    public void CompletePhotos(
        DateTime utcNow)
    {
        EnsureCurrentStep(
            RegistrationStep.Photos);

        MoveTo(
            RegistrationStep.Review,
            utcNow);
    }

    public void Complete(
        DateTime utcNow)
    {
        EnsureCurrentStep(
            RegistrationStep.Review);

        EnsureReadyForCompletion();

        CurrentStep =
            RegistrationStep.Completed;

        CompletedAtUtc = utcNow;
        UpdatedAtUtc = utcNow;
    }

    private void EnsureReadyForCompletion()
    {
        if (!IsAdultConfirmed ||
            string.IsNullOrWhiteSpace(DisplayName) ||
            BirthDate is null ||
            Gender is null ||
            LookingFor is null ||
            string.IsNullOrWhiteSpace(CountryCode) ||
            string.IsNullOrWhiteSpace(City))
        {
            throw new InvalidOperationException(
                "Registration is incomplete.");
        }
    }

    private void EnsureCurrentStep(
        RegistrationStep expectedStep)
    {
        if (CurrentStep != expectedStep)
        {
            throw new InvalidOperationException(
                $"Registration is currently at " +
                $"'{CurrentStep}', but " +
                $"'{expectedStep}' was expected.");
        }
    }

    private void MoveTo(
        RegistrationStep nextStep,
        DateTime utcNow)
    {
        CurrentStep = nextStep;
        UpdatedAtUtc = utcNow;
    }
}