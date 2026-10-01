using Datezy.Domain.Profiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Datezy.Persistence.Configurations.Profiles;

public sealed class ProfilePhotoConfiguration :
    IEntityTypeConfiguration<ProfilePhoto>
{
    public void Configure(
        EntityTypeBuilder<ProfilePhoto> builder)
    {
        builder.ToTable("profile_photos");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.ProfileId)
            .IsRequired();

        builder.Property(x => x.TelegramFileId)
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(x => x.TelegramFileUniqueId)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(x => x.SortOrder)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(
                x => new
                {
                    x.ProfileId,
                    x.SortOrder
                })
            .IsUnique();

        builder.HasIndex(
                x => new
                {
                    x.ProfileId,
                    x.TelegramFileUniqueId
                })
            .IsUnique();
    }
}