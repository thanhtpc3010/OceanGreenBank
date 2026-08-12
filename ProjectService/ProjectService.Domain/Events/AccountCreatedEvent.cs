using MediatR;

namespace ProjectService.Domain.Events;

/// <summary>
/// Domain event được phát khi tạo tài khoản mới thành công.
/// </summary>
public sealed record AccountCreatedEvent(string AccountId, string AccountNumber) : IDomainEvent, INotification
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
