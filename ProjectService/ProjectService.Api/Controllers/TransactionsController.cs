using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProjectService.Application.Services.Commands;
using ProjectService.Application.Services.DTOs;

namespace ProjectService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TransactionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ---- Command (Write side) ----
    [HttpPost]
    public async Task<ActionResult<TransactionDto>> Create([FromBody] CreateTransactionRequest request, CancellationToken ct)
        => Ok(await _mediator.Send(
            new CreateTransactionCommand(
                request.FromAccountId,
                request.Type,
                request.Amount,
                request.Description,
                request.ToAccountId,
                request.ReceiverAccount,
                request.ReceiverName,
                request.ReceiverBankCode),
            ct));

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> Cancel(string id, CancellationToken ct)
    {
        await _mediator.Send(new CancelTransactionCommand(id), ct);
        return NoContent();
    }
}
