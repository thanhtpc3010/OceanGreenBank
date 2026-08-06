using MediatR;
using ProjectService.Application.Common.Base;
using ProjectService.Application.Common.Interfaces;
using ProjectService.Application.Services.DTOs;
using ProjectService.Domain.Entity;
using ProjectService.Domain.Exceptions;

namespace ProjectService.Application.Services.Queries;

// ============================ REQUEST ============================
public sealed record GetUserQuery(string UserId) : BaseQuery<UserDto>;

public sealed record GetUsersQuery : BaseQuery<IReadOnlyList<UserDto>>;

// ============================ SERVICE INTERFACE ============================
/// <summary>
/// Interface của User Query Service — kế thừa IQueryService để dùng chung GetAsync.
/// </summary>
public interface IUserQueryService : IQueryService<GetUserQuery, UserDto> { }

// ============================ HANDLER (READ SIDE) ============================
/// <summary>
/// Toàn bộ read operations của User domain: GetAsync, GetAllAsync.
/// </summary>
public class UserQuery :
    IUserQueryService,
    IRequestHandler<GetUserQuery, UserDto>,
    IRequestHandler<GetUsersQuery, IReadOnlyList<UserDto>>
{
    private readonly IReadRepository<User> _userRepository;

    public UserQuery(IReadRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    // --- MediatR dispatch ---
    public Task<UserDto> Handle(GetUserQuery request, CancellationToken ct)
        => GetAsync(request, ct);

    public async Task<IReadOnlyList<UserDto>> Handle(GetUsersQuery request, CancellationToken ct)
        => await GetAllAsync(ct);

    // --- Operations ---
    public async Task<UserDto> GetAsync(GetUserQuery request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, ct)
            ?? throw new NotFoundException(nameof(User), request.UserId);

        return ToDto(user);
    }

    public async Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken ct)
    {
        var users = await _userRepository.ListAsync(ct);
        return users.Select(ToDto).ToList();
    }

    private static UserDto ToDto(User user) => new(
        user.Id,
        user.FullName,
        user.Email,
        user.Phone,
        user.IdentityCard,
        user.DateOfBirth,
        user.Address,
        user.IsActive,
        user.CreatedDate);
}
