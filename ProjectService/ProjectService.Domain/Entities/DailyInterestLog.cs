using ProjectService.Domain.Common;

namespace ProjectService.Domain.Entities;

/// <summary>
/// Nhật ký lãi suất hàng ngày của tài khoản tiết kiệm.
/// </summary>
public class DailyInterestLog : BaseEntity
{
    public Guid SavingsAccountId { get; set; }

    public SavingsAccount? SavingsAccount { get; set; }

    public DateTime LogDate { get; set; }

    public decimal InterestAmount { get; set; }
}
