using MediatR;
using ProjectService.Application.Common.Base;
using ProjectService.Application.Common.Interfaces;
using ProjectService.Application.Features.Transactions.DTOs;
using ProjectService.Domain.Entity;

namespace ProjectService.Application.Features.Transactions.Queries;

public sealed record GetAccountTransactionsQuery(string AccountId)
    : BaseQuery<IReadOnlyList<TransactionDto>>;

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
            t => t.FromAccountId == request.AccountId || t.ToAccountId == request.AccountId,
            cancellationToken);

        return transactions
            .OrderByDescending(t => t.CreatedDate)
            .Select(t => new TransactionDto(
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
                t.CreatedDate))
            .ToList();
    }
}
