using Marten;
using ShoppingCart.Application.Interfaces;
using ShoppingCart.Domain.Entities;

namespace ShoppingCart.Infrastructure.Marten;

public class CartRepository(IDocumentStore documentStore) : ICartRepository
{
    public async Task<Cart?> GetByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        await using var session = documentStore.LightweightSession();
        return await session.Events.AggregateStreamAsync<Cart>(userId, token: cancellationToken);
    }

    public async Task SaveAsync(Cart cart, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(cart);

        await using var session = documentStore.LightweightSession();
        
        // Append all domain events
        foreach (var domainEvent in cart.DomainEvents)
        {
            session.Events.Append(cart.Id, domainEvent);
        }
        
        await session.SaveChangesAsync(cancellationToken);
        cart.ClearDomainEvents();
    }
}
