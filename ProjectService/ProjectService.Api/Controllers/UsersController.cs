using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProjectService.Application.Features.Users.Commands;
using ProjectService.Application.Features.Users.DTOs;
using ProjectService.Application.Features.Users.Queries;

namespace ProjectService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetById(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new GetUserQuery(id), ct));

    [HttpPost]
    public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserRequest request, CancellationToken ct)
        => Ok(await _mediator.Send(
            new CreateUserCommand(request.FullName, request.Email, request.PhoneNumber, request.Password),
            ct));
}
