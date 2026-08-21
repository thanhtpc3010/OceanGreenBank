using MediatR;
using ProjectService.Application.Common.Base;
using ProjectService.Application.Common.Interfaces;
using ProjectService.Application.Services.DTOs;
using ProjectService.Domain.Entity;
using ProjectService.Domain.Exceptions;

namespace ProjectService.Application.Services.Queries;

/// <summary>
/// Query lấy vai trò & quyền của một user — phục vụ phân quyền (RBAC).
/// </summary>
public sealed record GetUserPermissionsQuery(string UserId) : BaseQuery<UserPermissionsDto>;

/// <summary>Handler lấy roles + permission codes của user.</summary>
public class UserPermissionsQuery : IRequestHandler<GetUserPermissionsQuery, UserPermissionsDto>
{
    private readonly IReadRepository<User> _userRepository;
    private readonly IReadRepository<Role> _roleRepository;

    public UserPermissionsQuery(
        IReadRepository<User> userRepository,
        IReadRepository<Role> roleRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }

    public async Task<UserPermissionsDto> Handle(GetUserPermissionsQuery request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, ct)
            ?? throw new NotFoundException(nameof(User), request.UserId);

        // Load các Role mà user thuộc, kèm Permissions của từng role.
        var roles = await _roleRepository.FindWithIncludesAsync(
            r => r.Users.Any(u => u.Id == request.UserId),
            r => r.Permissions);

        var roleDtos = roles
            .Select(r => new RoleDto(r.Id, r.RoleName, r.Code, r.Description))
            .ToList();

        var permissionCodes = roles
            .SelectMany(r => r.Permissions)
            .Select(p => p.Code)
            .Distinct()
            .OrderBy(c => c)
            .ToList();

        return new UserPermissionsDto(user.Id, user.FullName, user.Email, roleDtos, permissionCodes);
    }
}
