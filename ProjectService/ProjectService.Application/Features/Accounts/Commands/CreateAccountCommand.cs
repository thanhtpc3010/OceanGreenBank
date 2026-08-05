using MediatR;
using ProjectService.Application.Common.Interfaces;
using ProjectService.Application.Features.Accounts.DTOs;
using ProjectService.Domain.Entities;
using ProjectService.Domain.Enums;
using ProjectService.Domain.Events;

namespace ProjectService.Application.Features.Accounts.Commands;

public sealed record CreateAccountCommand(
    Guid UserId,
    AccountType Type,
    string Currency) : IRequest<AccountDto>;

public sealed class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, AccountDto>
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Account> _accountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public CreateAccountCommandHandler(
        IRepository<User> userRepository,
        IRepository<Account> accountRepository,
        IUnitOfWork unitOfWork,
        IMediator mediator)
    {
        _userRepository = userRepository;
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<AccountDto> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            throw new Domain.Exceptions.DomainException("Người dùng không tồn tại.");

        var account = new Account
        {
            UserId = request.UserId,
            AccountNumber = GenerateAccountNumber(),
            Type = request.Type,
            Currency = request.Currency,
            Balance = 0m
        };

        await _accountRepository.AddAsync(account, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Phát domain event sau khi lưu thành công.
        await _mediator.Publish(new AccountCreatedEvent(account.Id, account.AccountNumber), cancellationToken);

        return new AccountDto(
            account.Id,
            account.UserId,
            account.AccountNumber,
            account.Type,
            account.Balance,
            account.Currency,
            account.IsActive,
            account.CreatedAtUtc);
    }

    private static string GenerateAccountNumber()
        => Random.Shared.Next(100000000, 999999999).ToString();
}
