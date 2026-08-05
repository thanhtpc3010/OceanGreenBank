namespace ProjectService.Application.Features.Users.DTOs;

/// <summary>
/// DTO trả về thông tin người dùng.
/// </summary>
public sealed record UserDto(
    Guid Id,
    string FullName,
    string Email,
    string PhoneNumber,
    bool IsActive,
    DateTime CreatedAtUtc);
