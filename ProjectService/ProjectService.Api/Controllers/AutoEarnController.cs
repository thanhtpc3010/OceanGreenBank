using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectService.Application.Services;
using ProjectService.Application.Services.DTOs;

namespace ProjectService.Api.Controllers;

/// <summary>
/// Quản lý tính năng AutoEarn (sinh lời tự động):
///   - GET  /api/auto-earn/settings       → xem cấu hình (mọi user đã đăng nhập)
///   - PUT  /api/auto-earn/settings       → cập nhật cấu hình (chỉ admin)
///   - POST /api/auto-earn/run-now        → chạy job ngay (chỉ admin)
///   - GET  /api/auto-earn/summary/{userId} → tổng hợp cho dashboard
///   - GET  /api/auto-earn/logs           → nhật ký sinh lời (chỉ admin)
/// </summary>
[ApiController]
[Authorize]
[Route("api/auto-earn")]
public class AutoEarnController : ControllerBase
{
    private readonly IAutoEarnService _service;

    public AutoEarnController(IAutoEarnService service)
    {
        _service = service;
    }

    [HttpGet("settings")]
    public async Task<ActionResult<AutoEarnSettingDto>> GetSettings(CancellationToken ct)
        => Ok(await _service.GetSettingAsync(ct));

    [Authorize(Policy = "AdminOnly")]
    [HttpPut("settings")]
    public async Task<ActionResult<AutoEarnSettingDto>> UpdateSettings(
        [FromBody] UpdateAutoEarnSettingRequest request, CancellationToken ct)
        => Ok(await _service.UpdateSettingAsync(request, ct));

    [Authorize(Policy = "AdminOnly")]
    [HttpPost("run-now")]
    public async Task<ActionResult<AutoEarnSettingDto>> RunNow(CancellationToken ct)
        => Ok(await _service.RunDailyJobAsync(ct, force: true));

    [HttpGet("summary/{userId}")]
    public async Task<ActionResult<AutoEarnSummaryDto>> GetSummary(string userId, CancellationToken ct)
        => Ok(await _service.GetSummaryAsync(userId, ct));

    [Authorize(Policy = "AdminOnly")]
    [HttpGet("logs")]
    public async Task<ActionResult<IReadOnlyList<AutoEarnLogDto>>> GetLogs(CancellationToken ct)
        => Ok(await _service.GetLogsAsync(ct));

    [Authorize(Policy = "AdminOnly")]
    [HttpGet("accounts")]
    public async Task<ActionResult<IReadOnlyList<AutoEarnAccountAdminDto>>> GetAccounts(CancellationToken ct)
        => Ok(await _service.GetAccountsAsync(ct));

    [Authorize(Policy = "AdminOnly")]
    [HttpPut("accounts/{accountId}")]
    public async Task<ActionResult<AutoEarnAccountAdminDto>> UpdateAccountEnrollment(
        string accountId, [FromBody] UpdateAutoEarnEnrollmentRequest request, CancellationToken ct)
        => Ok(await _service.UpdateAccountEnrollmentAsync(accountId, request.IsEnrolled, request.Principal, ct));
}
