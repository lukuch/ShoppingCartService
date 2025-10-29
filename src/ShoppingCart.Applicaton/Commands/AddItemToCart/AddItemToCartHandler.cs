using MediatR;
using ShoppingCart.Application.Interfaces;
using ShoppingCart.Domain.Common;
using ShoppingCart.Domain.Entities;

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
                return Result.Failure($"Product with ID {request.ProductId} not found");

            // Get or create cart
            var cart = await cartRepository.GetByIdAsync(request.UserId, cancellationToken) 
                      ?? Cart.Create(request.UserId);

            // Use domain method
            cart.AddItem(product.Id, product.Title, product.Price, request.Quantity);

            // Save aggregate
            await cartRepository.SaveAsync(cart, cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
