using ProjectService.Application.Common.Interfaces;
using ProjectService.Application.Services.DTOs;
using ProjectService.Domain.Entity;
using ProjectService.Domain.Exceptions;

namespace ProjectService.Application.Services;

/// <summary>
/// Service AutoEarn (sinh lời tự động):
///   - Đảm bảo tồn tại cấu hình mặc định (1 dòng duy nhất).
///   - Chạy job sinh lời hằng ngày (gọi từ HostedService hoặc "Run now" của admin).
///   - Đọc/cập nhật cấu hình, tổng hợp số liệu cho dashboard.
/// </summary>
public interface IAutoEarnService
{
    Task EnsureSettingAsync(CancellationToken ct = default);
    Task<AutoEarnSettingDto> GetSettingAsync(CancellationToken ct = default);
    Task<AutoEarnSettingDto> UpdateSettingAsync(UpdateAutoEarnSettingRequest request, CancellationToken ct = default);
    Task<AutoEarnSummaryDto> GetSummaryAsync(string userId, CancellationToken ct = default);
    Task<IReadOnlyList<AutoEarnLogDto>> GetLogsAsync(CancellationToken ct = default);
    Task<AutoEarnSettingDto> RunDailyJobAsync(CancellationToken ct = default, bool force = false);
    Task<IReadOnlyList<AutoEarnAccountAdminDto>> GetAccountsAsync(CancellationToken ct = default);
    Task<AutoEarnAccountAdminDto> UpdateAccountEnrollmentAsync(
        string accountId, bool isEnrolled, decimal principal, CancellationToken ct = default);
}

/// <summary>Triển khai <see cref="IAutoEarnService"/>.</summary>
public class AutoEarnService : IAutoEarnService
{
    private static readonly TimeZoneInfo VnTz = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");

    private readonly IWriteRepository<AutoEarnSetting> _settingWriter;
    private readonly IReadRepository<AutoEarnSetting> _settingReader;
    private readonly IWriteRepository<AutoEarnLog> _logWriter;
    private readonly IReadRepository<AutoEarnLog> _logReader;
    private readonly IWriteRepository<Account> _accountWriter;
    private readonly IReadRepository<Account> _accountReader;
    private readonly IUnitOfWork _unitOfWork;

    public AutoEarnService(
        IWriteRepository<AutoEarnSetting> settingWriter,
        IReadRepository<AutoEarnSetting> settingReader,
        IWriteRepository<AutoEarnLog> logWriter,
        IReadRepository<AutoEarnLog> logReader,
        IWriteRepository<Account> accountWriter,
        IReadRepository<Account> accountReader,
        IUnitOfWork unitOfWork)
    {
        _settingWriter = settingWriter;
        _settingReader = settingReader;
        _logWriter = logWriter;
        _logReader = logReader;
        _accountWriter = accountWriter;
        _accountReader = accountReader;
        _unitOfWork = unitOfWork;
    }

