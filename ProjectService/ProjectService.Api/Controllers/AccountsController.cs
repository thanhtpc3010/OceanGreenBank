using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProjectService.Application.Features.Accounts.Commands;
using ProjectService.Application.Features.Accounts.DTOs;
using ProjectService.Application.Features.Accounts.Queries;
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

    [HttpGet("{id}")]
    public async Task<ActionResult<AccountDto>> GetById(string id, CancellationToken ct)
        => Ok(await _mediator.Send(new GetAccountQuery(id), ct));

    [HttpPost]
    public async Task<ActionResult<AccountDto>> Create([FromBody] CreateAccountRequest request, CancellationToken ct)
        => Ok(await _mediator.Send(
            new CreateAccountCommand(request.UserId, request.Currency),
            ct));

    [HttpGet("{id}/transactions")]
    public async Task<ActionResult<IReadOnlyList<TransactionDto>>> GetTransactions(string id, CancellationToken ct)
        => Ok(await _mediator.Send(new GetAccountTransactionsQuery(id), ct));
}
