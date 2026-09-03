namespace ProjectService.Application.Services.DTOs;

/// <summary>DTO giao dịch nạp tiền qua ví (MoMo/ZaloPay).</summary>
public record PaymentDto(
    string Id,
    string UserId,
    int Provider,
    string ProviderName,
    string AccountId,
    string AccountNumber,
    decimal Amount,
    string? OrderCode,
    int Status,
    string StatusName,
    string? Description,
    DateTime? CompletedDate,
    DateTime CreatedDate,
    string? MockPaymentUrl);
