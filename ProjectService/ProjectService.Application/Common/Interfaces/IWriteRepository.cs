using ProjectService.Domain.Common;

namespace ProjectService.Application.Common.Interfaces;

/// <summary>
/// Write repository (Port) — dùng cho Command (POST/PUT/PATCH/DELETE), có tracking để ghi DB.
/// Được triển khai bởi WriteRepository dùng ApplicationWriteDbContext.
/// </summary>
public interface IWriteRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    void Update(T entity);

    void Remove(T entity);
}
