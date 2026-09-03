using ProjectService.Domain.Common;

namespace ProjectService.Domain.Entity;

/// <summary>
/// Bản dịch giao diện (i18n) — lưu trong DB để quản trị viên có thể
/// chỉnh sửa nội dung ngôn ngữ qua màn hình quản trị mà không cần code lại.
/// </summary>
public class Translation : BaseEntity
{
    /// <summary>Khóa dịch (VD: "SIDEBAR.DASHBOARD").</summary>
    public required string Key { get; set; }

    /// <summary>Mã ngôn ngữ ("vi" | "en").</summary>
    public required string Language { get; set; }

    /// <summary>Nội dung bản dịch.</summary>
    public required string Value { get; set; }
}
