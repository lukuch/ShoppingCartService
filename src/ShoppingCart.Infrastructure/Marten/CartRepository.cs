using Marten;
using ShoppingCart.Application.Interfaces;
using ShoppingCart.Domain.Entities;
using ShoppingCart.Domain.Events;

namespace ShoppingCart.Infrastructure.Marten;

public class CartRepository(IDocumentStore documentStore) : ICartRepository
{
    public async Task<Cart?> GetCartAsync(string userId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        await using var session = documentStore.LightweightSession();

        var events = await session.Events.FetchStreamAsync(userId, token: cancellationToken);
        
        if (!events.Any())
            return null;

        var cart = new Cart { Id = userId, Items = [] };
        var items = new Dictionary<int, CartItem>();

        foreach (var evt in events.OrderBy(e => e.Sequence))
        {
            switch (evt.Data)
            {
                case ItemAdded itemAdded:
                    if (items.TryGetValue(itemAdded.ProductId, out var existingItem))
                    {
                        existingItem.Quantity += itemAdded.Quantity;
                    }
                    else
                    {
                        items[itemAdded.ProductId] = new CartItem
                        {
                            ProductId = itemAdded.ProductId,
                            Name = itemAdded.Name,
                            UnitPrice = itemAdded.UnitPrice,
                            Quantity = itemAdded.Quantity
                        };
                    }
                    break;

                case ItemQuantityUpdated quantityUpdated:
                    if (items.TryGetValue(quantityUpdated.ProductId, out var itemToUpdate))
                    {
                        itemToUpdate.Quantity = quantityUpdated.NewQuantity;
                    }
                    break;

                case ItemRemoved itemRemoved:
                    items.Remove(itemRemoved.ProductId);
                    break;
            }
        }

        return new Cart { Id = userId, Items = [.. items.Values] };
    }

    public async Task SaveEventAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        await using var session = documentStore.LightweightSession();

        string streamId;
        switch (domainEvent)
        {
            case ItemAdded itemAdded:
                streamId = itemAdded.UserId;
                session.Events.Append(streamId, itemAdded);
                break;

            case ItemQuantityUpdated quantityUpdated:
                streamId = quantityUpdated.UserId;
                session.Events.Append(streamId, quantityUpdated);
                break;

            case ItemRemoved itemRemoved:
                streamId = itemRemoved.UserId;
                session.Events.Append(streamId, itemRemoved);
                break;

            default:
                throw new NotSupportedException($"Event type {domainEvent.GetType().Name} is not supported");
        }

        await session.SaveChangesAsync(cancellationToken);
    }
}
