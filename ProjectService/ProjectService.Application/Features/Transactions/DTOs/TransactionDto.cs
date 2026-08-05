using ProjectService.Domain.Enum;

namespace ProjectService.Application.Features.Transactions.DTOs;

/// <summary>
/// DTO trả về thông tin giao dịch.
/// </summary>
public sealed record TransactionDto(
    string Id,
    string TransactionCode,
    string FromAccountId,
    string? ToAccountId,
    string? ReceiverAccount,
    string? ReceiverName,
    string? ReceiverBankCode,
    decimal Amount,
    decimal Fee,
    string? Description,
    TransactionStatus Status,
    TransactionType Type,
    DateTime CreatedDate);
