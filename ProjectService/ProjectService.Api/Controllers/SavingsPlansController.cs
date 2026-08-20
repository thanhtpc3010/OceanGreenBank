using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectService.Application.Services.Commands;
using ProjectService.Application.Services.DTOs;
using ProjectService.Application.Services.Queries;

namespace ProjectService.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/savings-plans")]
public class SavingsPlansController : ControllerBase
{
    private readonly IMediator _mediator;

    public SavingsPlansController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Tạo kế hoạch tiết kiệm định kỳ.</summary>
    [HttpPost]
    public async Task<ActionResult<SavingsPlanDto>> Create([FromBody] CreateSavingsPlanRequest request, CancellationToken ct)
        => Ok(await _mediator.Send(
            new CreateSavingsPlanCommand(
                request.UserId,
                request.SourceAccountId,
                request.TargetAccountId,
                request.Amount,
                request.Cycle,
                request.StartDate),
            ct));

    /// <summary>Danh sách kế hoạch tiết kiệm của user.</summary>
    [HttpGet("by-user/{userId}")]
    public async Task<ActionResult<IReadOnlyList<SavingsPlanDto>>> GetByUser(string userId, CancellationToken ct)
        => Ok(await _mediator.Send(new GetSavingsPlansByUserQuery(userId), ct));

    /// <summary>Gửi tiền ngay một kỳ cho kế hoạch (trích nguồn → đích).</summary>
    [HttpPost("{id}/deposit")]
    public async Task<ActionResult<SavingsPlanDto>> Deposit(string id, CancellationToken ct)
        => Ok(await _mediator.Send(new DepositSavingsPlanCommand(id), ct));

    /// <summary>Hủy kế hoạch.</summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Cancel(string id, CancellationToken ct)
    {
        await _mediator.Send(new CancelSavingsPlanCommand(id), ct);
        return NoContent();
    }
}
