using ProjectService.Domain.Enum;

namespace ProjectService.Application.Services.DTOs;

/// <summary>Chi tiết tổng chi theo từng danh mục.</summary>
public sealed record CategorySummaryDto(
    TransactionCategory Category,
    string CategoryName,
    decimal Total,
    int Count);

/// <summary>
/// Tổng hợp thu chi cho PFM — do BOT PFM gom nhóm từ dữ liệu user đã gán danh mục.
/// </summary>
public sealed record PfmSummaryDto(
    decimal TotalIncome,
    decimal TotalExpense,
    decimal Net,
    IReadOnlyList<CategorySummaryDto> ExpenseByCategory);
