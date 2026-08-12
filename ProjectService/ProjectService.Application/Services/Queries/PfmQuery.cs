using MediatR;
using ProjectService.Application.Common.Base;
using ProjectService.Application.Common.Interfaces;
using ProjectService.Application.Services.DTOs;
using ProjectService.Domain.Entity;
using ProjectService.Domain.Enum;

namespace ProjectService.Application.Services.Queries;

/// <summary>
/// Query tổng hợp thu/chi PFM cho một user.
/// BOT PFM chỉ gom nhóm + tính tổng theo danh mục mà user đã chọn.
/// </summary>
public sealed record GetPfmSummaryQuery(string UserId) : BaseQuery<PfmSummaryDto>;

/// <summary>Handler tổng hợp thu chi.</summary>
public class PfmQuery :
    IRequestHandler<GetPfmSummaryQuery, PfmSummaryDto>
{
    private readonly IReadRepository<Account> _accountRepository;
    private readonly IReadRepository<Transaction> _transactionRepository;

    public PfmQuery(
        IReadRepository<Account> accountRepository,
        IReadRepository<Transaction> transactionRepository)
    {
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<PfmSummaryDto> Handle(GetPfmSummaryQuery request, CancellationToken ct)
    {
        var accounts = await _accountRepository.FindAsync(a => a.UserId == request.UserId, ct);
        var accountIds = accounts.Select(a => a.Id).ToHashSet();

        // Giao dịch liên quan đến user: gửi đi (chi) hoặc nhận về (thu)
        var transactions = await _transactionRepository.FindAsync(
            t => accountIds.Contains(t.FromAccountId) || (t.ToAccountId != null && accountIds.Contains(t.ToAccountId)),
            ct);
        var success = transactions.Where(t => t.Status == TransactionStatus.Success).ToList();

        // THU: tiền vào tài khoản của user (ToAccountId thuộc user)
        var totalIncome = success
            .Where(t => t.ToAccountId != null && accountIds.Contains(t.ToAccountId))
            .Sum(t => t.Amount);

        // CHI: tiền ra từ tài khoản của user (FromAccountId thuộc user), gồm cả phí
        var expenses = success
            .Where(t => accountIds.Contains(t.FromAccountId))
            .GroupBy(t => t.Category)
            .Select(g => new CategorySummaryDto(
                g.Key,
                CategoryName(g.Key),
                g.Sum(t => t.Amount + t.Fee),
                g.Count()))
            .OrderByDescending(c => c.Total)
            .ToList();

        var totalExpense = expenses.Sum(e => e.Total);

        return new PfmSummaryDto(
            totalIncome,
            totalExpense,
            totalIncome - totalExpense,
            expenses);
    }

    private static string CategoryName(TransactionCategory category) => category switch
    {
        TransactionCategory.Food => "Ăn uống",
        TransactionCategory.Shopping => "Mua sắm",
        TransactionCategory.Bills => "Hóa đơn",
        TransactionCategory.Transport => "Di chuyển",
        TransactionCategory.Entertainment => "Giải trí",
        TransactionCategory.Health => "Y tế",
        TransactionCategory.Education => "Giáo dục",
        TransactionCategory.Savings => "Tiết kiệm",
        TransactionCategory.Transfer => "Chuyển khoản",
        _ => "Khác"
    };
}
