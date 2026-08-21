using ProjectService.Domain.Common;

namespace ProjectService.Domain.Entity;

/// <summary>
/// Cấu hình toàn cục của tính năng AutoEarn (sinh lời tự động).
/// Bảng chỉ có 1 dòng duy nhất; được tạo tự động khi server khởi động.
/// </summary>
public class AutoEarnSetting : BaseEntity
{
    /// <summary>Bật/tắt tính năng AutoEarn.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Lãi suất %/năm (VD: 4.5 = 4.5%/năm).</summary>
    public decimal AnnualInterestRate { get; set; } = 4.5m;

    /// <summary>Thời gian tự động chạy mỗi ngày, định dạng "HH:mm" (theo giờ Việt Nam, UTC+7).</summary>
    public string RunTime { get; set; } = "00:00";

    /// <summary>Lần chạy gần nhất (UTC).</summary>
    public DateTime? LastRunAt { get; set; }

    /// <summary>Lần chạy kế tiếp (UTC).</summary>
    public DateTime? NextRunAt { get; set; }
}
