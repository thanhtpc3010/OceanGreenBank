using ProjectService.Domain.Enums;

namespace ProjectService.Application.Features.Accounts.DTOs;

/// <summary>
/// DTO trả về thông tin tài khoản ngân hàng.
/// </summary>
public sealed record AccountDto(
    Guid Id,
    Guid UserId,
    string AccountNumber,
    AccountType Type,
    decimal Balance,
    string Currency,
    bool IsActive,
    DateTime CreatedAtUtc);
