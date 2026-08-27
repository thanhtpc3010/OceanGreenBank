using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectService.Application.Services.Commands;
using ProjectService.Application.Services.DTOs;
using ProjectService.Application.Services.Queries;
using ProjectService.Domain.Enum;
using ProjectService.Domain.Exceptions;

namespace ProjectService.Api.Controllers;

/// <summary>DTO tạo đơn nạp tiền qua ví (mock).</summary>
public class CreatePaymentRequest
{
    public PaymentProvider Provider { get; set; } = PaymentProvider.Momo;
    public string AccountId { get; set; } = "";
    public decimal Amount { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// Nạp tiền qua ví điện tử (MoMo / ZaloPay) — bản mock local:
///   - POST /api/payments             → tạo đơn (Pending), trả MockPaymentUrl
///   - POST /api/payments/{id}/confirm→ ví xác nhận (credit tiền vào tài khoản)
///   - POST /api/payments/{id}/cancel → hủy thanh toán
///   - GET  /api/payments/by-user/{userId} → lịch sử nạp tiền
/// </summary>
[ApiController]
[Authorize]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<PaymentDto>> Create([FromBody] CreatePaymentRequest request, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new DomainException("Không xác định được người dùng.");

        return Ok(await _mediator.Send(
            new CreatePaymentCommand(userId, request.Provider, request.AccountId, request.Amount, request.Description),
            ct));
    }

    [HttpPost("{id}/confirm")]
    public async Task<ActionResult<PaymentDto>> Confirm(string id, CancellationToken ct)
        => Ok(await _mediator.Send(new ConfirmPaymentCommand(id), ct));

    [HttpPost("{id}/cancel")]
    public async Task<ActionResult<PaymentDto>> Cancel(string id, CancellationToken ct)
        => Ok(await _mediator.Send(new CancelPaymentCommand(id), ct));

    [HttpGet("{id}")]
    public async Task<ActionResult<PaymentDto>> GetById(string id, CancellationToken ct)
        => Ok(await _mediator.Send(new GetPaymentByIdQuery(id), ct));

    [HttpGet("by-user/{userId}")]
    public async Task<ActionResult<IReadOnlyList<PaymentDto>>> GetByUser(string userId, CancellationToken ct)
        => Ok(await _mediator.Send(new GetPaymentsByUserQuery(userId), ct));
}
