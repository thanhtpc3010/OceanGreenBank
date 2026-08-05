using MediatR;
using ProjectService.Application.Common.Base;
using ProjectService.Application.Common.Interfaces;
using ProjectService.Application.Features.Transactions.DTOs;
using ProjectService.Domain.Entity;
using ProjectService.Domain.Enum;
using ProjectService.Domain.Exceptions;

namespace ProjectService.Application.Features.Transactions.Commands;

public sealed record CreateTransactionCommand(
    string FromAccountId,
    TransactionType Type,
    decimal Amount,
    string? Description,
    string? ToAccountId,
    string? ReceiverAccount,
    string? ReceiverName,
    string? ReceiverBankCode) : BaseCommand<TransactionDto>;

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
        var fromAccount = await _accountRepository.GetByIdAsync(request.FromAccountId, cancellationToken)
            ?? throw new DomainException("Tài khoản gửi không tồn tại.");

        if (!fromAccount.IsActive)
            throw new DomainException("Tài khoản gửi đã bị khóa.");

        if (request.Amount <= 0)
            throw new DomainException("Số tiền giao dịch phải lớn hơn 0.");

        var fee = request.Type == TransactionType.InterbankTransfer ? 5000m : 0m;
        var totalDebit = request.Amount + fee;

        if (fromAccount.Balance < totalDebit)
            throw new DomainException("Số dư không đủ để thực hiện giao dịch.");

        Account? toAccount = null;
        if (!string.IsNullOrEmpty(request.ToAccountId))
        {
            toAccount = await _accountRepository.GetByIdAsync(request.ToAccountId!, cancellationToken);
            if (toAccount is null)
                throw new DomainException("Tài khoản nhận không tồn tại.");
            if (!toAccount.IsActive)
                throw new DomainException("Tài khoản nhận đã bị khóa.");
        }

        // Ghi Nợ tài khoản gửi.
        fromAccount.Balance -= totalDebit;
        _accountRepository.Update(fromAccount);

        // Ghi Có tài khoản nhận (nếu chuyển nội bộ).
        if (toAccount is not null)
        {
            toAccount.Balance += request.Amount;
            _accountRepository.Update(toAccount);
        }

        var transaction = new Transaction
        {
            Id = Guid.NewGuid().ToString("N"),
            CreatedDate = DateTime.UtcNow,
            CreatedBy = null,
            TransactionCode = GenerateTransactionCode(),
            FromAccountId = request.FromAccountId,
            ToAccountId = request.ToAccountId,
            ReceiverAccount = request.ReceiverAccount,
            ReceiverName = request.ReceiverName,
            ReceiverBankCode = request.ReceiverBankCode,
            Amount = request.Amount,
            Fee = fee,
            Description = request.Description,
            Status = TransactionStatus.Success,
            Type = request.Type
        };

        await _transactionRepository.AddAsync(transaction, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new TransactionDto(
            transaction.Id,
            transaction.TransactionCode,
            transaction.FromAccountId,
            transaction.ToAccountId,
            transaction.ReceiverAccount,
            transaction.ReceiverName,
            transaction.ReceiverBankCode,
            transaction.Amount,
            transaction.Fee,
            transaction.Description,
            transaction.Status,
            transaction.Type,
            transaction.CreatedDate);
    }

    private static string GenerateTransactionCode()
        => $"TXN{DateTime.Now:yyyyMMddHHmmss}{Random.Shared.Next(100, 999)}";
}
