using Datezy.Application.Common.Abstractions.Persistence;
using Datezy.Persistence.Contexts;
using Datezy.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Datezy.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var connectionString =
            configuration.GetConnectionString("PostgreSql");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'PostgreSql' is not configured.");
        }

        services.AddDbContext<DatezyDbContext>(
            options =>
            {
                options.UseNpgsql(
                    connectionString,
                    npgsqlOptions =>
                    {
                        npgsqlOptions.MigrationsAssembly(
                            typeof(DatezyDbContext)
                                .Assembly
                                .FullName);

                        npgsqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5);
                    });
            });

        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<
            IRegistrationSessionRepository,
            RegistrationSessionRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}