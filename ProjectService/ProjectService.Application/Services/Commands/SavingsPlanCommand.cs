using MediatR;
using ProjectService.Application.Common.Base;
using ProjectService.Application.Common.Interfaces;
using ProjectService.Application.Services.DTOs;
using ProjectService.Domain.Entity;
using ProjectService.Domain.Enum;
using ProjectService.Domain.Exceptions;

namespace ProjectService.Application.Services.Commands;

// ============================ REQUEST ============================
public sealed record CreateSavingsPlanCommand(
    string UserId,
    string SourceAccountId,
    string TargetAccountId,
    decimal Amount,
    string Cycle,
    DateTime StartDate) : BaseCommand<SavingsPlanDto>;

public sealed record DepositSavingsPlanCommand(string PlanId) : BaseCommand<SavingsPlanDto>;

public sealed record CancelSavingsPlanCommand(string PlanId) : BaseCommand<Unit>;

// ============================ HANDLER ============================
/// <summary>
/// Write operations cho kế hoạch tiết kiệm định kỳ.
/// Deposit tái sử dụng CreateTransactionCommand (ghi Nợ/Có + ghi giao dịch).
/// </summary>
public class SavingsPlanCommand :
    IRequestHandler<CreateSavingsPlanCommand, SavingsPlanDto>,
    IRequestHandler<DepositSavingsPlanCommand, SavingsPlanDto>,
    IRequestHandler<CancelSavingsPlanCommand, Unit>
{
    private readonly IWriteRepository<SavingsPlan> _planRepository;
    private readonly IReadRepository<Account> _accountRepository;
    private readonly IMediator _mediator;
    private readonly IUnitOfWork _unitOfWork;

    public SavingsPlanCommand(
        IWriteRepository<SavingsPlan> planRepository,
        IReadRepository<Account> accountRepository,
        IMediator mediator,
        IUnitOfWork unitOfWork)
    {
        _planRepository = planRepository;
        _accountRepository = accountRepository;
        _mediator = mediator;
        _unitOfWork = unitOfWork;
    }

    public async Task<SavingsPlanDto> Handle(CreateSavingsPlanCommand request, CancellationToken ct)
    {
        if (request.Amount <= 0)
            throw new DomainException("Số tiền gửi mỗi kỳ phải lớn hơn 0.");

        var source = await _accountRepository.GetByIdAsync(request.SourceAccountId, ct)
            ?? throw new DomainException("Tài khoản nguồn không tồn tại.");
        var target = await _accountRepository.GetByIdAsync(request.TargetAccountId, ct)
            ?? throw new DomainException("Tài khoản tiết kiệm đích không tồn tại.");

        if (request.SourceAccountId == request.TargetAccountId)
            throw new DomainException("Tài khoản nguồn và tài khoản đích phải khác nhau.");

        var plan = new SavingsPlan
        {
            Id = Guid.NewGuid().ToString("N"),
            CreatedDate = DateTime.UtcNow,
            CreatedBy = null,
            UserId = request.UserId,
            SourceAccountId = request.SourceAccountId,
            TargetAccountId = request.TargetAccountId,
            Amount = request.Amount,
            Cycle = NormalizeCycle(request.Cycle),
            StartDate = DateTime.SpecifyKind(request.StartDate, DateTimeKind.Utc),
            NextDepositDate = DateTime.SpecifyKind(request.StartDate, DateTimeKind.Utc),
            IsActive = true,
            TotalDeposits = 0,
            TotalSaved = 0m
        };

        await _planRepository.AddAsync(plan, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return ToDto(plan, source.AccountNumber, target.AccountNumber);
    }

    public async Task<SavingsPlanDto> Handle(DepositSavingsPlanCommand request, CancellationToken ct)
    {
        var plan = await _planRepository.GetByIdAsync(request.PlanId, ct)
            ?? throw new NotFoundException(nameof(SavingsPlan), request.PlanId);

        if (!plan.IsActive)
            throw new DomainException("Kế hoạch đã bị hủy.");

        // Tái sử dụng logic chuyển tiền nội bộ: trích từ nguồn → đích, ghi giao dịch loại Savings.
        await _mediator.Send(new CreateTransactionCommand(
            plan.SourceAccountId,
            TransactionType.InternalTransfer,
            plan.Amount,
            $"Gửi tiết kiệm định kỳ ({plan.Cycle})",
            plan.TargetAccountId,
            null,
            null,
            null,
            TransactionCategory.Savings), ct);

        plan.TotalDeposits += 1;
        plan.TotalSaved += plan.Amount;
        plan.LastModifiedDate = DateTime.UtcNow;
        plan.NextDepositDate = NextDate(plan.NextDepositDate ?? plan.StartDate, plan.Cycle);
        _planRepository.Update(plan);
        await _unitOfWork.SaveChangesAsync(ct);

        var source = await _accountRepository.GetByIdAsync(plan.SourceAccountId, ct);
        var target = await _accountRepository.GetByIdAsync(plan.TargetAccountId, ct);
        return ToDto(plan, source?.AccountNumber ?? "", target?.AccountNumber ?? "");
    }

    public async Task<Unit> Handle(CancelSavingsPlanCommand request, CancellationToken ct)
    {
        var plan = await _planRepository.GetByIdAsync(request.PlanId, ct)
            ?? throw new NotFoundException(nameof(SavingsPlan), request.PlanId);

        plan.IsActive = false;
        plan.NextDepositDate = null;
        plan.LastModifiedDate = DateTime.UtcNow;
        _planRepository.Update(plan);
        await _unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }

    private static string NormalizeCycle(string cycle)
        => cycle.ToUpperInvariant() switch
        {
            "DAILY" or "WEEKLY" => cycle.ToUpperInvariant(),
            _ => "MONTHLY"
        };

    private static DateTime NextDate(DateTime from, string cycle) => cycle switch
    {
        "DAILY" => from.AddDays(1),
        "WEEKLY" => from.AddDays(7),
        _ => from.AddMonths(1)
    };

    private static SavingsPlanDto ToDto(SavingsPlan p, string sourceNo, string targetNo) => new(
        p.Id,
        p.UserId,
        p.SourceAccountId,
        sourceNo,
        p.TargetAccountId,
        targetNo,
        p.Amount,
        p.Cycle,
        p.StartDate,
        p.NextDepositDate,
        p.IsActive,
        p.TotalDeposits,
        p.TotalSaved,
        p.CreatedDate);
}
