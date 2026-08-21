using MediatR;
using ProjectService.Application.Common.Base;
using ProjectService.Application.Common.Interfaces;
using ProjectService.Application.Services.DTOs;
using ProjectService.Domain.Entity;
using ProjectService.Domain.Exceptions;

namespace ProjectService.Application.Services.Commands;

// ============================ REQUEST ============================
public sealed record CreateUserCommand(
    string FullName,
    string Email,
    string Phone,
    string IdentityCard,
    DateTime DateOfBirth,
    string Password,
    string? Address) : BaseCommand<UserDto>;

public sealed record UpdateUserCommand(
    string UserId,
    string? FullName,
    string? Phone,
    string? Address,
    bool? IsActive) : BaseCommand<UserDto>;

public sealed record DeleteUserCommand(string UserId) : BaseCommand<Unit>;

// ============================ SERVICE INTERFACE ============================
/// <summary>
/// Interface của User Command Service — kế thừa ICommandService để dùng chung Create/Update/Delete.
/// </summary>
public interface IUserCommandService :
    ICommandService<CreateUserCommand, UpdateUserCommand, DeleteUserCommand, UserDto> { }

// ============================ HANDLER (WRITE SIDE) ============================
/// <summary>
/// Toàn bộ write operations của User domain: Create, Update, Delete.
/// </summary>
public class UserCommand :
    IUserCommandService,
    IRequestHandler<CreateUserCommand, UserDto>,
    IRequestHandler<UpdateUserCommand, UserDto>,
    IRequestHandler<DeleteUserCommand, Unit>
{
    private readonly IWriteRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserCommand(IWriteRepository<User> userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    // --- MediatR dispatch ---
    public Task<UserDto> Handle(CreateUserCommand request, CancellationToken ct)
        => CreateAsync(request, ct);

    public Task<UserDto> Handle(UpdateUserCommand request, CancellationToken ct)
        => UpdateAsync(request, ct);

    public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken ct)
    {
        await DeleteAsync(request, ct);
        return Unit.Value;
    }

    // --- Operations ---
    public async Task<UserDto> CreateAsync(CreateUserCommand request, CancellationToken ct)
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
            // Npgsql yêu cầu DateTime Kind=Utc cho cột timestamptz — request từ JSON là Unspecified.
            DateOfBirth = DateTime.SpecifyKind(request.DateOfBirth, DateTimeKind.Utc),
            PasswordHash = request.Password,
            Address = request.Address
        };

        await _userRepository.AddAsync(user, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return ToDto(user);
    }

    public async Task<UserDto> UpdateAsync(UpdateUserCommand request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, ct)
            ?? throw new NotFoundException(nameof(User), request.UserId);

        user.FullName = request.FullName ?? user.FullName;
        user.Phone = request.Phone ?? user.Phone;
        user.Address = request.Address ?? user.Address;
        user.IsActive = request.IsActive ?? user.IsActive;
        user.LastModifiedDate = DateTime.UtcNow;
        user.LastModifiedBy = null;

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(ct);

        return ToDto(user);
    }

    public async Task DeleteAsync(DeleteUserCommand request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, ct)
            ?? throw new NotFoundException(nameof(User), request.UserId);

        _userRepository.Remove(user);
        await _unitOfWork.SaveChangesAsync(ct);
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
