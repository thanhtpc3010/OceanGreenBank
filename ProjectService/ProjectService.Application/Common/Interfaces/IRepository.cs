using System.Linq.Expressions;
using ProjectService.Domain.Common;

namespace ProjectService.Application.Common.Interfaces;

/// <summary>
/// Generic repository (Port) — Application chỉ khai báo interface, không phụ thuộc EF Core.
/// </summary>
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<T>> ListAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    void Update(T entity);

    void Remove(T entity);
}
