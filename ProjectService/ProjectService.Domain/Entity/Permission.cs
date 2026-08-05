using ProjectService.Domain.Common;

namespace ProjectService.Domain.Entity;

/// <summary>
/// Quyền (Permission) — Code dùng để kiểm tra quyền trong logic code (VD: TRANSACTION.CREATE).
/// </summary>
public class Permission : BaseEntity
{
    public required string Name { get; set; }

    public required string Code { get; set; }

    public string? Description { get; set; }
}
