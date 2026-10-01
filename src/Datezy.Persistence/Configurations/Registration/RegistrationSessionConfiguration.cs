using Datezy.Domain.Profiles;
using Datezy.Domain.Registration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Datezy.Domain.Users;

namespace Datezy.Persistence.Configurations.Registration;

public sealed class RegistrationSessionConfiguration :
    IEntityTypeConfiguration<RegistrationSession>
{
    public void Configure(
        EntityTypeBuilder<RegistrationSession> builder)
    {
        builder.ToTable("registration_sessions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.HasIndex(x => x.UserId)
            .IsUnique();

        builder.HasOne<User>()
            .WithOne()
            .HasForeignKey<RegistrationSession>(
                x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.CurrentStep)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.IsAdultConfirmed)
            .IsRequired();

        builder.Property(x => x.DisplayName)
            .HasMaxLength(
                ProfileRules.MaximumDisplayNameLength);

        builder.Property(x => x.BirthDate);

        builder.Property(x => x.Gender)
            .HasConversion<int?>();

        builder.Property(x => x.LookingFor)
            .HasConversion<int?>();

        builder.Property(x => x.CountryCode)
            .HasMaxLength(2);

        builder.Property(x => x.City)
            .HasMaxLength(
                ProfileRules.MaximumCityLength);

        builder.Property(x => x.Bio)
            .HasMaxLength(
                ProfileRules.MaximumBioLength);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .IsRequired();

        builder.Property(x => x.CompletedAtUtc);
    }
}