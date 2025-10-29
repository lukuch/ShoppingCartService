using MediatR;
using ShoppingCart.Application.DTOs;
using ShoppingCart.Application.Interfaces;

namespace ShoppingCart.Application.Queries.GetProducts;

public class GetProductsHandler(IProductService productService, ICacheService cacheService) : IRequestHandler<GetProductsQuery, List<ProductDto>>
{
    private const string CacheKey = "products:all";

    public async Task<List<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var cachedProducts = await cacheService.GetAsync<List<ProductDto>>(CacheKey, cancellationToken);
        if (cachedProducts != null)
            return cachedProducts;

        var products = await productService.GetProductsAsync(cancellationToken);
        
        await cacheService.SetAsync(CacheKey, products, TimeSpan.FromMinutes(10), cancellationToken);

        return products;
    }
}
