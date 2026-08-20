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
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ---- Query (Read side) ----
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> GetAll(CancellationToken ct)
        => Ok(await _mediator.Send(new GetUsersQuery(), ct));

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetById(string id, CancellationToken ct)
        => Ok(await _mediator.Send(new GetUserQuery(id), ct));

    /// <summary>Vai trò & quyền của user (phân quyền RBAC).</summary>
    [HttpGet("{id}/permissions")]
    public async Task<ActionResult<UserPermissionsDto>> GetPermissions(string id, CancellationToken ct)
        => Ok(await _mediator.Send(new GetUserPermissionsQuery(id), ct));

    // ---- Command (Write side) ----
    [HttpPost]
    public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserRequest request, CancellationToken ct)
        => Ok(await _mediator.Send(
            new CreateUserCommand(
                request.FullName,
                request.Email,
                request.Phone,
                request.IdentityCard,
                request.DateOfBirth,
                request.Password,
                request.Address),
            ct));

    [HttpPut("{id}")]
    public async Task<ActionResult<UserDto>> Update(string id, [FromBody] UpdateUserRequest request, CancellationToken ct)
        => Ok(await _mediator.Send(
            new UpdateUserCommand(id, request.FullName, request.Phone, request.Address, request.IsActive),
            ct));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteUserCommand(id), ct);
        return NoContent();
    }
}
