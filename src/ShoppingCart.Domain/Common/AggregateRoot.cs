using ShoppingCart.Domain.Events;

namespace ShoppingCart.Domain.Common;

public abstract class AggregateRoot<TId> where TId : notnull
{
    public TId Id { get; protected set; } = default!;
    
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
