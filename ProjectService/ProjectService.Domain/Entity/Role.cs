using ProjectService.Domain.Common;

namespace ProjectService.Domain.Entity;

/// <summary>
/// Vai trò (Role) trong hệ thống.
/// </summary>
public class Role : BaseEntity
{
    public required string RoleName { get; set; }

    public string? Code { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
