using MediatR;
using ProjectService.Application.Common.Base;
using ProjectService.Application.Common.Interfaces;
using ProjectService.Application.Services.DTOs;
using ProjectService.Domain.Entity;
using ProjectService.Domain.Exceptions;

namespace ProjectService.Application.Services.Queries;

// ============================ REQUEST ============================
public sealed record GetAccountQuery(string AccountId) : BaseQuery<AccountDto>;

public sealed record GetAccountsByUserQuery(string UserId) : BaseQuery<IReadOnlyList<AccountDto>>;

// ============================ SERVICE INTERFACE ============================
/// <summary>
/// Interface của Account Query Service — kế thừa IQueryService để dùng chung GetAsync.
/// </summary>
public interface IAccountQueryService : IQueryService<GetAccountQuery, AccountDto> { }

// ============================ HANDLER (READ SIDE) ============================
/// <summary>
/// Toàn bộ read operations của Account domain: GetAsync, GetByUserAsync.
/// </summary>
public class AccountQuery :
    IAccountQueryService,
    IRequestHandler<GetAccountQuery, AccountDto>,
    IRequestHandler<GetAccountsByUserQuery, IReadOnlyList<AccountDto>>
{
    private readonly IReadRepository<Account> _accountRepository;

    public AccountQuery(IReadRepository<Account> accountRepository)
    {
        _accountRepository = accountRepository;
    }

    // --- MediatR dispatch ---
    public Task<AccountDto> Handle(GetAccountQuery request, CancellationToken ct)
        => GetAsync(request, ct);

    public async Task<IReadOnlyList<AccountDto>> Handle(GetAccountsByUserQuery request, CancellationToken ct)
        => await GetByUserAsync(request.UserId, ct);

    // --- Operations ---
    public async Task<AccountDto> GetAsync(GetAccountQuery request, CancellationToken ct)
    {
        var account = await _accountRepository.GetByIdAsync(request.AccountId, ct)
            ?? throw new NotFoundException(nameof(Account), request.AccountId);

        return ToDto(account);
    }

    public async Task<IReadOnlyList<AccountDto>> GetByUserAsync(string userId, CancellationToken ct)
    {
        var accounts = await _accountRepository.FindAsync(a => a.UserId == userId, ct);
        return accounts.Select(ToDto).ToList();
    }

    private static AccountDto ToDto(Account account) => new(
        account.Id,
        account.UserId,
        account.AccountNumber,
        account.Balance,
        account.Currency,
        account.IsActive,
        account.CreatedDate);
}
