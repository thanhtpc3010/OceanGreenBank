namespace ProjectService.Application.Common.Interfaces;

/// <summary>
/// Unit of Work — đảm bảo ACID khi lưu nhiều thay đổi trong một transaction.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
