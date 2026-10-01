using Datezy.Domain.Profiles;
using Datezy.Domain.Registration;
using Datezy.Domain.Users;
using Datezy.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Datezy.IntegrationTests.Persistence;

public sealed class DatezyDbContextModelTests
{
    private static DatezyDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<DatezyDbContext>()
            .UseNpgsql(
                "Host=localhost;Port=5432;Database=datezy_model_test;" +
                "Username=datezy;Password=datezy")
            .Options;

        return new DatezyDbContext(options);
    }

    [Fact]
    public void Model_ShouldBeCreatedSuccessfully()
    {
        using var context = CreateContext();

        var model = context.Model;

        Assert.NotNull(model);
    }

    [Fact]
    public void Model_ShouldContainExpectedEntities()
    {
        using var context = CreateContext();

        Assert.NotNull(
            context.Model.FindEntityType(typeof(User)));

        Assert.NotNull(
            context.Model.FindEntityType(typeof(Profile)));

        Assert.NotNull(
            context.Model.FindEntityType(typeof(ProfilePhoto)));

        Assert.NotNull(
            context.Model.FindEntityType(typeof(RegistrationSession)));
    }

    [Fact]
    public void User_ShouldMapToUsersTable()
    {
        using var context = CreateContext();

        var entity = context.Model.FindEntityType(typeof(User));

        Assert.NotNull(entity);
        Assert.Equal("users", entity.GetTableName());
    }

    [Fact]
    public void Profile_ShouldMapToProfilesTable()
    {
        using var context = CreateContext();

        var entity = context.Model.FindEntityType(typeof(Profile));

        Assert.NotNull(entity);
        Assert.Equal("profiles", entity.GetTableName());
    }

    [Fact]
    public void ProfilePhoto_ShouldMapToProfilePhotosTable()
    {
        using var context = CreateContext();

        var entity =
            context.Model.FindEntityType(typeof(ProfilePhoto));

        Assert.NotNull(entity);
        Assert.Equal(
            "profile_photos",
            entity.GetTableName());
    }

    [Fact]
    public void RegistrationSession_ShouldMapToRegistrationSessionsTable()
    {
        using var context = CreateContext();

        var entity =
            context.Model.FindEntityType(
                typeof(RegistrationSession));

        Assert.NotNull(entity);
        Assert.Equal(
            "registration_sessions",
            entity.GetTableName());
    }

    [Fact]
    public void User_ShouldHaveUniqueTelegramUserIdIndex()
    {
        using var context = CreateContext();

        var entity =
            context.Model.FindEntityType(typeof(User))!;

        var index = entity
            .GetIndexes()
            .SingleOrDefault(
                x => x.Properties.Count == 1 &&
                     x.Properties[0].Name ==
                     nameof(User.TelegramUserId));

        Assert.NotNull(index);
        Assert.True(index.IsUnique);
    }

    [Fact]
    public void Profile_ShouldHaveUniqueUserIdIndex()
    {
        using var context = CreateContext();

        var entity =
            context.Model.FindEntityType(typeof(Profile))!;

        var index = entity
            .GetIndexes()
            .SingleOrDefault(
                x => x.Properties.Count == 1 &&
                     x.Properties[0].Name ==
                     nameof(Profile.UserId));

        Assert.NotNull(index);
        Assert.True(index.IsUnique);
    }

    [Fact]
    public void RegistrationSession_ShouldHaveUniqueUserIdIndex()
    {
        using var context = CreateContext();

        var entity =
            context.Model.FindEntityType(
                typeof(RegistrationSession))!;

        var index = entity
            .GetIndexes()
            .SingleOrDefault(
                x => x.Properties.Count == 1 &&
                     x.Properties[0].Name ==
                     nameof(RegistrationSession.UserId));

        Assert.NotNull(index);
        Assert.True(index.IsUnique);
    }

    [Fact]
    public void ProfilePhoto_ShouldHaveUniqueProfileSortOrderIndex()
    {
        using var context = CreateContext();

        var entity =
            context.Model.FindEntityType(
                typeof(ProfilePhoto))!;

        var index = entity
            .GetIndexes()
            .SingleOrDefault(
                x =>
                    x.Properties.Count == 2 &&
                    x.Properties[0].Name ==
                    nameof(ProfilePhoto.ProfileId) &&
                    x.Properties[1].Name ==
                    nameof(ProfilePhoto.SortOrder));

        Assert.NotNull(index);
        Assert.True(index.IsUnique);
    }

    [Fact]
    public void Profile_ShouldReferenceUser()
    {
        using var context = CreateContext();

        var entity =
            context.Model.FindEntityType(typeof(Profile))!;

        var foreignKey = entity
            .GetForeignKeys()
            .SingleOrDefault(
                x => x.PrincipalEntityType.ClrType ==
                     typeof(User));

        Assert.NotNull(foreignKey);

        Assert.Equal(
            DeleteBehavior.Cascade,
            foreignKey.DeleteBehavior);
    }

    [Fact]
    public void RegistrationSession_ShouldReferenceUser()
    {
        using var context = CreateContext();

        var entity =
            context.Model.FindEntityType(
                typeof(RegistrationSession))!;

        var foreignKey = entity
            .GetForeignKeys()
            .SingleOrDefault(
                x => x.PrincipalEntityType.ClrType ==
                     typeof(User));

        Assert.NotNull(foreignKey);

        Assert.Equal(
            DeleteBehavior.Cascade,
            foreignKey.DeleteBehavior);
    }

    [Fact]
    public void ProfilePhoto_ShouldReferenceProfile()
    {
        using var context = CreateContext();

        var entity =
            context.Model.FindEntityType(
                typeof(ProfilePhoto))!;

        var foreignKey = entity
            .GetForeignKeys()
            .SingleOrDefault(
                x => x.PrincipalEntityType.ClrType ==
                     typeof(Profile));

        Assert.NotNull(foreignKey);

        Assert.Equal(
            DeleteBehavior.Cascade,
            foreignKey.DeleteBehavior);
    }
}