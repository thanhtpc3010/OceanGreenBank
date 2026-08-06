using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ProjectService.Application.Common.Interfaces;
using ProjectService.Domain.Common;
using ProjectService.Infrastructure.Persistence.Contexts;

namespace ProjectService.Infrastructure.Persistence.Repositories;

/// <summary>
/// Triển khai IReadRepository dùng ApplicationReadDbContext (NoTracking) — tối ưu cho Query.
/// </summary>
public class ReadRepository<T> : IReadRepository<T> where T : BaseEntity
{
    private readonly ApplicationReadDbContext _dbContext;

    public ReadRepository(ApplicationReadDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        => await _dbContext.Set<T>().FindAsync([id], cancellationToken);

    public async Task<IReadOnlyList<T>> ListAsync(CancellationToken cancellationToken = default)
        => await _dbContext.Set<T>().AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
        => await _dbContext.Set<T>().AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
}
