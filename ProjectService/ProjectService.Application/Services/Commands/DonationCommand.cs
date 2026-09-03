using MediatR;
using ProjectService.Application.Common.Base;
using ProjectService.Application.Services.DTOs;
using ProjectService.Domain.Enum;
using ProjectService.Domain.Exceptions;

namespace ProjectService.Application.Services.Commands;

// ============================ REQUEST ============================
public sealed record DonateCommand(
    string FromAccountId,
    string FundId,
    string BankCode,
    decimal Amount,
    string? Message = null,
    string? TransactionPassword = null) : BaseCommand<TransactionDto>;

// ============================ SERVICE INTERFACE ============================
/// <summary>Interface của Donation Command Service.</summary>
public interface IDonationCommandService
{
    Task<TransactionDto> DonateAsync(DonateCommand request, CancellationToken cancellationToken = default);
}

// ============================ HANDLER (WRITE SIDE) ============================
/// <summary>
/// Thực hiện giao dịch ủng hộ: tái sử dụng nghiệp vụ tạo giao dịch liên ngân hàng
/// với phí = 0 và danh mục = Donation. Thông tin người nhận được lưu đầy đủ.
/// </summary>
public class DonationCommand : IDonationCommandService,
    IRequestHandler<DonateCommand, TransactionDto>
{
    private readonly IMediator _mediator;

    public DonationCommand(IMediator mediator)
    {
        _mediator = mediator;
    }

    // --- MediatR dispatch ---
    public Task<TransactionDto> Handle(DonateCommand request, CancellationToken ct)
        => DonateAsync(request, ct);

    // --- Operations ---
    public async Task<TransactionDto> DonateAsync(DonateCommand request, CancellationToken ct)
    {
        var fund = DonationFundCatalog.Find(request.FundId)
            ?? throw new NotFoundException("Quỹ quyên góp", request.FundId);

        var account = fund.Accounts.FirstOrDefault(a => a.BankCode == request.BankCode)
            ?? throw new DomainException("Không tìm thấy tài khoản tiếp nhận của quỹ.");

        var description = string.IsNullOrWhiteSpace(request.Message)
            ? $"Ủng hộ {fund.Name}"
            : $"Ủng hộ {fund.Name} — {request.Message.Trim()}";

        // Fee = 0: giao dịch ủng hộ miễn phí.
        return await _mediator.Send(new CreateTransactionCommand(
            FromAccountId: request.FromAccountId,
            Type: TransactionType.InterbankTransfer,
            Amount: request.Amount,
            Description: description,
            ToAccountId: null,
            ReceiverAccount: account.AccountNumber,
            ReceiverName: fund.Name,
            ReceiverBankCode: account.BankCode,
            Category: TransactionCategory.Donation,
            IsEarlyWithdrawal: false,
            TransactionPassword: request.TransactionPassword,
            Fee: 0m), ct);
    }
}
