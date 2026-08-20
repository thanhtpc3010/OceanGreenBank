using MediatR;
using ProjectService.Application.Common.Base;
using ProjectService.Application.Common.Interfaces;
using ProjectService.Application.Services.DTOs;
using ProjectService.Domain.Entity;

namespace ProjectService.Application.Services.Queries;

/// <summary>Query danh sách kế hoạch tiết kiệm của user.</summary>
public sealed record GetSavingsPlansByUserQuery(string UserId) : BaseQuery<IReadOnlyList<SavingsPlanDto>>;

/// <summary>Handler lấy danh sách kế hoạch tiết kiệm theo user.</summary>
public class SavingsPlanQuery : IRequestHandler<GetSavingsPlansByUserQuery, IReadOnlyList<SavingsPlanDto>>
{
    private readonly IReadRepository<SavingsPlan> _planRepository;
    private readonly IReadRepository<Account> _accountRepository;

    public SavingsPlanQuery(
        IReadRepository<SavingsPlan> planRepository,
        IReadRepository<Account> accountRepository)
    {
        _planRepository = planRepository;
        _accountRepository = accountRepository;
    }

    public async Task<IReadOnlyList<SavingsPlanDto>> Handle(GetSavingsPlansByUserQuery request, CancellationToken ct)
    {
        var plans = await _planRepository.FindAsync(p => p.UserId == request.UserId, ct);
        var accountIds = plans.SelectMany(p => new[] { p.SourceAccountId, p.TargetAccountId }).Distinct();
        var accounts = await _accountRepository.FindAsync(a => accountIds.Contains(a.Id), ct);
        var map = accounts.ToDictionary(a => a.Id, a => a.AccountNumber);

        return plans
            .OrderByDescending(p => p.CreatedDate)
            .Select(p => new SavingsPlanDto(
                p.Id,
                p.UserId,
                p.SourceAccountId,
                map.GetValueOrDefault(p.SourceAccountId, ""),
                p.TargetAccountId,
                map.GetValueOrDefault(p.TargetAccountId, ""),
                p.Amount,
                p.Cycle,
                p.StartDate,
                p.NextDepositDate,
                p.IsActive,
                p.TotalDeposits,
                p.TotalSaved,
                p.CreatedDate))
            .ToList();
    }
}
