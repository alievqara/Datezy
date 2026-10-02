using Datezy.Application.Common.Abstractions;
using Datezy.Application.Common.Abstractions.Persistence;
using Datezy.Domain.Users;

namespace Datezy.Application.Users.Start;

public sealed class StartUserHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;

    public StartUserHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IClock clock)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    public async Task<StartUserResult> HandleAsync(
        StartUserCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var utcNow = _clock.UtcNow;

        var user =
            await _userRepository.GetByTelegramUserIdAsync(
                command.TelegramUserId,
                cancellationToken);

        if (user is null)
        {
            user = User.CreateFromTelegram(
                command.TelegramUserId,
                command.TelegramChatId,
                command.FirstName,
                command.LastName,
                command.Username,
                command.TelegramLanguageCode,
                utcNow);

            _userRepository.Add(user);

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            return new StartUserResult(
                user.Id,
                IsNewUser: true,
                user.RegistrationStatus,
                user.PreferredLanguageCode);
        }

        user.UpdateTelegramIdentity(
            command.TelegramChatId,
            command.FirstName,
            command.LastName,
            command.Username,
            command.TelegramLanguageCode,
            utcNow);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new StartUserResult(
            user.Id,
            IsNewUser: false,
            user.RegistrationStatus,
            user.PreferredLanguageCode);
    }
}