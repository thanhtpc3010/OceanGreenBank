using ProjectService.Domain.Common;

namespace ProjectService.Domain.Entity;

/// <summary>
/// Ngân hàng (dùng cho chuyển tiền liên ngân hàng / định danh chi nhánh).
/// </summary>
public class Bank : BaseEntity
{
    public required string Name { get; set; }

    /// <summary>Mã BIN ngân hàng (VD: 970436 cho VCB).</summary>
    public required string BinCode { get; set; }

    public string? SwiftCode { get; set; }

    public virtual ICollection<Site> Sites { get; set; } = new List<Site>();
}
