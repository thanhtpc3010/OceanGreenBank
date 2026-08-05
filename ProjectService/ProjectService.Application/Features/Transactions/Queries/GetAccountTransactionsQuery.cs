using MediatR;
using ProjectService.Application.Common.Interfaces;
using ProjectService.Application.Features.Transactions.DTOs;
using ProjectService.Domain.Entities;

namespace ProjectService.Application.Features.Transactions.Queries;

public sealed record GetAccountTransactionsQuery(Guid AccountId) : IRequest<IReadOnlyList<TransactionDto>>;

public sealed class GetAccountTransactionsQueryHandler
    : IRequestHandler<GetAccountTransactionsQuery, IReadOnlyList<TransactionDto>>
{
    private readonly IRepository<Transaction> _transactionRepository;

    public GetAccountTransactionsQueryHandler(IRepository<Transaction> transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<IReadOnlyList<TransactionDto>> Handle(
        GetAccountTransactionsQuery request,
        CancellationToken cancellationToken)
    {
        var transactions = await _transactionRepository.FindAsync(
            t => t.AccountId == request.AccountId,
            cancellationToken);

        return transactions
            .OrderByDescending(t => t.CreatedAtUtc)
            .Select(t => new TransactionDto(
                t.Id,
                t.AccountId,
                t.Type,
                t.Status,
                t.Amount,
                t.BalanceAfter,
                t.Description,
                t.CreatedAtUtc))
            .ToList();
    }
}
