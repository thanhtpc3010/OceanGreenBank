using MediatR;
using ProjectService.Application.Common.Base;
using ProjectService.Application.Common.Interfaces;
using ProjectService.Application.Services.DTOs;
using ProjectService.Domain.Entity;
using ProjectService.Domain.Enum;
using ProjectService.Domain.Exceptions;

namespace ProjectService.Application.Services.Queries;

// ============================ REQUEST ============================
public sealed record GetPaymentsByUserQuery(string UserId)
    : BaseQuery<IReadOnlyList<PaymentDto>>;

public sealed record GetPaymentByIdQuery(string PaymentId)
    : BaseQuery<PaymentDto>;

// ============================ SERVICE INTERFACE ============================
public interface IPaymentQueryService : IQueryService<GetPaymentsByUserQuery, IReadOnlyList<PaymentDto>> { }

// ============================ HANDLER (READ SIDE) ============================
public class PaymentQuery : IPaymentQueryService,
    IRequestHandler<GetPaymentsByUserQuery, IReadOnlyList<PaymentDto>>,
    IRequestHandler<GetPaymentByIdQuery, PaymentDto>
{
    private readonly IReadRepository<Payment> _paymentRepository;

    public PaymentQuery(IReadRepository<Payment> paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    // --- MediatR dispatch ---
    public Task<IReadOnlyList<PaymentDto>> Handle(GetPaymentsByUserQuery request, CancellationToken ct)
        => GetAsync(request, ct);

    public Task<PaymentDto> Handle(GetPaymentByIdQuery request, CancellationToken ct)
        => GetByIdAsync(request, ct);

    // --- Operations ---
    public async Task<IReadOnlyList<PaymentDto>> GetAsync(GetPaymentsByUserQuery request, CancellationToken ct)
    {
        var payments = await _paymentRepository.FindAsync(p => p.UserId == request.UserId, ct);

        return payments
            .OrderByDescending(p => p.CreatedDate)
            .Select(ToDto)
            .ToList();
    }

    public async Task<PaymentDto> GetByIdAsync(GetPaymentByIdQuery request, CancellationToken ct)
    {
        var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, ct)
            ?? throw new DomainException("Đơn thanh toán không tồn tại.");
        return ToDto(payment);
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
