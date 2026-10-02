using Datezy.Application.Registration.Start;
using Datezy.Application.Users.Start;
using Microsoft.Extensions.DependencyInjection;

namespace Datezy.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<StartUserHandler>();
        services.AddScoped<StartRegistrationHandler>();

        return services;
    }
}