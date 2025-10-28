using MediatR;
using ShoppingCart.Application.Interfaces;
using ShoppingCart.Domain.Events;

namespace ShoppingCart.Application.Commands.AddItemToCart;

public class AddItemToCartHandler(
    ICartRepository cartRepository,
    IProductService productService) : IRequestHandler<AddItemToCartCommand, Result>
{
    public async Task<Result> Handle(AddItemToCartCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get product details
            var product = await productService.GetProductByIdAsync(request.ProductId, cancellationToken);
            if (product == null)
                return new Result(false, $"Product with ID {request.ProductId} not found");

            // Create event
            var @event = new ItemAdded(
                request.UserId,
                product.Id,
                product.Title,
                product.Price,
                request.Quantity,
                DateTime.UtcNow
            );

            // Save event
            await cartRepository.SaveEventAsync(@event, cancellationToken);

            return new Result(true);
        }
        catch (Exception ex)
        {
            return new Result(false, ex.Message);
        }
    }
}
