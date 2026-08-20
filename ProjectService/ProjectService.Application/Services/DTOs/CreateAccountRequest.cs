using ProjectService.Domain.Enum;

namespace ProjectService.Application.Services.DTOs;

/// <summary>
/// DTO dùng để tạo tài khoản mới (API binding).
/// </summary>
public sealed record CreateAccountRequest(
    string UserId,
    string Currency = "VND",
    AccountType Type = AccountType.Normal,
    int? SavingsTermMonths = null,
    decimal? InterestRate = null,
    DateTime? SavingsStartDate = null);
