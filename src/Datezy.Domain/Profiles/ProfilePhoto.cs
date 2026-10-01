using Datezy.Domain.Common;

namespace Datezy.Domain.Profiles;

public sealed class ProfilePhoto : Entity
{
    public Guid ProfileId { get; private set; }

    public string TelegramFileId { get; private set; } = null!;

    public string TelegramFileUniqueId { get; private set; } = null!;

    public int SortOrder { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    private ProfilePhoto()
    {
    }

    private ProfilePhoto(
        Guid id,
        Guid profileId,
        string telegramFileId,
        string telegramFileUniqueId,
        int sortOrder,
        DateTime utcNow)
        : base(id)
    {
        if (profileId == Guid.Empty)
        {
            throw new ArgumentException(
                "Profile identifier cannot be empty.",
                nameof(profileId));
        }

        if (string.IsNullOrWhiteSpace(telegramFileId))
        {
            throw new ArgumentException(
                "Telegram file identifier is required.",
                nameof(telegramFileId));
        }

        if (string.IsNullOrWhiteSpace(telegramFileUniqueId))
        {
            throw new ArgumentException(
                "Telegram unique file identifier is required.",
                nameof(telegramFileUniqueId));
        }

        if (sortOrder is < 1 or > 3)
        {
            throw new ArgumentOutOfRangeException(
                nameof(sortOrder),
                "Photo sort order must be between 1 and 3.");
        }

        ProfileId = profileId;
        TelegramFileId = telegramFileId.Trim();
        TelegramFileUniqueId = telegramFileUniqueId.Trim();
        SortOrder = sortOrder;
        CreatedAtUtc = utcNow;
    }

    internal static ProfilePhoto Create(
        Guid profileId,
        string telegramFileId,
        string telegramFileUniqueId,
        int sortOrder,
        DateTime utcNow)
    {
        return new ProfilePhoto(
            Guid.NewGuid(),
            profileId,
            telegramFileId,
            telegramFileUniqueId,
            sortOrder,
            utcNow);
    }

    internal void ChangeSortOrder(int sortOrder)
    {
        if (sortOrder is < 1 or > 3)
        {
            throw new ArgumentOutOfRangeException(
                nameof(sortOrder));
        }

        SortOrder = sortOrder;
    }
}