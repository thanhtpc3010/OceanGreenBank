using ProjectService.Domain.Common;

namespace ProjectService.Domain.Entity;

/// <summary>
/// Chi nhánh / điểm giao dịch của ngân hàng.
/// </summary>
public class Site : BaseEntity
{
    public required string Name { get; set; }

    public string? City { get; set; }
}
