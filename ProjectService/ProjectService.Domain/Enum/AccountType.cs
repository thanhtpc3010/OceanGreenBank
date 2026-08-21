namespace ProjectService.Domain.Enum;

/// <summary>
/// Loại tài khoản ngân hàng.
/// - Normal: tài khoản thanh toán thông thường, rút/chuyển bất cứ lúc nào.
/// - Savings: tài khoản tiết kiệm có kỳ hạn — chỉ được rút khi đáo hạn (rút sớm mất lãi).
/// </summary>
public enum AccountType
{
    /// <summary>Tài khoản thanh toán thông thường.</summary>
    Normal = 0,

    /// <summary>Tài khoản tiết kiệm (có kỳ hạn + lãi suất).</summary>
    Savings = 1
}
