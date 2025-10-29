namespace ShoppingCart.Domain.Events;

public record ItemQuantityUpdated(
    string UserId,
    int ProductId,
    int NewQuantity,
    DateTime OccurredOn = default) : IDomainEvent
{
    public DateTime OccurredOn { get; init; } = OccurredOn == default ? DateTime.UtcNow : OccurredOn;
}
