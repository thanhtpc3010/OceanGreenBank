namespace ProjectService.Domain.Common;

/// <summary>
/// Lớp cơ sở cho tất cả các entity trong Domain Layer.
/// Id là string để linh hoạt trong nhiều loại cơ sở dữ liệu.
/// </summary>
public abstract class BaseEntity
{
    public required string Id { get; set; }

    public required DateTime CreatedDate { get; set; }

    public required string? CreatedBy { get; set; }

    public DateTime? LastModifiedDate { get; set; }

    public string? LastModifiedBy { get; set; }
}
