using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectService.Application.Services.Commands;
using ProjectService.Application.Services.DTOs;
using ProjectService.Application.Services.Queries;

namespace ProjectService.Api.Controllers;

/// <summary>
/// Ủng hộ Mặt trận Tổ quốc / quyên góp từ thiện:
///   - GET  /api/donations/funds → danh sách quỹ + tài khoản tiếp nhận
///   - POST /api/donations       → thực hiện giao dịch ủng hộ (lưu vào lịch sử giao dịch)
/// </summary>
[ApiController]
[Authorize]
[Route("api/donations")]
public class DonationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DonationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Danh sách quỹ quyên góp / ủng hộ.</summary>
    [HttpGet("funds")]
    public async Task<ActionResult<IReadOnlyList<DonationFundDto>>> GetFunds(CancellationToken ct)
        => Ok(await _mediator.Send(new GetDonationFundsQuery(), ct));

    /// <summary>Thực hiện giao dịch ủng hộ (miễn phí, lưu thông tin chuyển khoản).</summary>
    [HttpPost]
    public async Task<ActionResult<TransactionDto>> Donate(
        [FromBody] CreateDonationRequest request, CancellationToken ct)
        => Ok(await _mediator.Send(new DonateCommand(
            request.FromAccountId,
            request.FundId,
            request.BankCode,
            request.Amount,
            request.Message,
            request.TransactionPassword), ct));
}
