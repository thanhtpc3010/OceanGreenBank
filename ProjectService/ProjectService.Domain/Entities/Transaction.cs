using ProjectService.Domain.Common;
using ProjectService.Domain.Enums;

namespace ProjectService.Domain.Entities;

/// <summary>
/// Giao dịch tài chính trên tài khoản.
/// </summary>
public class Transaction : BaseEntity
{
    public Guid AccountId { get; set; }

    public Account? Account { get; set; }

    public TransactionType Type { get; set; }

    public TransactionStatus Status { get; set; }

    public decimal Amount { get; set; }

    public decimal BalanceAfter { get; set; }

    public string Description { get; set; } = string.Empty;
}
