namespace ProjectService.Domain.Events;

public interface IDomainEvent
{
    DateTime OccurredOnUtc { get; }
}
