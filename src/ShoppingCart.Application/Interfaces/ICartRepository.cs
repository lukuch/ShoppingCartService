using ShoppingCart.Domain.Entities;

namespace ShoppingCart.Application.Interfaces;

public interface ICartRepository
{
    Task<Cart?> GetByIdAsync(string userId, CancellationToken cancellationToken = default);
    Task SaveAsync(Cart cart, CancellationToken cancellationToken = default);
}
