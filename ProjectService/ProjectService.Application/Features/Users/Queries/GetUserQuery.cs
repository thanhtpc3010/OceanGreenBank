using MediatR;
using ProjectService.Application.Common.Base;
using ProjectService.Application.Common.Exceptions;
using ProjectService.Application.Common.Interfaces;
using ProjectService.Application.Features.Users.DTOs;
using ProjectService.Domain.Entity;

namespace ProjectService.Application.Features.Users.Queries;

public sealed record GetUserQuery(string UserId) : BaseQuery<UserDto>;

public sealed class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserDto>
{
    private readonly IRepository<User> _userRepository;

    public GetUserQueryHandler(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(User), request.UserId);

        return new UserDto(
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
}
