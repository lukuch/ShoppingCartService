using ShoppingCart.Application.DTOs;

namespace ShoppingCart.Application.Interfaces;

public interface IProductService
{
    Task<List<ProductDto>> GetProductsAsync(CancellationToken cancellationToken = default);
    Task<ProductDto> GetProductByIdAsync(int id, CancellationToken cancellationToken = default);
}
