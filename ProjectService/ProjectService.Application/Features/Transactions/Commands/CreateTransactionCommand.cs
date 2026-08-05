using MediatR;
using ProjectService.Application.Common.Interfaces;
using ProjectService.Application.Features.Transactions.DTOs;
using ProjectService.Domain.Entities;
using ProjectService.Domain.Enums;

namespace ProjectService.Application.Features.Transactions.Commands;

public sealed record CreateTransactionCommand(
    Guid AccountId,
    TransactionType Type,
    decimal Amount,
    string Description) : IRequest<TransactionDto>;

public sealed class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, TransactionDto>
{
    private readonly IRepository<Account> _accountRepository;
    private readonly IRepository<Transaction> _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTransactionCommandHandler(
        IRepository<Account> accountRepository,
        IRepository<Transaction> transactionRepository,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TransactionDto> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByIdAsync(request.AccountId, cancellationToken)
            ?? throw new Domain.Exceptions.DomainException("Tài khoản không tồn tại.");

        if (request.Amount <= 0)
            throw new Domain.Exceptions.DomainException("Số tiền giao dịch phải lớn hơn 0.");

        if (request.Type == TransactionType.Withdrawal && account.Balance < request.Amount)
            throw new Domain.Exceptions.DomainException("Số dư không đủ để thực hiện giao dịch.");

        var balanceAfter = request.Type switch
        {
            TransactionType.Deposit => account.Balance + request.Amount,
            _ => account.Balance - request.Amount
        };

        var transaction = new Transaction
        {
            AccountId = request.AccountId,
            Type = request.Type,
            Status = TransactionStatus.Completed,
            Amount = request.Amount,
            BalanceAfter = balanceAfter,
            Description = request.Description
        };

        account.Balance = balanceAfter;

        _accountRepository.Update(account);
        await _transactionRepository.AddAsync(transaction, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new TransactionDto(
            transaction.Id,
            transaction.AccountId,
            transaction.Type,
            transaction.Status,
            transaction.Amount,
            transaction.BalanceAfter,
            transaction.Description,
            transaction.CreatedAtUtc);
    }
}
