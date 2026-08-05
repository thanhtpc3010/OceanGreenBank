using MediatR;
using ProjectService.Application.Common.Exceptions;
using ProjectService.Application.Common.Interfaces;
using ProjectService.Application.Features.Accounts.DTOs;
using ProjectService.Domain.Entities;

namespace ProjectService.Application.Features.Accounts.Queries;

public sealed record GetAccountQuery(Guid AccountId) : IRequest<AccountDto>;

public sealed class GetAccountQueryHandler : IRequestHandler<GetAccountQuery, AccountDto>
{
    private readonly IRepository<Account> _accountRepository;

    public GetAccountQueryHandler(IRepository<Account> accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<AccountDto> Handle(GetAccountQuery request, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByIdAsync(request.AccountId, cancellationToken)
            ?? throw new NotFoundException(nameof(Account), request.AccountId);

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
}
