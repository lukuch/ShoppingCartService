using Refit;

namespace ShoppingCart.Infrastructure.External;

public interface IProductApi
{
    [Get("/api/v1/products")]
    Task<List<ProductApiResponse>> GetProductsAsync(CancellationToken cancellationToken = default);

    [Get("/api/v1/products/{id}")]
    Task<ProductApiResponse> GetProductByIdAsync(int id, CancellationToken cancellationToken = default);
}
