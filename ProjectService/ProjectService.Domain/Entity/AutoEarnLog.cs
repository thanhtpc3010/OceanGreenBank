using ProjectService.Domain.Common;

namespace ProjectService.Domain.Entity;

/// <summary>
/// Nhật ký một lần sinh lời AutoEarn cho một tài khoản tham gia.
/// </summary>
public class AutoEarnLog : BaseEntity
{
    /// <summary>Tài khoản được cộng lãi.</summary>
    public required string AccountId { get; set; }

    public virtual Account Account { get; set; } = null!;

    public string AccountNumber { get; set; } = "";

    /// <summary>Ngày chạy (theo giờ Việt Nam, phần ngày).</summary>
    public DateTime RunDate { get; set; }

    /// <summary>Tiền gốc tham gia.</summary>
    public decimal Principal { get; set; }

    /// <summary>Số tiền lãi cộng vào tài khoản.</summary>
    public decimal InterestAmount { get; set; }

    /// <summary>Lãi suất %/năm áp dụng.</summary>
    public decimal AnnualRate { get; set; }
}
