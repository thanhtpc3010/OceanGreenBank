using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProjectService.Application.Features.Accounts.Commands;
using ProjectService.Application.Features.Accounts.DTOs;
using ProjectService.Application.Features.Accounts.Queries;
using ProjectService.Application.Features.Transactions.Commands;
using ProjectService.Application.Features.Transactions.DTOs;
using ProjectService.Application.Features.Transactions.Queries;

namespace ProjectService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccountsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AccountDto>> GetById(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new GetAccountQuery(id), ct));

    [HttpPost]
    public async Task<ActionResult<AccountDto>> Create([FromBody] CreateAccountRequest request, CancellationToken ct)
        => Ok(await _mediator.Send(
            new CreateAccountCommand(request.UserId, request.Type, request.Currency),
            ct));

    [HttpGet("{id:guid}/transactions")]
    public async Task<ActionResult<IReadOnlyList<TransactionDto>>> GetTransactions(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new GetAccountTransactionsQuery(id), ct));

    [HttpPost("{id:guid}/transactions")]
    public async Task<ActionResult<TransactionDto>> CreateTransaction(
        Guid id,
        [FromBody] CreateTransactionRequest request,
        CancellationToken ct)
        => Ok(await _mediator.Send(
            new CreateTransactionCommand(id, request.Type, request.Amount, request.Description),
            ct));
}
