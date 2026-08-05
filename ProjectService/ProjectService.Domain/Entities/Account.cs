using ProjectService.Domain.Common;
using ProjectService.Domain.Enums;

namespace ProjectService.Domain.Entities;

/// <summary>
/// Tài khoản ngân hàng của người dùng.
/// </summary>
public class Account : BaseEntity
{
    public Guid UserId { get; set; }

    public User? User { get; set; }

    public string AccountNumber { get; set; } = string.Empty;

    public AccountType Type { get; set; }

    public decimal Balance { get; set; }

    public string Currency { get; set; } = "VND";

    public bool IsActive { get; set; } = true;

    public SavingsAccount? SavingsAccount { get; set; }

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
