using MediatR;
using ProjectService.Application.Common.Base;
using ProjectService.Application.Common.Interfaces;
using ProjectService.Application.Services.DTOs;
using ProjectService.Domain.Entity;

namespace ProjectService.Application.Services.Queries;

// ============================ REQUEST ============================
public sealed record GetAccountTransactionsQuery(string AccountId)
    : BaseQuery<IReadOnlyList<TransactionDto>>;

// ============================ SERVICE INTERFACE ============================
/// <summary>
/// Interface của Transaction Query Service — kế thừa IQueryService để dùng chung GetAsync.
/// </summary>
public interface ITransactionQueryService : IQueryService<GetAccountTransactionsQuery, IReadOnlyList<TransactionDto>> { }

// ============================ HANDLER (READ SIDE) ============================
/// <summary>
/// Toàn bộ read operations của Transaction domain: GetAsync (theo tài khoản).
/// </summary>
public class TransactionQuery : ITransactionQueryService,
    IRequestHandler<GetAccountTransactionsQuery, IReadOnlyList<TransactionDto>>
{
    private readonly IReadRepository<Transaction> _transactionRepository;

    public TransactionQuery(IReadRepository<Transaction> transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    // --- MediatR dispatch ---
    public Task<IReadOnlyList<TransactionDto>> Handle(GetAccountTransactionsQuery request, CancellationToken ct)
        => GetAsync(request, ct);

    // --- Operations ---
    public async Task<IReadOnlyList<TransactionDto>> GetAsync(GetAccountTransactionsQuery request, CancellationToken ct)
    {
        var transactions = await _transactionRepository.FindAsync(
            t => t.FromAccountId == request.AccountId || t.ToAccountId == request.AccountId,
            ct);

        return transactions
            .OrderByDescending(t => t.CreatedDate)
            .Select(ToDto)
            .ToList();
    }

    private static TransactionDto ToDto(Transaction t) => new(
        t.Id,
        t.TransactionCode,
        t.FromAccountId,
        t.ToAccountId,
        t.ReceiverAccount,
        t.ReceiverName,
        t.ReceiverBankCode,
        t.Amount,
        t.Fee,
        t.Description,
        t.Status,
        t.Type,
        t.CreatedDate);
}
