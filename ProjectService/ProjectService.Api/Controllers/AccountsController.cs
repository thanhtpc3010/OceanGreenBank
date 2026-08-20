using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectService.Application.Services.Commands;
using ProjectService.Application.Services.DTOs;
using ProjectService.Application.Services.Queries;

namespace ProjectService.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccountsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ---- Query (Read side) ----
    [HttpGet("{id}")]
    public async Task<ActionResult<AccountDto>> GetById(string id, CancellationToken ct)
        => Ok(await _mediator.Send(new GetAccountQuery(id), ct));

    [HttpGet("by-user/{userId}")]
    public async Task<ActionResult<IReadOnlyList<AccountDto>>> GetByUser(string userId, CancellationToken ct)
        => Ok(await _mediator.Send(new GetAccountsByUserQuery(userId), ct));

    [HttpGet("{id}/transactions")]
    public async Task<ActionResult<IReadOnlyList<TransactionDto>>> GetTransactions(string id, CancellationToken ct)
        => Ok(await _mediator.Send(new GetAccountTransactionsQuery(id), ct));

    // ---- Command (Write side) ----
    [HttpPost]
    public async Task<ActionResult<AccountDto>> Create([FromBody] CreateAccountRequest request, CancellationToken ct)
        => Ok(await _mediator.Send(
            new CreateAccountCommand(
                request.UserId,
                request.Currency,
                request.Type,
                request.SavingsTermMonths,
                request.InterestRate,
                request.SavingsStartDate),
            ct));

    [HttpPut("{id}")]
    public async Task<ActionResult<AccountDto>> Update(string id, [FromBody] UpdateAccountRequest request, CancellationToken ct)
        => Ok(await _mediator.Send(
            new UpdateAccountCommand(id, request.Currency, request.IsActive),
            ct));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteAccountCommand(id), ct);
        return NoContent();
    }
}
