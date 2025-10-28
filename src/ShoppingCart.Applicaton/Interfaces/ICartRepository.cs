using ShoppingCart.Domain.Entities;
using ShoppingCart.Domain.Events;

namespace ShoppingCart.Application.Interfaces;

public interface ICartRepository
{
    Task<Cart?> GetCartAsync(string userId, CancellationToken cancellationToken = default);
    Task SaveEventAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
}
