namespace ProjectService.Application.Features.Accounts.DTOs;

/// <summary>
/// DTO dùng để tạo tài khoản mới.
/// </summary>
public sealed record CreateAccountRequest(
    string UserId,
    string Currency = "VND");
