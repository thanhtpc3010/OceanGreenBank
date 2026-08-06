using Microsoft.EntityFrameworkCore;
using ProjectService.Application.Common.Interfaces;
using ProjectService.Domain.Common;
using ProjectService.Infrastructure.Persistence.Contexts;

namespace ProjectService.Infrastructure.Persistence.Repositories;

/// <summary>
/// Triển khai IWriteRepository dùng ApplicationWriteDbContext (tracking) — cho Command.
/// </summary>
public class WriteRepository<T> : IWriteRepository<T> where T : BaseEntity
{
    private readonly ApplicationWriteDbContext _dbContext;

    public WriteRepository(ApplicationWriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        => await _dbContext.Set<T>().FindAsync([id], cancellationToken);

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        => await _dbContext.Set<T>().AddAsync(entity, cancellationToken);

    public void Update(T entity) => _dbContext.Set<T>().Update(entity);

    public void Remove(T entity) => _dbContext.Set<T>().Remove(entity);
}
