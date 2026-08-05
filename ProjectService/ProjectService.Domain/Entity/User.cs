using ProjectService.Domain.Common;

namespace ProjectService.Domain.Entity;

/// <summary>
/// Người dùng hệ thống ngân hàng.
/// </summary>
public class User : BaseEntity
{
    public required string FullName { get; set; }

    public required string Email { get; set; }

    /// <summary>Số điện thoại — dùng string để giữ số 0 đầu (VD: 0912...).</summary>
    public required string Phone { get; set; }

    public required string IdentityCard { get; set; }

    public required DateTime DateOfBirth { get; set; }

    /// <summary>Mật khẩu đã băm — KHÔNG bao giờ lưu mật khẩu thô.</summary>
    public required string PasswordHash { get; set; }

    public string? Address { get; set; }

    public bool IsActive { get; set; } = true;

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}
