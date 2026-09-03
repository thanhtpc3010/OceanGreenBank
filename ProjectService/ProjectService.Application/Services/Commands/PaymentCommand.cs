using MediatR;
using ProjectService.Application.Common.Base;
using ProjectService.Application.Common.Interfaces;
using ProjectService.Application.Services.DTOs;
using ProjectService.Domain.Entity;
using ProjectService.Domain.Enum;
using ProjectService.Domain.Exceptions;

namespace ProjectService.Application.Services.Commands;

// ============================ REQUEST ============================
/// <summary>Tạo đơn nạp tiền qua ví (mock).</summary>
public sealed record CreatePaymentCommand(
    string UserId,
    PaymentProvider Provider,
    string AccountId,
    decimal Amount,
    string? Description) : BaseCommand<PaymentDto>;

/// <summary>Ví xác nhận thanh toán thành công (mock IPN callback) → credit tiền.</summary>
public sealed record ConfirmPaymentCommand(string PaymentId) : BaseCommand<PaymentDto>;

/// <summary>Người dùng hủy / ví từ chối thanh toán.</summary>
public sealed record CancelPaymentCommand(string PaymentId) : BaseCommand<PaymentDto>;

// ============================ SERVICE INTERFACE ============================
public interface IPaymentCommandService
{
    Task<PaymentDto> CreateAsync(CreatePaymentCommand request, CancellationToken cancellationToken = default);

    Task<PaymentDto> ConfirmAsync(ConfirmPaymentCommand request, CancellationToken cancellationToken = default);

    Task<PaymentDto> CancelAsync(CancelPaymentCommand request, CancellationToken cancellationToken = default);
}

// ============================ HANDLER (WRITE SIDE) ============================
/// <summary>
/// Write operations của Payment domain: tạo đơn, xác nhận (credit tiền), hủy.
/// </summary>
public class PaymentCommand :
    IPaymentCommandService,
    IRequestHandler<CreatePaymentCommand, PaymentDto>,
    IRequestHandler<ConfirmPaymentCommand, PaymentDto>,
    IRequestHandler<CancelPaymentCommand, PaymentDto>
{
    private readonly IWriteRepository<Payment> _paymentRepository;
    private readonly IWriteRepository<Account> _accountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PaymentCommand(
        IWriteRepository<Payment> paymentRepository,
        IWriteRepository<Account> accountRepository,
        IUnitOfWork unitOfWork)
    {
        _paymentRepository = paymentRepository;
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }

    // --- MediatR dispatch ---
    public Task<PaymentDto> Handle(CreatePaymentCommand request, CancellationToken ct)
        => CreateAsync(request, ct);

    public Task<PaymentDto> Handle(ConfirmPaymentCommand request, CancellationToken ct)
        => ConfirmAsync(request, ct);

    public Task<PaymentDto> Handle(CancelPaymentCommand request, CancellationToken ct)
        => CancelAsync(request, ct);

    // --- Operations ---
    public async Task<PaymentDto> CreateAsync(CreatePaymentCommand request, CancellationToken ct)
    {
        if (request.Amount <= 0)
            throw new DomainException("Số tiền nạp phải lớn hơn 0.");

        var account = await _accountRepository.GetByIdAsync(request.AccountId, ct)
            ?? throw new DomainException("Tài khoản nhận tiền không tồn tại.");

        if (account.UserId != request.UserId)
            throw new DomainException("Bạn không sở hữu tài khoản này.");

        if (!account.IsActive)
            throw new DomainException("Tài khoản nhận tiền đã bị khóa.");

        var now = DateTime.UtcNow;
        var payment = new Payment
        {
            Id = Guid.NewGuid().ToString("N"),
            CreatedDate = now,
            CreatedBy = request.UserId,
            UserId = request.UserId,
            Provider = request.Provider,
            AccountId = account.Id,
            AccountNumber = account.AccountNumber,
            Amount = request.Amount,
            OrderCode = GenerateOrderCode(request.Provider),
            Status = PaymentStatus.Pending,
            Description = request.Description
        };

        await _paymentRepository.AddAsync(payment, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return ToDto(payment);
    }

    public async Task<PaymentDto> ConfirmAsync(ConfirmPaymentCommand request, CancellationToken ct)
    {
        var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, ct)
            ?? throw new DomainException("Đơn thanh toán không tồn tại.");

        if (payment.Status != PaymentStatus.Pending)
            throw new DomainException("Đơn thanh toán không còn ở trạng thái chờ xác nhận.");

        // Credit tiền vào tài khoản đích (giống AutoEarn: cập nhật trực tiếp số dư).
        var account = await _accountRepository.GetByIdAsync(payment.AccountId, ct);
        if (account is not null && account.IsActive)
        {
            account.Balance += payment.Amount;
            _accountRepository.Update(account);
        }

        payment.Status = PaymentStatus.Success;
        payment.CompletedDate = DateTime.UtcNow;
        payment.LastModifiedDate = DateTime.UtcNow;
        payment.LastModifiedBy = payment.UserId;

        _paymentRepository.Update(payment);
        await _unitOfWork.SaveChangesAsync(ct);

        return ToDto(payment);
    }

    public async Task<PaymentDto> CancelAsync(CancelPaymentCommand request, CancellationToken ct)
    {
        var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, ct)
            ?? throw new DomainException("Đơn thanh toán không tồn tại.");

        if (payment.Status != PaymentStatus.Pending)
            throw new DomainException("Đơn thanh toán không còn ở trạng thái chờ xác nhận.");

        payment.Status = PaymentStatus.Failed;
        payment.CompletedDate = DateTime.UtcNow;
        payment.LastModifiedDate = DateTime.UtcNow;
        payment.LastModifiedBy = payment.UserId;

        _paymentRepository.Update(payment);
        await _unitOfWork.SaveChangesAsync(ct);

        return ToDto(payment);
    }

    // --- Helpers ---
    private static string GenerateOrderCode(PaymentProvider provider)
    {
        var prefix = provider == PaymentProvider.Momo ? "MOMO" : "ZLP";
        var stamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var rand = Random.Shared.Next(10000, 99999).ToString();
        return $"{prefix}{stamp}{rand}";
    }

    private static PaymentDto ToDto(Payment p) => new(
        p.Id,
        p.UserId,
        (int)p.Provider,
        ProviderName(p.Provider),
        p.AccountId,
        p.AccountNumber,
        p.Amount,
        p.OrderCode,
        (int)p.Status,
        StatusName(p.Status),
        p.Description,
        p.CompletedDate,
        p.CreatedDate,
        $"/wallet-pay/{p.Id}");

    private static string ProviderName(PaymentProvider provider)
        => provider == PaymentProvider.Momo ? "MoMo" : "ZaloPay";

    private static string StatusName(PaymentStatus status)
        => status switch
        {
            PaymentStatus.Pending => "Chờ thanh toán",
            PaymentStatus.Success => "Thành công",
            PaymentStatus.Failed => "Đã hủy",
            _ => "Không xác định"
        };
}
