using Datezy.Domain.Registration;

namespace Datezy.Application.Registration.Start;

public sealed record StartRegistrationResult(
    Guid RegistrationSessionId,
    Guid UserId,
    RegistrationStep CurrentStep,
    bool IsNewSession);