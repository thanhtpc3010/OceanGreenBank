namespace ProjectService.Application.Features.Accounts.DTOs;

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
    DateTime CreatedDate);
