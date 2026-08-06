namespace ProjectService.Application.Services.DTOs;

/// <summary>
/// DTO dùng để tạo người dùng mới (API binding).
/// </summary>
public sealed record CreateUserRequest(
    string FullName,
    string Email,
    string Phone,
    string IdentityCard,
    DateTime DateOfBirth,
    string Password,
    string? Address);
