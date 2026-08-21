using ProjectService.Domain.Common;
using ProjectService.Domain.Enum;

namespace ProjectService.Domain.Entity;

/// <summary>
/// Giao dịch tài chính (chuyển nội bộ hoặc liên ngân hàng).
/// </summary>
public class Transaction : BaseEntity
{
    /// <summary>Mã giao dịch duy nhất (VD: TXN20260805001).</summary>
    public required string TransactionCode { get; set; }

    /// <summary>Tài khoản gửi (bắt buộc).</summary>
    public required string FromAccountId { get; set; }

    public virtual Account FromAccount { get; set; } = null!;

    /// <summary>Tài khoản nhận trong hệ thống — null nếu chuyển ra ngân hàng ngoài.</summary>
    public string? ToAccountId { get; set; }

    /// <summary>Số tài khoản người nhận (khi chuyển liên ngân hàng).</summary>
    public string? ReceiverAccount { get; set; }

    public string? ReceiverName { get; set; }

    /// <summary>Mã ngân hàng nhận (VD: VCB, TCB).</summary>
    public string? ReceiverBankCode { get; set; }

    public decimal Amount { get; set; }

    public decimal Fee { get; set; } = 0;

    public string? Description { get; set; }

    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;

    public TransactionType Type { get; set; } = TransactionType.InternalTransfer;

    /// <summary>Danh mục thu chi do người dùng chọn (phục vụ PFM).</summary>
    public TransactionCategory Category { get; set; } = TransactionCategory.Other;
}
