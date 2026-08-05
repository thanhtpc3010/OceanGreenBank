using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProjectService.Application.Features.Transactions.Commands;
using ProjectService.Application.Features.Transactions.DTOs;

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
}
