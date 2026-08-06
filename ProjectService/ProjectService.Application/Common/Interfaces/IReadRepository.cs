using System.Linq.Expressions;
using ProjectService.Domain.Common;

namespace ProjectService.Application.Common.Interfaces;

/// <summary>
/// Read-only repository (Port) — dùng cho Query, chỉ đọc dữ liệu, không tracking.
/// Được triển khai bởi ReadRepository dùng ApplicationReadDbContext (NoTracking).
/// </summary>
public interface IReadRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<T>> ListAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
}
