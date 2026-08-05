using ProjectService.Domain.Common;

namespace ProjectService.Domain.Entities;

/// <summary>
/// Người dùng của hệ thống ngân hàng.
/// </summary>
public class User : BaseEntity
{
    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTime? LastLoginAtUtc { get; set; }

    public ICollection<Account> Accounts { get; set; } = new List<Account>();
}
