using MediatR;
using ShoppingCart.Application.DTOs;
using ShoppingCart.Application.Interfaces;
using ShoppingCart.Application.Queries.GetProducts;

namespace ShoppingCart.API.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/products")
            .WithTags("Products")
            .WithOpenApi();

        group.MapGet("", async (IMediator mediator, CancellationToken cancellationToken) =>
        {
            var query = new GetProductsQuery();
            var products = await mediator.Send(query, cancellationToken);
            return Results.Ok(products);
        })
        .WithName("GetProducts")
        .WithSummary("Get all products")
        .Produces<List<ProductDto>>();

        group.MapGet("/{id:int}", async (IProductService productService, int id, CancellationToken cancellationToken) =>
        {
            var product = await productService.GetProductByIdAsync(id, cancellationToken);
            if (product == null)
                return Results.NotFound();
            return Results.Ok(product);
        })
        .WithName("GetProduct")
        .WithSummary("Get product by ID")
        .Produces<ProductDto>()
        .Produces(404);
    }
}
