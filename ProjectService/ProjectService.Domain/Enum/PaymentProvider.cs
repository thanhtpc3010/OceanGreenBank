namespace ProjectService.Domain.Enum;

/// <summary>
/// Nhà cung cấp ví thanh toán — hiện mock local (không gọi API thật).
/// </summary>
public enum PaymentProvider
{
    /// <summary>Ví MoMo.</summary>
    Momo = 1,

    /// <summary>Ví ZaloPay.</summary>
    ZaloPay = 2,

    /// <summary>Tiền mặt — nạp tại quầy/ATM, tiền vào ngay (không qua ví).</summary>
    Cash = 3
}
