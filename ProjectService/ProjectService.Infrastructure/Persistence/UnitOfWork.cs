using ProjectService.Application.Common.Interfaces;
using ProjectService.Infrastructure.Persistence.Contexts;

namespace ProjectService.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationWriteDbContext _dbContext;

    public UnitOfWork(ApplicationWriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _dbContext.SaveChangesAsync(cancellationToken);
}
