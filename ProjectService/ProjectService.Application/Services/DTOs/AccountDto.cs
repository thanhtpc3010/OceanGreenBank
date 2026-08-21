using ProjectService.Domain.Enum;

namespace ProjectService.Application.Services.DTOs;

/// <summary>
/// DTO trả về thông tin tài khoản ngân hàng.
/// </summary>
public sealed record AccountDto(
    string Id,
    string UserId,
    string AccountNumber,
    decimal Balance,
    string Currency,
    bool IsActive,
    AccountType Type,
    int? SavingsTermMonths,
    decimal? InterestRate,
    DateTime? SavingsStartDate,
    DateTime? SavingsMaturityDate,
    DateTime CreatedDate);