    private static DateTime VnNow() => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, VnTz);
    private static DateTime ToUtc(DateTime local) => TimeZoneInfo.ConvertTimeToUtc(local, VnTz);

    private static DateTime ComputeNextRunUtc(AutoEarnSetting setting, DateTime vnNow)
    {
        var parts = setting.RunTime.Split(':');
        var hh = int.TryParse(parts.ElementAtOrDefault(0), out var h) ? h : 0;
        var mm = int.TryParse(parts.ElementAtOrDefault(1), out var m) ? m : 0;
        var today = vnNow.Date.AddHours(hh).AddMinutes(mm);
        var next = today <= vnNow ? today.AddDays(1) : today;
        return ToUtc(next);
    }

    /// <summary>Đảm bảo luôn có đúng 1 dòng cấu hình AutoEarn.</summary>
    public async Task EnsureSettingAsync(CancellationToken ct = default)
    {
        var existing = await _settingReader.ListAsync(ct);
        if (existing.Count > 0) return;

        var now = DateTime.UtcNow;
        var setting = new AutoEarnSetting
        {
            Id = Guid.NewGuid().ToString("N"),
            CreatedDate = now,
            CreatedBy = "system",
            IsActive = true,
            AnnualInterestRate = 4.5m,
            RunTime = "00:00",
        };
        setting.NextRunAt = ComputeNextRunUtc(setting, VnNow());
        await _settingWriter.AddAsync(setting, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<AutoEarnSettingDto> GetSettingAsync(CancellationToken ct = default)
    {
        await EnsureSettingAsync(ct);
        var setting = (await _settingReader.ListAsync(ct)).First();
        return ToDto(setting);
    }

    public async Task<AutoEarnSettingDto> UpdateSettingAsync(UpdateAutoEarnSettingRequest request, CancellationToken ct = default)
    {
        await EnsureSettingAsync(ct);
        var setting = (await _settingReader.ListAsync(ct)).First();

        if (request.IsActive.HasValue) setting.IsActive = request.IsActive.Value;
        if (request.AnnualInterestRate.HasValue)
        {
            if (request.AnnualInterestRate.Value < 0 || request.AnnualInterestRate.Value > 20)
                throw new DomainException("Lãi suất phải nằm trong khoảng 0 – 20%.");
            setting.AnnualInterestRate = request.AnnualInterestRate.Value;
        }
        if (!string.IsNullOrWhiteSpace(request.RunTime))
        {
            if (!TimeOnly.TryParse(request.RunTime.Trim(), out _))
                throw new DomainException("Thời gian chạy phải đúng định dạng HH:mm (VD: 00:00, 06:30).");
            setting.RunTime = request.RunTime.Trim();
        }

        setting.LastModifiedDate = DateTime.UtcNow;
        setting.LastModifiedBy = "admin";
        setting.NextRunAt = ComputeNextRunUtc(setting, VnNow());

        _settingWriter.Update(setting);
        await _unitOfWork.SaveChangesAsync(ct);
        return ToDto(setting);
    }

    /// <summary>
    /// Job sinh lời hằng ngày: cộng lãi = gốc * lãi suất / 365 cho từng tài khoản
    /// tham gia, ghi nhật ký và cập nhật thời gian chạy.
    /// </summary>
    public async Task<AutoEarnSettingDto> RunDailyJobAsync(CancellationToken ct = default, bool force = false)
    {
        await EnsureSettingAsync(ct);
        var setting = (await _settingReader.ListAsync(ct)).First();

        var nowUtc = DateTime.UtcNow;
        var runDate = DateTime.SpecifyKind(VnNow().Date, DateTimeKind.Utc);

        if (!setting.IsActive)
        {
            setting.LastRunAt = nowUtc;
            setting.NextRunAt = ComputeNextRunUtc(setting, VnNow());
            _settingWriter.Update(setting);
            await _unitOfWork.SaveChangesAsync(ct);
            return ToDto(setting);
        }

        // Chống ghi lãi 2 lần trong cùng 1 ngày (VD: server khởi động lại).
        var alreadyRanToday = setting.LastRunAt.HasValue
            && TimeZoneInfo.ConvertTimeFromUtc(setting.LastRunAt.Value, VnTz).Date == runDate;

        if (!alreadyRanToday || force)
        {
            var rate = setting.AnnualInterestRate;
            var enrolled = (await _accountReader.FindAsync(a => a.IsAutoEarnEnrolled, ct))
                .Where(a => a.IsActive && a.AutoEarnPrincipal > 0)
                .ToList();

            foreach (var account in enrolled)
            {
                var interest = Math.Round(
                    account.AutoEarnPrincipal * rate / 100m / 365m,
                    0, MidpointRounding.AwayFromZero);
                if (interest <= 0) continue;

                account.Balance += interest;
                account.LastModifiedDate = nowUtc;
                account.LastModifiedBy = "system";
                _accountWriter.Update(account);

                await _logWriter.AddAsync(new AutoEarnLog
                {
                    Id = Guid.NewGuid().ToString("N"),
                    CreatedDate = nowUtc,
                    CreatedBy = "system",
                    AccountId = account.Id,
                    AccountNumber = account.AccountNumber,
                    RunDate = runDate,
                    Principal = account.AutoEarnPrincipal,
                    InterestAmount = interest,
                    AnnualRate = rate,
                }, ct);
            }
        }

        setting.LastRunAt = nowUtc;
        setting.NextRunAt = ComputeNextRunUtc(setting, VnNow());
        setting.LastModifiedDate = nowUtc;
        _settingWriter.Update(setting);

        await _unitOfWork.SaveChangesAsync(ct);
        return ToDto(setting);
    }

    /// <summary>Tổng hợp AutoEarn cho 1 user (dashboard): tổng gốc + lũy kế tháng này.</summary>
    public async Task<AutoEarnSummaryDto> GetSummaryAsync(string userId, CancellationToken ct = default)
    {
        await EnsureSettingAsync(ct);
        var setting = (await _settingReader.ListAsync(ct)).First();

        var accounts = (await _accountReader.FindAsync(a => a.UserId == userId, ct))
            .Where(a => a.IsActive)
            .ToList();

        var enrolled = accounts
            .Where(a => a.IsAutoEarnEnrolled)
            .Select(a => new AutoEarnAccountDto(a.Id, a.AccountNumber, a.AutoEarnPrincipal, true))
            .ToList();

        var totalPrincipal = enrolled.Sum(e => e.Principal);

        // Lũy kế tháng này (theo giờ VN) cho các tài khoản tham gia.
        var accountIds = enrolled.Select(e => e.AccountId).ToHashSet();
        var vnNow = VnNow();
        var monthStart = DateTime.SpecifyKind(vnNow.Date.AddDays(1 - vnNow.Day), DateTimeKind.Utc);
        var logs = accountIds.Count == 0
            ? new List<AutoEarnLog>()
            : (await _logReader.FindAsync(l => accountIds.Contains(l.AccountId), ct))
                .Where(l => l.RunDate >= monthStart)
                .ToList();
        var monthlyAccum = logs.Sum(l => l.InterestAmount);

        return new AutoEarnSummaryDto(
            setting.IsActive,
            setting.AnnualInterestRate,
            setting.RunTime,
            setting.LastRunAt,
            setting.NextRunAt,
            totalPrincipal,
            monthlyAccum,
            enrolled);
    }

    public async Task<IReadOnlyList<AutoEarnLogDto>> GetLogsAsync(CancellationToken ct = default)
    {
        var logs = await _logReader.ListAsync(ct);
        return logs
            .OrderByDescending(l => l.RunDate)
            .ThenByDescending(l => l.CreatedDate)
            .Select(l => new AutoEarnLogDto(
                l.Id, l.AccountId, l.AccountNumber, l.RunDate,
                l.Principal, l.InterestAmount, l.AnnualRate, l.CreatedDate))
            .ToList();
    }

    /// <summary>Danh sách tài khoản + trạng thái AutoEarn (admin quản lý đăng ký).</summary>
    public async Task<IReadOnlyList<AutoEarnAccountAdminDto>> GetAccountsAsync(CancellationToken ct = default)
    {
        var accounts = await _accountReader.FindWithIncludesAsync(a => a.IsActive, a => a.User);
        return accounts
            .OrderBy(a => a.User.FullName)
            .ThenBy(a => a.AccountNumber)
            .Select(a => new AutoEarnAccountAdminDto(
                a.Id,
                a.AccountNumber,
                a.User.FullName,
                a.Balance,
                a.IsAutoEarnEnrolled,
                a.AutoEarnPrincipal))
            .ToList();
    }

    /// <summary>Đăng ký/hủy đăng ký 1 tài khoản tham gia AutoEarn (admin).</summary>
    public async Task<AutoEarnAccountAdminDto> UpdateAccountEnrollmentAsync(
        string accountId, bool isEnrolled, decimal principal, CancellationToken ct = default)
    {
        var account = (await _accountReader.FindWithIncludesAsync(a => a.Id == accountId, a => a.User))
            .FirstOrDefault()
            ?? throw new NotFoundException(nameof(Account), accountId);

        if (isEnrolled && principal <= 0)
            throw new DomainException("Tiền gốc tham gia phải lớn hơn 0.");

        account.IsAutoEarnEnrolled = isEnrolled;
        account.AutoEarnPrincipal = isEnrolled ? principal : 0;
        account.LastModifiedDate = DateTime.UtcNow;
        account.LastModifiedBy = "admin";
        _accountWriter.Update(account);
        await _unitOfWork.SaveChangesAsync(ct);

        return new AutoEarnAccountAdminDto(
            account.Id,
            account.AccountNumber,
            account.User.FullName,
            account.Balance,
            account.IsAutoEarnEnrolled,
            account.AutoEarnPrincipal);
    }

    private static AutoEarnSettingDto ToDto(AutoEarnSetting s)
        => new(s.IsActive, s.AnnualInterestRate, s.RunTime, s.LastRunAt, s.NextRunAt);
}
