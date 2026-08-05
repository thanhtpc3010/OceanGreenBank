namespace ProjectService.Domain.Common;

/// <summary>
/// Lớp cơ sở cho tất cả các entity trong Domain Layer.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAtUtc { get; set; }
}
