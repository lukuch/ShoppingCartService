namespace ShoppingCart.Domain.Events;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
