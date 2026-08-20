namespace ProjectService.Application.Services.DTOs;

/// <summary>Vai trò của user.</summary>
public sealed record RoleDto(
    string Id,
    string RoleName,
    string? Code,
    string? Description);

/// <summary>
/// Quyền & vai trò của một user — dùng cho phân quyền (RBAC).
/// </summary>
public sealed record UserPermissionsDto(
    string UserId,
    string FullName,
    string Email,
    IReadOnlyList<RoleDto> Roles,
    IReadOnlyList<string> PermissionCodes);
