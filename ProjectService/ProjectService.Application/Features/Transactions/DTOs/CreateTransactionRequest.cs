using ProjectService.Domain.Enums;

namespace ProjectService.Application.Features.Transactions.DTOs;

/// <summary>
/// DTO dùng để tạo giao dịch mới.
/// </summary>
public sealed record CreateTransactionRequest(
    Guid AccountId,
    TransactionType Type,
    decimal Amount,
    string Description = "");
