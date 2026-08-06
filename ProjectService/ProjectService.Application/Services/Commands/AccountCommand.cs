using MediatR;
using ProjectService.Application.Common.Base;
using ProjectService.Application.Common.Interfaces;
using ProjectService.Application.Services.DTOs;
using ProjectService.Domain.Entity;
using ProjectService.Domain.Events;
using ProjectService.Domain.Exceptions;

namespace ProjectService.Application.Services.Commands;

// ============================ REQUEST ============================
public sealed record CreateAccountCommand(string UserId, string Currency) : BaseCommand<AccountDto>;

public sealed record UpdateAccountCommand(
    string AccountId,
    string? Currency,
    bool? IsActive) : BaseCommand<AccountDto>;

public sealed record DeleteAccountCommand(string AccountId) : BaseCommand<Unit>;

// ============================ SERVICE INTERFACE ============================
/// <summary>
/// Interface của Account Command Service — kế thừa ICommandService để dùng chung Create/Update/Delete.
/// </summary>
public interface IAccountCommandService :
    ICommandService<CreateAccountCommand, UpdateAccountCommand, DeleteAccountCommand, AccountDto> { }

// ============================ HANDLER (WRITE SIDE) ============================
/// <summary>
/// Toàn bộ write operations của Account domain: Create, Update, Delete.
/// </summary>
public class AccountCommand :
    IAccountCommandService,
    IRequestHandler<CreateAccountCommand, AccountDto>,
    IRequestHandler<UpdateAccountCommand, AccountDto>,
    IRequestHandler<DeleteAccountCommand, Unit>
{
    private readonly IWriteRepository<User> _userRepository;
    private readonly IWriteRepository<Account> _accountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public AccountCommand(
        IWriteRepository<User> userRepository,
        IWriteRepository<Account> accountRepository,
        IUnitOfWork unitOfWork,
        IMediator mediator)
    {
        _userRepository = userRepository;
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    // --- MediatR dispatch ---
    public Task<AccountDto> Handle(CreateAccountCommand request, CancellationToken ct)
        => CreateAsync(request, ct);

    public Task<AccountDto> Handle(UpdateAccountCommand request, CancellationToken ct)
        => UpdateAsync(request, ct);

    public async Task<Unit> Handle(DeleteAccountCommand request, CancellationToken ct)
    {
        await DeleteAsync(request, ct);
        return Unit.Value;
    }

    // --- Operations ---
    public async Task<AccountDto> CreateAsync(CreateAccountCommand request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, ct);
        if (user is null)
            throw new DomainException("Người dùng không tồn tại.");

        var account = new Account
        {
            Id = Guid.NewGuid().ToString("N"),
            CreatedDate = DateTime.UtcNow,
            CreatedBy = null,
            UserId = request.UserId,
            AccountNumber = GenerateAccountNumber(),
            Currency = request.Currency,
            Balance = 0m
        };

        await _accountRepository.AddAsync(account, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        // Phát domain event sau khi lưu thành công.
        await _mediator.Publish(new AccountCreatedEvent(account.Id, account.AccountNumber), ct);

        return ToDto(account);
    }

    public async Task<AccountDto> UpdateAsync(UpdateAccountCommand request, CancellationToken ct)
    {
        var account = await _accountRepository.GetByIdAsync(request.AccountId, ct)
            ?? throw new NotFoundException(nameof(Account), request.AccountId);

        account.Currency = request.Currency ?? account.Currency;
        account.IsActive = request.IsActive ?? account.IsActive;
        account.LastModifiedDate = DateTime.UtcNow;
        account.LastModifiedBy = null;

        _accountRepository.Update(account);
        await _unitOfWork.SaveChangesAsync(ct);

        return ToDto(account);
    }

    /// <summary>Xóa tài khoản = khóa mềm (IsActive=false) để giữ toàn vẹn dữ liệu giao dịch.</summary>
    public async Task DeleteAsync(DeleteAccountCommand request, CancellationToken ct)
    {
        var account = await _accountRepository.GetByIdAsync(request.AccountId, ct)
            ?? throw new NotFoundException(nameof(Account), request.AccountId);

        account.IsActive = false;
        account.LastModifiedDate = DateTime.UtcNow;
        account.LastModifiedBy = null;

        _accountRepository.Update(account);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    private static string GenerateAccountNumber()
        => Random.Shared.Next(100000000, 999999999).ToString();

    private static AccountDto ToDto(Account account) => new(
        account.Id,
        account.UserId,
        account.AccountNumber,
        account.Balance,
        account.Currency,
        account.IsActive,
        account.CreatedDate);
}
