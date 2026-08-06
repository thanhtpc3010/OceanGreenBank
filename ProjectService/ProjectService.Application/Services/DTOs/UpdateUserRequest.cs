namespace ProjectService.Application.Services.DTOs;

/// <summary>
/// DTO dùng để cập nhật thông tin người dùng.
/// </summary>
public sealed record UpdateUserRequest(
    string? FullName,
    string? Phone,
    string? Address,
    bool? IsActive);
