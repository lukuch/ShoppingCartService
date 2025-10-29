namespace ShoppingCart.Domain.Events;

public record ItemAdded(
    string UserId,
    int ProductId,
    string Name,
    decimal UnitPrice,
    int Quantity,
    DateTime OccurredOn = default) : IDomainEvent
{
    public DateTime OccurredOn { get; init; } = OccurredOn == default ? DateTime.UtcNow : OccurredOn;
}
