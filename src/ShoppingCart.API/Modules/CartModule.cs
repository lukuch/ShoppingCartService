using Carter;
using MediatR;
using ShoppingCart.Application.Commands.AddItemToCart;
using ShoppingCart.Application.Commands.RemoveCartItem;
using ShoppingCart.Application.Commands.UpdateCartItem;
using ShoppingCart.Application.DTOs;
using ShoppingCart.Application.Queries.GetCart;
using ShoppingCart.Application.Queries.GetCartSummary;

namespace ShoppingCart.API.Modules;

public class CartModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/cart")
            .WithTags("Cart")
            .WithOpenApi();

        group.MapPost("/{userId}/items", AddItemToCart)
            .WithName("AddItemToCart")
            .WithSummary("Add item to cart")
            .Accepts<AddItemToCartDto>("application/json")
            .Produces(201)
            .Produces(400);

        group.MapPatch("/{userId}/items/{productId:int}", UpdateCartItem)
            .WithName("UpdateCartItem")
            .WithSummary("Update cart item quantity")
            .Accepts<UpdateCartItemDto>("application/json")
            .Produces(204)
            .Produces(400);

        group.MapDelete("/{userId}/items/{productId:int}", RemoveCartItem)
            .WithName("RemoveCartItem")
            .WithSummary("Remove item from cart")
            .Produces(204)
            .Produces(400);

        group.MapGet("/{userId}", GetCart)
            .WithName("GetCart")
            .WithSummary("Get cart contents")
            .Produces<CartDto>()
            .Produces(404);

        group.MapGet("/{userId}/summary", GetCartSummary)
            .WithName("GetCartSummary")
            .WithSummary("Get cart summary")
            .Produces<CartSummaryDto>()
            .Produces(404);
    }

    private static async Task<IResult> AddItemToCart(
        AddItemToCartDto dto,
        string userId,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new AddItemToCartCommand(userId, dto.ProductId, dto.Quantity);
        var result = await mediator.Send(command, cancellationToken);

        if (!result.Success)
            return Results.BadRequest(new { error = result.ErrorMessage });

        return Results.Created($"/api/cart/{userId}/items/{dto.ProductId}", new { message = "Item added successfully" });
    }

    private static async Task<IResult> UpdateCartItem(
        UpdateCartItemDto dto,
        string userId,
        int productId,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCartItemCommand(userId, productId, dto.Quantity);
        var result = await mediator.Send(command, cancellationToken);

        if (!result.Success)
            return Results.BadRequest(new { error = result.ErrorMessage });

        return Results.NoContent();
    }

    private static async Task<IResult> RemoveCartItem(
        string userId,
        int productId,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new RemoveCartItemCommand(userId, productId);
        var result = await mediator.Send(command, cancellationToken);

        if (!result.Success)
            return Results.BadRequest(new { error = result.ErrorMessage });

        return Results.NoContent();
    }

    private static async Task<IResult> GetCart(
        string userId,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetCartQuery(userId);
        var cart = await mediator.Send(query, cancellationToken);

        if (cart == null)
            return Results.NotFound();

        return Results.Ok(cart);
    }

    private static async Task<IResult> GetCartSummary(
        string userId,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetCartSummaryQuery(userId);
        var summary = await mediator.Send(query, cancellationToken);

        if (summary == null)
            return Results.NotFound();

        return Results.Ok(summary);
    }
}
