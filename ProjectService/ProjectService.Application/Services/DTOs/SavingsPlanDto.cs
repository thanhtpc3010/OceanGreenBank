namespace ProjectService.Application.Services.DTOs;

/// <summary>Kế hoạch tiết kiệm định kỳ.</summary>
public sealed record SavingsPlanDto(
    string Id,
    string UserId,
    string SourceAccountId,
    string SourceAccountNumber,
    string TargetAccountId,
    string TargetAccountNumber,
    decimal Amount,
    string Cycle,
    DateTime StartDate,
    DateTime? NextDepositDate,
    bool IsActive,
    int TotalDeposits,
    decimal TotalSaved,
    DateTime CreatedDate);

/// <summary>Request tạo kế hoạch tiết kiệm định kỳ.</summary>
public sealed record CreateSavingsPlanRequest(
    string UserId,
    string SourceAccountId,
    string TargetAccountId,
    decimal Amount,
    string Cycle,
    DateTime StartDate);
