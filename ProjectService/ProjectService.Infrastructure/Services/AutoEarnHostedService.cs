using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProjectService.Application.Services;

namespace ProjectService.Infrastructure.Services;

/// <summary>
/// Background service chạy tự động khi server khởi động: mỗi ngày vào đúng
/// thời gian cấu hình (AutoEarnSetting.RunTime), gọi job sinh lời AutoEarn.
/// </summary>
public class AutoEarnHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AutoEarnHostedService> _logger;

    public AutoEarnHostedService(IServiceScopeFactory scopeFactory, ILogger<AutoEarnHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Khởi tạo cấu hình mặc định nếu chưa có (chạy 1 lần khi server start).
        try
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var svc = scope.ServiceProvider.GetRequiredService<IAutoEarnService>();
                await svc.EnsureSettingAsync(stoppingToken);
            }
            _logger.LogInformation("AutoEarnHostedService started — daily job enabled.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AutoEarnHostedService: failed to initialize setting.");
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            // Tính delay đến lần chạy kế tiếp.
            var delay = TimeSpan.FromMinutes(1);
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var svc = scope.ServiceProvider.GetRequiredService<IAutoEarnService>();
                    var setting = await svc.GetSettingAsync(stoppingToken);
                    if (setting.NextRunAt.HasValue)
                        delay = setting.NextRunAt.Value - DateTime.UtcNow;
                    if (delay <= TimeSpan.Zero) delay = TimeSpan.FromSeconds(10);
                    if (delay > TimeSpan.FromDays(2)) delay = TimeSpan.FromDays(2); // safety net
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AutoEarnHostedService: failed to compute next run.");
            }

            try
            {
                await Task.Delay(delay, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }

            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var svc = scope.ServiceProvider.GetRequiredService<IAutoEarnService>();
                    await svc.RunDailyJobAsync(stoppingToken);
                }
                _logger.LogInformation("AutoEarn daily job executed at {Utc:O}.", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AutoEarn daily job failed.");
            }
        }
    }
}
