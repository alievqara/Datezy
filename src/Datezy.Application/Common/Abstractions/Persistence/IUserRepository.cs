using Datezy.Domain.Users;

namespace Datezy.Application.Common.Abstractions.Persistence;

public interface IUserRepository
{
    Task<User?> GetByTelegramUserIdAsync(
        long telegramUserId,
        CancellationToken cancellationToken = default);

    void Add(User user);
}
