using Datezy.Application.Common.Abstractions;
using Datezy.Application.Common.Abstractions.Persistence;
using Datezy.Domain.Registration;
using Datezy.Domain.Users;

namespace Datezy.Application.Registration.Start;

public sealed class StartRegistrationHandler
{
    private readonly IUserRepository _userRepository;

    private readonly IRegistrationSessionRepository
        _registrationSessionRepository;

    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;

    public StartRegistrationHandler(
        IUserRepository userRepository,
        IRegistrationSessionRepository
            registrationSessionRepository,
        IUnitOfWork unitOfWork,
        IClock clock)
    {
        _userRepository = userRepository;

        _registrationSessionRepository =
            registrationSessionRepository;

        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    public async Task<StartRegistrationResult> HandleAsync(
        StartRegistrationCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.UserId == Guid.Empty)
        {
            throw new ArgumentException(
                "User identifier cannot be empty.",
                nameof(command));
        }

        var user =
            await _userRepository.GetByIdAsync(
                command.UserId,
                cancellationToken);

        if (user is null)
        {
            throw new InvalidOperationException(
                $"User '{command.UserId}' was not found.");
        }

        var existingSession =
            await _registrationSessionRepository
                .GetByUserIdAsync(
                    command.UserId,
                    cancellationToken);

        if (existingSession is not null)
        {
            return new StartRegistrationResult(
                existingSession.Id,
                existingSession.UserId,
                existingSession.CurrentStep,
                IsNewSession: false);
        }

        if (user.RegistrationStatus ==
            RegistrationStatus.Completed)
        {
            throw new InvalidOperationException(
                "Completed registration cannot be started again.");
        }

        var utcNow = _clock.UtcNow;

        user.StartRegistration(utcNow);

        var registrationSession =
            RegistrationSession.Start(
                user.Id,
                utcNow);

        _registrationSessionRepository.Add(
            registrationSession);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new StartRegistrationResult(
            registrationSession.Id,
            registrationSession.UserId,
            registrationSession.CurrentStep,
            IsNewSession: true);
    }
}