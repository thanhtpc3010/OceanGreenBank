using MediatR;
using ProjectService.Application.Common.Interfaces;
using ProjectService.Application.Features.Users.DTOs;
using ProjectService.Domain.Entities;

namespace ProjectService.Application.Features.Users.Commands;

public sealed record CreateUserCommand(
    string FullName,
    string Email,
    string PhoneNumber,
    string Password) : IRequest<UserDto>;

public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly IRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(IRepository<User> userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // TODO: hash password bằng BCrypt / ASP.NET Core Identity trước khi lưu.
        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            PasswordHash = request.Password
        };

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UserDto(
            user.Id,
            user.FullName,
            user.Email,
            user.PhoneNumber,
            user.IsActive,
            user.CreatedAtUtc);
    }
}
