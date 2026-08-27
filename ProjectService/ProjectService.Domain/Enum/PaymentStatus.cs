namespace ProjectService.Domain.Enum;

/// <summary>
/// Trạng thái giao dịch nạp tiền qua ví (mock).
/// </summary>
public enum PaymentStatus
{
    /// <summary>Đã tạo đơn, chờ ví xác nhận.</summary>
    Pending = 1,

    /// <summary>Ví đã xác nhận → tiền đã credit vào tài khoản.</summary>
    Success = 2,

    /// <summary>Người dùng hủy hoặc thanh toán thất bại.</summary>
    Failed = 3
}
