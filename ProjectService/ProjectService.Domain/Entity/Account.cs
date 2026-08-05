using ProjectService.Domain.Common;

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

    /// <summary>Các giao dịch gửi đi từ tài khoản này.</summary>
    public virtual ICollection<Transaction> FromTransactions { get; set; } = new List<Transaction>();
}
