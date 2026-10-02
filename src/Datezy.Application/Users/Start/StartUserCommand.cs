namespace Datezy.Application.Users.Start;

public sealed record StartUserCommand(
    long TelegramUserId,
    long TelegramChatId,
    string FirstName,
    string? LastName,
    string? Username,
    string? TelegramLanguageCode);