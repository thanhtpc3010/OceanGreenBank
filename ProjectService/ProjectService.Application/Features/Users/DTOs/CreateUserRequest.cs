namespace ProjectService.Application.Features.Users.DTOs;

/// <summary>
/// DTO dùng để tạo người dùng mới (API binding).
/// </summary>
public sealed record CreateUserRequest(
    string FullName,
    string Email,
    string PhoneNumber,
    string Password);
