using MediatR;
using Serilog;
using ShoppingCart.Application.Commands.AddItemToCart;
using ShoppingCart.Application.Commands.RemoveCartItem;
using ShoppingCart.Application.Commands.UpdateCartItem;
using ShoppingCart.Application.DTOs;
using ShoppingCart.Application.Interfaces;
using ShoppingCart.Application.Queries.GetCart;
using ShoppingCart.Application.Queries.GetCartSummary;
using ShoppingCart.Application.Queries.GetProducts;
using ShoppingCart.Infrastructure.DI;

var builder = WebApplication.CreateBuilder(args);

// Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Application services
builder.Services.AddApplicationServices();

// Infrastructure services
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

// Configure pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

// Register endpoints
app.MapGet("/api/products", async (IMediator mediator, CancellationToken cancellationToken) =>
{
    var query = new GetProductsQuery();
    var products = await mediator.Send(query, cancellationToken);
    return Results.Ok(products);
})
.WithName("GetProducts")
.WithOpenApi()
.Produces<List<ProductDto>>();

app.MapGet("/api/products/{id:int}", async (IProductService productService, int id, CancellationToken cancellationToken) =>
{
    var product = await productService.GetProductByIdAsync(id, cancellationToken);
    if (product == null)
        return Results.NotFound();
    return Results.Ok(product);
})
.WithName("GetProduct")
.WithOpenApi()
.Produces<ProductDto>()
.Produces(404);

app.MapPost("/api/cart/{userId}/items", async (IMediator mediator, string userId, AddItemToCartDto dto, CancellationToken cancellationToken) =>
{
    var command = new AddItemToCartCommand(userId, dto.ProductId, dto.Quantity);
    var result = await mediator.Send(command, cancellationToken);

    if (!result.Success)
        return Results.BadRequest(new { error = result.ErrorMessage });

    return Results.Created($"/api/cart/{userId}/items/{dto.ProductId}", new { message = "Item added successfully" });
})
.WithName("AddItemToCart")
.WithOpenApi()
.Accepts<AddItemToCartDto>("application/json")
.Produces(201)
.Produces(400);

app.MapPatch("/api/cart/{userId}/items/{productId:int}", async (IMediator mediator, string userId, int productId, UpdateCartItemDto dto, CancellationToken cancellationToken) =>
{
    var command = new UpdateCartItemCommand(userId, productId, dto.Quantity);
    var result = await mediator.Send(command, cancellationToken);

    if (!result.Success)
        return Results.BadRequest(new { error = result.ErrorMessage });

    return Results.NoContent();
})
.WithName("UpdateCartItem")
.WithOpenApi()
.Accepts<UpdateCartItemDto>("application/json")
.Produces(204)
.Produces(400);

app.MapDelete("/api/cart/{userId}/items/{productId:int}", async (IMediator mediator, string userId, int productId, CancellationToken cancellationToken) =>
{
    var command = new RemoveCartItemCommand(userId, productId);
    var result = await mediator.Send(command, cancellationToken);

    if (!result.Success)
        return Results.BadRequest(new { error = result.ErrorMessage });

    return Results.NoContent();
})
.WithName("RemoveCartItem")
.WithOpenApi()
.Produces(204)
.Produces(400);

app.MapGet("/api/cart/{userId}", async (IMediator mediator, string userId, CancellationToken cancellationToken) =>
{
    var query = new GetCartQuery(userId);
    var cart = await mediator.Send(query, cancellationToken);

    if (cart == null)
        return Results.NotFound();

    return Results.Ok(cart);
})
.WithName("GetCart")
.WithOpenApi()
.Produces<CartDto>()
.Produces(404);

app.MapGet("/api/cart/{userId}/summary", async (IMediator mediator, string userId, CancellationToken cancellationToken) =>
{
    var query = new GetCartSummaryQuery(userId);
    var summary = await mediator.Send(query, cancellationToken);

    if (summary == null)
        return Results.NotFound();

    return Results.Ok(summary);
})
.WithName("GetCartSummary")
.WithOpenApi()
.Produces<CartSummaryDto>()
.Produces(404);

app.Run();
