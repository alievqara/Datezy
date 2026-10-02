using Datezy.Domain.Registration;

namespace Datezy.Application.Common.Abstractions.Persistence;

public interface IRegistrationSessionRepository
{
    Task<RegistrationSession?> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    void Add(RegistrationSession registrationSession);
}