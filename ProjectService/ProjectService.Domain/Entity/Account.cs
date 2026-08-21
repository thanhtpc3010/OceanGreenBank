using ProjectService.Domain.Common;
using ProjectService.Domain.Enum;

namespace ProjectService.Domain.Entity;

/// <summary>
/// Tài khoản ngân hàng của người dùng.
/// </summary>
public class Account : BaseEntity
{
    public required string AccountNumber { get; set; }

    public required string UserId { get; set; }

    public virtual User User { get; set; } = null!;

    public decimal Balance { get; set; }

    public string Currency { get; set; } = "VND";

    public bool IsActive { get; set; } = true;

    /// <summary>Loại tài khoản: Normal (thông thường) hoặc Savings (tiết kiệm).</summary>
    public AccountType Type { get; set; } = AccountType.Normal;

    /* ===== Chỉ dùng cho tài khoản tiết kiệm (Type = Savings) ===== */

    /// <summary>Kỳ hạn (tháng): 1, 3, 6, 12...</summary>
    public int? SavingsTermMonths { get; set; }

    /// <summary>Lãi suất năm (%): vd 4.5 = 4.5%/năm.</summary>
    public decimal? InterestRate { get; set; }

    /// <summary>Ngày bắt đầu kỳ hạn hiện tại.</summary>
    public DateTime? SavingsStartDate { get; set; }

    /// <summary>Ngày đáo hạn kỳ hạn hiện tại (= start + kỳ hạn).</summary>
    public DateTime? SavingsMaturityDate { get; set; }

    /* ===== AutoEarn (sinh lời tự động) ===== */

    /// <summary>Có tham gia AutoEarn hay không.</summary>
    public bool IsAutoEarnEnrolled { get; set; }

    /// <summary>Tiền gốc tham gia AutoEarn (VND).</summary>
    public decimal AutoEarnPrincipal { get; set; }

    /// <summary>Các giao dịch gửi đi từ tài khoản này.</summary>
    public virtual ICollection<Transaction> FromTransactions { get; set; } = new List<Transaction>();
}
