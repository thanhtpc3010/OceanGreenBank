namespace ProjectService.Application.Features.Users.DTOs;

/// <summary>
/// DTO dùng để cập nhật thông tin người dùng.
/// </summary>
public sealed record UpdateUserRequest(
    string? FullName,
    string? PhoneNumber,
    bool? IsActive);
