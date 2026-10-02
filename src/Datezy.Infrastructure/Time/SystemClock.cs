// Datezy.Infrastructure/Time/SystemClock.cs

using Datezy.Application.Common.Abstractions;

namespace Datezy.Infrastructure.Time;

public sealed class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}