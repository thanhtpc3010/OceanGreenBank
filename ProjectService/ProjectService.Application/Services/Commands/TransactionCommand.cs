using MediatR;
using ProjectService.Application.Common.Base;
using ProjectService.Application.Common.Interfaces;
using ProjectService.Application.Services.DTOs;
using ProjectService.Domain.Entity;
using ProjectService.Domain.Enum;
using ProjectService.Domain.Exceptions;

namespace ProjectService.Application.Services.Commands;

// ============================ REQUEST ============================
public sealed record CreateTransactionCommand(
    string FromAccountId,
    TransactionType Type,
    decimal Amount,
    string? Description,
    string? ToAccountId,
    string? ReceiverAccount,
    string? ReceiverName,
    string? ReceiverBankCode,
    TransactionCategory Category = TransactionCategory.Other,
    bool IsEarlyWithdrawal = false) : BaseCommand<TransactionDto>;

public sealed record CancelTransactionCommand(string TransactionId) : BaseCommand<Unit>;

// ============================ SERVICE INTERFACE ============================
/// <summary>
/// Interface của Transaction Command Service.
/// Giao dịch tài chính không cho phép Update/Delete nên không kế thừa ICommandService đầy đủ.
/// </summary>
public interface ITransactionCommandService
{
    Task<TransactionDto> CreateAsync(CreateTransactionCommand request, CancellationToken cancellationToken = default);

    Task CancelAsync(CancelTransactionCommand request, CancellationToken cancellationToken = default);
}

// ============================ HANDLER (WRITE SIDE) ============================
/// <summary>
/// Toàn bộ write operations của Transaction domain: Create, Cancel.
/// </summary>
public class TransactionCommand :
    ITransactionCommandService,
    IRequestHandler<CreateTransactionCommand, TransactionDto>,
    IRequestHandler<CancelTransactionCommand, Unit>
{
    private readonly IWriteRepository<Account> _accountRepository;
    private readonly IWriteRepository<Transaction> _transactionRepository;
    private readonly IWriteRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TransactionCommand(
        IWriteRepository<Account> accountRepository,
        IWriteRepository<Transaction> transactionRepository,
        IWriteRepository<User> userRepository,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    // --- MediatR dispatch ---
    public Task<TransactionDto> Handle(CreateTransactionCommand request, CancellationToken ct)
        => CreateAsync(request, ct);

    public async Task<Unit> Handle(CancelTransactionCommand request, CancellationToken ct)
    {
        await CancelAsync(request, ct);
        return Unit.Value;
    }

    // --- Operations ---
    public async Task<TransactionDto> CreateAsync(CreateTransactionCommand request, CancellationToken ct)
    {
        var fromAccount = await _accountRepository.GetByIdAsync(request.FromAccountId, ct)
            ?? throw new DomainException("Tài khoản gửi không tồn tại.");

        if (!fromAccount.IsActive)
            throw new DomainException("Tài khoản gửi đã bị khóa.");

        // User sở hữu tài khoản bị khóa → không được thực hiện giao dịch.
        var owner = await _userRepository.GetByIdAsync(fromAccount.UserId, ct);
        if (owner is not null && !owner.IsActive)
            throw new DomainException("Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên.");

        if (request.Amount <= 0)
            throw new DomainException("Số tiền giao dịch phải lớn hơn 0.");

        // ===== RULES TÀI KHOẢN TIẾT KIỆM =====
        if (fromAccount.Type == AccountType.Savings)
        {
            var now = DateTime.UtcNow;
            var matured = fromAccount.SavingsMaturityDate.HasValue
                          && now >= fromAccount.SavingsMaturityDate.Value;

            if (!matured && !request.IsEarlyWithdrawal)
                throw new DomainException(
                    "Tài khoản tiết kiệm chưa đáo hạn. Chỉ được rút khi hết kỳ hạn, hoặc dùng rút trước hạn (mất toàn bộ lãi chu kỳ).");

            if (matured)
            {
                // Đáo hạn: cộng lãi suất kỳ hạn vào số dư trước khi xử lý rút.
                var interest = fromAccount.Balance
                    * (fromAccount.InterestRate ?? 0) / 100m
                    * (fromAccount.SavingsTermMonths ?? 1) / 12m;
                fromAccount.Balance += Math.Round(interest, 0, MidpointRounding.AwayFromZero);
            }
            // Rút trước hạn (IsEarlyWithdrawal): KHÔNG cộng lãi → mất toàn bộ lãi chu kỳ.
        }

        var fee = request.Type == TransactionType.InterbankTransfer ? 5000m : 0m;
        var totalDebit = request.Amount + fee;

        if (fromAccount.Balance < totalDebit)
            throw new DomainException("Số dư không đủ để thực hiện giao dịch.");

        Account? toAccount = null;
        if (!string.IsNullOrEmpty(request.ToAccountId))
        {
            toAccount = await _accountRepository.GetByIdAsync(request.ToAccountId!, ct);
            if (toAccount is null)
                throw new DomainException("Tài khoản nhận không tồn tại.");
            if (!toAccount.IsActive)
                throw new DomainException("Tài khoản nhận đã bị khóa.");
        }

        // Ghi Nợ tài khoản gửi.
        fromAccount.Balance -= totalDebit;
        _accountRepository.Update(fromAccount);

        // Tài khoản tiết kiệm: nếu còn dư sau khi rút → gia hạn kỳ hạn mới.
        if (fromAccount.Type == AccountType.Savings && fromAccount.Balance > 0)
        {
            var now = DateTime.UtcNow;
            fromAccount.SavingsStartDate = now;
            fromAccount.SavingsMaturityDate = now.AddMonths(fromAccount.SavingsTermMonths ?? 1);
        }

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
            Type = request.Type,
            Category = request.Category
        };

        await _transactionRepository.AddAsync(transaction, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return ToDto(transaction);
    }

    /// <summary>Hủy giao dịch đang ở trạng thái Pending.</summary>
    public async Task CancelAsync(CancelTransactionCommand request, CancellationToken ct)
    {
        var transaction = await _transactionRepository.GetByIdAsync(request.TransactionId, ct)
            ?? throw new DomainException("Giao dịch không tồn tại.");

        if (transaction.Status != TransactionStatus.Pending)
            throw new DomainException("Chỉ có thể hủy giao dịch đang ở trạng thái chờ xử lý.");

        transaction.Status = TransactionStatus.Failed;
        transaction.LastModifiedDate = DateTime.UtcNow;
        transaction.LastModifiedBy = null;

        _transactionRepository.Update(transaction);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    private static string GenerateTransactionCode()
        => $"TXN{DateTime.Now:yyyyMMddHHmmss}{Random.Shared.Next(100, 999)}";

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
        t.Category,
        t.CreatedDate);
}
