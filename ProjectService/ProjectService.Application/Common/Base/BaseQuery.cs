using MediatR;

namespace ProjectService.Application.Common.Base;

/// <summary>
/// Lớp cơ sở cho mọi Query (read) — GET đi qua ReadDbContext.
/// Mọi field chung của read side nên đặt ở đây (Paging, Filter, ...).
/// </summary>
public abstract record BaseQuery<TResponse> : IRequest<TResponse>
{
    public DateTime RequestedAtUtc { get; } = DateTime.UtcNow;
}
