using Carter;
using MediatR;
using ShoppingCart.Application.DTOs;
using ShoppingCart.Application.Interfaces;
using ShoppingCart.Application.Queries.GetProducts;

namespace ShoppingCart.API.Modules;

public class ProductModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products")
            .WithTags("Products")
            .WithOpenApi();

        group.MapGet("", GetProducts)
            .WithName("GetProducts")
            .WithSummary("Get all products")
            .Produces<List<ProductDto>>();

        group.MapGet("/{id:int}", GetProduct)
            .WithName("GetProduct")
            .WithSummary("Get product by ID")
            .Produces<ProductDto>()
            .Produces(404);
    }

    private static async Task<IResult> GetProducts(
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetProductsQuery();
        var products = await mediator.Send(query, cancellationToken);
        return Results.Ok(products);
    }

    private static async Task<IResult> GetProduct(
        int id,
        IProductService productService,
        CancellationToken cancellationToken)
    {
        var product = await productService.GetProductByIdAsync(id, cancellationToken);
        if (product == null)
            return Results.NotFound();
        
        return Results.Ok(product);
    }
}
