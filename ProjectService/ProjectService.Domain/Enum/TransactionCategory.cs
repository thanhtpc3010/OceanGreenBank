namespace ProjectService.Domain.Enum;

/// <summary>
/// Danh mục thu chi — do người dùng chọn khi tạo giao dịch, BOT PFM chỉ gom nhóm + tổng hợp.
/// </summary>
public enum TransactionCategory
{
    /// <summary>Chưa phân loại / mặc định.</summary>
    Other = 0,

    /// <summary>Ăn uống.</summary>
    Food = 1,

    /// <summary>Mua sắm.</summary>
    Shopping = 2,

    /// <summary>Hóa đơn (điện, nước, internet...).</summary>
    Bills = 3,

    /// <summary>Di chuyển (grab, xăng, taxi...).</summary>
    Transport = 4,

    /// <summary>Giải trí.</summary>
    Entertainment = 5,

    /// <summary>Y tế.</summary>
    Health = 6,

    /// <summary>Giáo dục.</summary>
    Education = 7,

    /// <summary>Tiết kiệm / đầu tư.</summary>
    Savings = 8,

    /// <summary>Chuyển khoản.</summary>
    Transfer = 9
}
