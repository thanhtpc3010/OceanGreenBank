using ProjectService.Domain.Common;

namespace ProjectService.Domain.Entity;

/// <summary>
/// Kế hoạch gửi tiết kiệm theo chu kỳ (hằng ngày / hằng tuần / hằng tháng).
/// Mỗi kỳ, hệ thống tự trích tiền từ tài khoản nguồn vào tài khoản tiết kiệm đích.
/// </summary>
public class SavingsPlan : BaseEntity
{
    /// <summary>Chủ sở hữu kế hoạch.</summary>
    public required string UserId { get; set; }

    public virtual User User { get; set; } = null!;

    /// <summary>Tài khoản nguồn (trích tiền).</summary>
    public required string SourceAccountId { get; set; }

    public virtual Account SourceAccount { get; set; } = null!;

    /// <summary>Tài khoản tiết kiệm đích (nhận tiền).</summary>
    public required string TargetAccountId { get; set; }

    public virtual Account TargetAccount { get; set; } = null!;

    /// <summary>Số tiền gửi mỗi kỳ.</summary>
    public decimal Amount { get; set; }

    /// <summary>Chu kỳ: DAILY / WEEKLY / MONTHLY.</summary>
    public string Cycle { get; set; } = "MONTHLY";

    /// <summary>Ngày bắt đầu.</summary>
    public DateTime StartDate { get; set; }

    /// <summary>Ngày gửi kỳ tới.</summary>
    public DateTime? NextDepositDate { get; set; }

    public bool IsActive { get; set; } = true;

    /// <summary>Số kỳ đã gửi.</summary>
    public int TotalDeposits { get; set; } = 0;

    /// <summary>Tổng số tiền đã gửi.</summary>
    public decimal TotalSaved { get; set; } = 0;
}
