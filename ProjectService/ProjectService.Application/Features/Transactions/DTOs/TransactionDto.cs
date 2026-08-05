using ProjectService.Domain.Enums;

namespace ProjectService.Application.Features.Transactions.DTOs;

/// <summary>
/// DTO trả về thông tin giao dịch.
/// </summary>
public sealed record TransactionDto(
    Guid Id,
    Guid AccountId,
    TransactionType Type,
    TransactionStatus Status,
    decimal Amount,
    decimal BalanceAfter,
    string Description,
    DateTime CreatedAtUtc);
