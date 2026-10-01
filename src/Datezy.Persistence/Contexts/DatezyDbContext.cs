using Datezy.Domain.Profiles;
using Datezy.Domain.Registration;
using Datezy.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Datezy.Persistence.Contexts;

public sealed class DatezyDbContext : DbContext
{
    public DatezyDbContext(
        DbContextOptions<DatezyDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users =>
        Set<User>();

    public DbSet<Profile> Profiles =>
        Set<Profile>();

    public DbSet<ProfilePhoto> ProfilePhotos =>
        Set<ProfilePhoto>();

    public DbSet<RegistrationSession> RegistrationSessions =>
        Set<RegistrationSession>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(DatezyDbContext).Assembly);
    }
}