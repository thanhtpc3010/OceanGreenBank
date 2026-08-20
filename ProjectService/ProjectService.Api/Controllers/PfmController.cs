using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectService.Application.Services.DTOs;
using ProjectService.Application.Services.Queries;

namespace ProjectService.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class PfmController : ControllerBase
{
    private readonly IMediator _mediator;

    public PfmController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>BOT PFM: tổng hợp thu/chi theo danh mục cho một user.</summary>
    [HttpGet("summary/{userId}")]
    public async Task<ActionResult<PfmSummaryDto>> GetSummary(string userId, CancellationToken ct)
        => Ok(await _mediator.Send(new GetPfmSummaryQuery(userId), ct));
}
