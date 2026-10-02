using Datezy.Domain.Users;

namespace Datezy.Application.Users.Start;

public sealed record StartUserResult(
    Guid UserId,
    bool IsNewUser,
    RegistrationStatus RegistrationStatus,
    string? PreferredLanguageCode);