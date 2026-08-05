using MediatR;

namespace ProjectService.Application.Common.Base;

/// <summary>
/// Lớp cơ sở cho mọi Command (write) không trả về dữ liệu (POST/PUT/PATCH/DELETE đơn giản).
/// Mọi field chung của write side nên đặt ở đây (UserId, RequestedAtUtc, ...).
/// </summary>
public abstract record BaseCommand : IRequest<Unit>
{
    public DateTime RequestedAtUtc { get; } = DateTime.UtcNow;
}

/// <summary>
/// Lớp cơ sở cho mọi Command (write) có trả về dữ liệu.
/// </summary>
public abstract record BaseCommand<TResponse> : IRequest<TResponse>
{
    public DateTime RequestedAtUtc { get; } = DateTime.UtcNow;
}
