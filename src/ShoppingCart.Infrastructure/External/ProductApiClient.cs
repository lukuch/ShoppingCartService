using ShoppingCart.Application.DTOs;
using ShoppingCart.Application.Interfaces;

namespace ShoppingCart.Infrastructure.External;

public class ProductApiClient(IProductApi productApi) : IProductService
{
    public async Task<List<ProductDto>> GetProductsAsync(CancellationToken cancellationToken = default)
    {
        var products = await productApi.GetProductsAsync(cancellationToken);
        return products.Select(MapToDto).ToList();
    }

    public async Task<ProductDto> GetProductByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await productApi.GetProductByIdAsync(id, cancellationToken);
        return MapToDto(product);
    }

    private static ProductDto MapToDto(ProductApiResponse apiResponse) => new(
        apiResponse.Id,
        apiResponse.Title,
        apiResponse.Price,
        apiResponse.Description,
        apiResponse.Category.Name,
        apiResponse.Images);
}
