using Datezy.Domain.Common;

namespace Datezy.Domain.Users;

public sealed class User : AggregateRoot
{
    public long TelegramUserId { get; private set; }

    public long TelegramChatId { get; private set; }

    public string? TelegramUsername { get; private set; }

    public string TelegramFirstName { get; private set; } = null!;

    public string? TelegramLastName { get; private set; }

    public string? TelegramLanguageCode { get; private set; }

    public string? PreferredLanguageCode { get; private set; }

    public UserStatus Status { get; private set; }

    public RegistrationStatus RegistrationStatus { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime UpdatedAtUtc { get; private set; }

    public DateTime LastSeenAtUtc { get; private set; }

    private User()
    {
    }

    private User(
        Guid id,
        long telegramUserId,
        long telegramChatId,
        string firstName,
        string? lastName,
        string? username,
        string? telegramLanguageCode,
        DateTime utcNow)
        : base(id)
    {
        if (telegramUserId <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(telegramUserId),
                "Telegram user identifier must be positive.");
        }

        if (telegramChatId <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(telegramChatId),
                "Telegram chat identifier must be positive.");
        }

        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new ArgumentException(
                "Telegram first name is required.",
                nameof(firstName));
        }

        TelegramUserId = telegramUserId;
        TelegramChatId = telegramChatId;

        TelegramFirstName = firstName.Trim();
        TelegramLastName = NormalizeOptional(lastName);
        TelegramUsername = NormalizeOptional(username);
        TelegramLanguageCode = NormalizeLanguageCode(telegramLanguageCode);

        Status = UserStatus.Active;
        RegistrationStatus = RegistrationStatus.NotStarted;

        CreatedAtUtc = utcNow;
        UpdatedAtUtc = utcNow;
        LastSeenAtUtc = utcNow;
    }

    public static User CreateFromTelegram(
        long telegramUserId,
        long telegramChatId,
        string firstName,
        string? lastName,
        string? username,
        string? telegramLanguageCode,
        DateTime utcNow)
    {
        return new User(
            Guid.NewGuid(),
            telegramUserId,
            telegramChatId,
            firstName,
            lastName,
            username,
            telegramLanguageCode,
            utcNow);
    }

    public void UpdateTelegramIdentity(
        long telegramChatId,
        string firstName,
        string? lastName,
        string? username,
        string? telegramLanguageCode,
        DateTime utcNow)
    {
        if (telegramChatId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(telegramChatId));
        }

        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new ArgumentException(
                "Telegram first name is required.",
                nameof(firstName));
        }

        TelegramChatId = telegramChatId;
        TelegramFirstName = firstName.Trim();
        TelegramLastName = NormalizeOptional(lastName);
        TelegramUsername = NormalizeOptional(username);
        TelegramLanguageCode = NormalizeLanguageCode(telegramLanguageCode);

        Touch(utcNow);
    }

    public void SetPreferredLanguage(
        string languageCode,
        DateTime utcNow)
    {
        if (string.IsNullOrWhiteSpace(languageCode))
        {
            throw new ArgumentException(
                "Language code is required.",
                nameof(languageCode));
        }

        PreferredLanguageCode = NormalizeLanguageCode(languageCode);

        Touch(utcNow);
    }

    public void StartRegistration(DateTime utcNow)
    {
        if (RegistrationStatus == RegistrationStatus.Completed)
        {
            throw new InvalidOperationException(
                "Registration has already been completed.");
        }

        RegistrationStatus = RegistrationStatus.InProgress;

        Touch(utcNow);
    }

    public void CompleteRegistration(DateTime utcNow)
    {
        if (RegistrationStatus != RegistrationStatus.InProgress)
        {
            throw new InvalidOperationException(
                "Registration must be in progress before completion.");
        }

        RegistrationStatus = RegistrationStatus.Completed;

        Touch(utcNow);
    }

    public void MarkSeen(DateTime utcNow)
    {
        LastSeenAtUtc = utcNow;
        UpdatedAtUtc = utcNow;
    }

    private void Touch(DateTime utcNow)
    {
        UpdatedAtUtc = utcNow;
        LastSeenAtUtc = utcNow;
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }

    private static string? NormalizeLanguageCode(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim().ToLowerInvariant();
    }
}