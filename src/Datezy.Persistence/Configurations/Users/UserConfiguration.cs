using Datezy.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Datezy.Persistence.Configurations.Users;

public sealed class UserConfiguration :
    IEntityTypeConfiguration<User>
{
    public void Configure(
        EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.TelegramUserId)
            .IsRequired();

        builder.HasIndex(x => x.TelegramUserId)
            .IsUnique();

        builder.Property(x => x.TelegramChatId)
            .IsRequired();

        builder.Property(x => x.TelegramUsername)
            .HasMaxLength(64);

        builder.Property(x => x.TelegramFirstName)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(x => x.TelegramLastName)
            .HasMaxLength(64);

        builder.Property(x => x.TelegramLanguageCode)
            .HasMaxLength(16);

        builder.Property(x => x.PreferredLanguageCode)
            .HasMaxLength(16);

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.RegistrationStatus)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .IsRequired();

        builder.Property(x => x.LastSeenAtUtc)
            .IsRequired();

        builder.HasIndex(x => x.LastSeenAtUtc);
    }
}