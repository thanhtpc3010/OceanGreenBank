namespace ProjectService.Application.Services.DTOs;

/// <summary>
/// DTO dùng để cập nhật tài khoản.
/// </summary>
public sealed record UpdateAccountRequest(
    string? Currency,
    bool? IsActive);
