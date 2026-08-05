using MediatR;
using ProjectService.Application.Common.Base;
using ProjectService.Application.Common.Interfaces;
using ProjectService.Application.Features.Users.DTOs;
using ProjectService.Domain.Entity;

namespace ProjectService.Application.Features.Users.Commands;

public sealed record CreateUserCommand(
    string FullName,
    string Email,
    string Phone,
    string IdentityCard,
    DateTime DateOfBirth,
    string Password,
    string? Address) : BaseCommand<UserDto>;

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
            Id = Guid.NewGuid().ToString("N"),
            CreatedDate = DateTime.UtcNow,
            CreatedBy = null,
            FullName = request.FullName,
            Email = request.Email,
            Phone = request.Phone,
            IdentityCard = request.IdentityCard,
            DateOfBirth = request.DateOfBirth,
            PasswordHash = request.Password,
            Address = request.Address
        };

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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
