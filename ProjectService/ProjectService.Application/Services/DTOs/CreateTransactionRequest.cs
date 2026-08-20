using ProjectService.Domain.Enum;

namespace ProjectService.Application.Services.DTOs;

/// <summary>
/// DTO dùng để tạo giao dịch mới.
/// </summary>
public sealed record CreateTransactionRequest(
    string FromAccountId,
    TransactionType Type,
    decimal Amount,
    string? Description = null,
    string? ToAccountId = null,
    string? ReceiverAccount = null,
    string? ReceiverName = null,
    string? ReceiverBankCode = null,
    TransactionCategory Category = TransactionCategory.Other,
    bool IsEarlyWithdrawal = false);
