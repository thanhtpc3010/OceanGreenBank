namespace ProjectService.Application.Features.Users.DTOs;

/// <summary>
/// DTO trả về thông tin người dùng.
/// </summary>
public sealed record UserDto(
    string Id,
    string FullName,
    string Email,
    string Phone,
    string IdentityCard,
    DateTime DateOfBirth,
    string? Address,
    bool IsActive,
    DateTime CreatedDate);
