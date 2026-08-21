namespace ProjectService.Application.Services.DTOs;

/// <summary>Thông tin cấu hình AutoEarn (trả về cho mọi người dùng đã đăng nhập).</summary>
public sealed record AutoEarnSettingDto(
    bool IsActive,
    decimal AnnualInterestRate,
    string RunTime,
    DateTime? LastRunAt,
    DateTime? NextRunAt);

/// <summary>Request cập nhật cấu hình AutoEarn (chỉ admin).</summary>
public sealed record UpdateAutoEarnSettingRequest(
    bool? IsActive = null,
    decimal? AnnualInterestRate = null,
    string? RunTime = null);

/// <summary>Tài khoản tham gia AutoEarn của một user.</summary>
public sealed record AutoEarnAccountDto(
    string AccountId,
    string AccountNumber,
    decimal Principal,
    bool IsEnrolled);

/// <summary>Bảng tổng hợp AutoEarn của một user (cho dashboard).</summary>
public sealed record AutoEarnSummaryDto(
    bool IsActive,
    decimal AnnualInterestRate,
    string RunTime,
    DateTime? LastRunAt,
    DateTime? NextRunAt,
    decimal TotalPrincipal,
    decimal MonthlyAccum,
    IReadOnlyList<AutoEarnAccountDto> EnrolledAccounts);

/// <summary>Một dòng nhật ký sinh lời AutoEarn.</summary>
public sealed record AutoEarnLogDto(
    string Id,
    string AccountId,
    string AccountNumber,
    DateTime RunDate,
    decimal Principal,
    decimal InterestAmount,
    decimal AnnualRate,
    DateTime CreatedDate);

/// <summary>Danh sách tài khoản + trạng thái đăng ký AutoEarn (dành cho admin).</summary>
public sealed record AutoEarnAccountAdminDto(
    string AccountId,
    string AccountNumber,
    string OwnerName,
    decimal Balance,
    bool IsEnrolled,
    decimal Principal);

/// <summary>Request đăng ký/hủy đăng ký 1 tài khoản tham gia AutoEarn (chỉ admin).</summary>
public sealed record UpdateAutoEarnEnrollmentRequest(
    bool IsEnrolled,
    decimal Principal);
