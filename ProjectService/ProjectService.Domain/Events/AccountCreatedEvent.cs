namespace ProjectService.Domain.Events;

/// <summary>
/// Domain event được phát khi tạo tài khoản mới thành công.
/// </summary>
public sealed record AccountCreatedEvent(Guid AccountId, string AccountNumber) : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
