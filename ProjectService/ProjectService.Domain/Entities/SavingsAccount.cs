using ProjectService.Domain.Common;

namespace ProjectService.Domain.Entities;

/// <summary>
/// Tài khoản tiết kiệm (gắn với một tài khoản chính).
/// </summary>
public class SavingsAccount : BaseEntity
{
    public Guid AccountId { get; set; }

    public Account? Account { get; set; }

    public decimal InterestRate { get; set; }

    public DateTime StartDate { get; set; } = DateTime.UtcNow;

    public DateTime? MaturityDate { get; set; }
}
