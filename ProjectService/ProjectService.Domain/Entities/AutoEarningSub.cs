using ProjectService.Domain.Common;

namespace ProjectService.Domain.Entities;

/// <summary>
/// Đăng ký tự động trích tiền (Auto Earning) từ tài khoản.
/// </summary>
public class AutoEarningSub : BaseEntity
{
    public Guid AccountId { get; set; }

    public Account? Account { get; set; }

    public decimal Amount { get; set; }

    public int DayOfMonth { get; set; }

    public bool IsActive { get; set; } = true;
}
