namespace ShoppingCart.Domain.Events;

public record ItemRemoved(
    string UserId,
    int ProductId,
    DateTime OccurredOn = default) : IDomainEvent;
