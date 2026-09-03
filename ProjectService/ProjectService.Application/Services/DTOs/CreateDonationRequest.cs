namespace ProjectService.Application.Services.DTOs;

/// <summary>
/// DTO dùng để tạo giao dịch ủng hộ (API binding).
/// </summary>
public sealed record CreateDonationRequest(
    string FromAccountId,
    string FundId,
    string BankCode,
    decimal Amount,
    string? Message = null,
    string? TransactionPassword = null);
