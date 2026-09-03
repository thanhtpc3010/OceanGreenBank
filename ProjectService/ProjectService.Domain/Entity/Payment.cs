using ProjectService.Domain.Common;
using ProjectService.Domain.Enum;

namespace ProjectService.Domain.Entity;

/// <summary>
/// Giao dịch nạp tiền qua ví điện tử (MoMo / ZaloPay) — bản mock local.
/// Khi ví xác nhận thanh toán thành công → số dư tài khoản SmartBank được credit.
/// </summary>
public class Payment : BaseEntity
{
    /// <summary>Người nạp tiền (chủ tài khoản nhận).</summary>
    public required string UserId { get; set; }

    /// <summary>Ví thanh toán: MoMo / ZaloPay.</summary>
    public PaymentProvider Provider { get; set; } = PaymentProvider.Momo;

    /// <summary>Tài khoản SmartBank nhận tiền.</summary>
    public required string AccountId { get; set; }

    /// <summary>Số tài khoản đích (snapshot để hiển thị).</summary>
    public required string AccountNumber { get; set; }

    public decimal Amount { get; set; }

    /// <summary>Mã đơn hàng mô phỏng của ví (VD: MOMO..., ZLP...).</summary>
    public string? OrderCode { get; set; }

    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    public string? Description { get; set; }

    /// <summary>Thời điểm ví xác nhận thanh toán (mock IPN).</summary>
    public DateTime? CompletedDate { get; set; }
}
