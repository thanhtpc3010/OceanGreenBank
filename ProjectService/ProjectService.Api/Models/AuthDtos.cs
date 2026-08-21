namespace ProjectService.Api.Models;

/// <summary>Yêu cầu đăng nhập.</summary>
public sealed record LoginRequest(string Email, string Password);

/// <summary>Yêu cầu đăng ký.</summary>
public sealed record RegisterRequest(
    string FullName,
    string Email,
    string Phone,
    string IdentityCard,
    DateTime DateOfBirth,
    string Password,
    string? Address);

/// <summary>Thông tin user trong response auth.</summary>
public sealed record AuthUserDto(
    string Id,
    string FullName,
    string Email,
    IReadOnlyList<string> Roles,
    IReadOnlyList<string> Permissions);

/// <summary>Response đăng nhập / đăng ký thành công.</summary>
public sealed record AuthResponse(
    string Token,
    DateTime ExpiresAt,
    AuthUserDto User);
