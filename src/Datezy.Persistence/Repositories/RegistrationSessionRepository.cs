using Datezy.Application.Common.Abstractions.Persistence;
using Datezy.Domain.Registration;
using Datezy.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Datezy.Persistence.Repositories;

public sealed class RegistrationSessionRepository
    : IRegistrationSessionRepository
{
    private readonly DatezyDbContext _dbContext;

    public RegistrationSessionRepository(
        DatezyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<RegistrationSession?> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.RegistrationSessions
            .SingleOrDefaultAsync(
                session => session.UserId == userId,
                cancellationToken);
    }

    public void Add(
        RegistrationSession registrationSession)
    {
        ArgumentNullException.ThrowIfNull(
            registrationSession);

        _dbContext.RegistrationSessions.Add(
            registrationSession);
    }
}