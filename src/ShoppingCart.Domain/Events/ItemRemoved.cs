namespace ShoppingCart.Domain.Events;

public record ItemRemoved(
    string UserId,
    int ProductId,
    DateTime OccurredOn = default) : IDomainEvent
{
    public DateTime OccurredOn { get; init; } = OccurredOn == default ? DateTime.UtcNow : OccurredOn;
}
