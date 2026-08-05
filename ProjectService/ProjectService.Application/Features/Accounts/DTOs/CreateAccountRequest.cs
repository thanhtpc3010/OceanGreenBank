using ProjectService.Domain.Enums;

namespace ProjectService.Application.Features.Accounts.DTOs;

/// <summary>
/// DTO dùng để tạo tài khoản mới.
/// </summary>
public sealed record CreateAccountRequest(
    Guid UserId,
    AccountType Type,
    string Currency = "VND");
