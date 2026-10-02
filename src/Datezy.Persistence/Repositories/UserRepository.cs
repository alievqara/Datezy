using Datezy.Application.Common.Abstractions.Persistence;
using Datezy.Domain.Users;
using Datezy.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Datezy.Persistence.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly DatezyDbContext _dbContext;

    public UserRepository(DatezyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<User?> GetByTelegramUserIdAsync(
        long telegramUserId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Users
            .SingleOrDefaultAsync(
                user => user.TelegramUserId == telegramUserId,
                cancellationToken);
    }

    public Task<User?> GetByIdAsync(
    Guid userId,
    CancellationToken cancellationToken = default)
    {
        return _dbContext.Users
            .SingleOrDefaultAsync(
                user => user.Id == userId,
                cancellationToken);
    }

    public void Add(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        _dbContext.Users.Add(user);
    }
}
