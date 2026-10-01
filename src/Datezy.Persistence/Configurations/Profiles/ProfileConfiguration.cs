using Datezy.Domain.Profiles;
using Datezy.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Datezy.Persistence.Configurations.Profiles;

public sealed class ProfileConfiguration :
    IEntityTypeConfiguration<Profile>
{
    public void Configure(
        EntityTypeBuilder<Profile> builder)
    {
        builder.ToTable("profiles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.HasIndex(x => x.UserId)
            .IsUnique();

        builder.HasOne<User>()
            .WithOne()
            .HasForeignKey<Profile>(
                x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.DisplayName)
            .HasMaxLength(
                ProfileRules.MaximumDisplayNameLength)
            .IsRequired();

        builder.Property(x => x.BirthDate)
            .IsRequired();

        builder.Property(x => x.Gender)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.LookingFor)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.CountryCode)
            .HasMaxLength(2)
            .IsRequired();

        builder.Property(x => x.City)
            .HasMaxLength(
                ProfileRules.MaximumCityLength)
            .IsRequired();

        builder.Property(x => x.Bio)
            .HasMaxLength(
                ProfileRules.MaximumBioLength);

        builder.Property(x => x.Visibility)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .IsRequired();

        builder.HasMany(x => x.Photos)
            .WithOne()
            .HasForeignKey(x => x.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}