using Datezy.Application.Common.Abstractions.Persistence;
using Datezy.Persistence.Contexts;

namespace Datezy.Persistence.Repositories;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly DatezyDbContext _dbContext;

    public UnitOfWork(DatezyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
