using MediatR;
using ShoppingCart.Application.Interfaces;
using ShoppingCart.Domain.Common;
using ShoppingCart.Domain.Entities;

namespace ShoppingCart.Application.Commands.UpdateCartItem;

public class UpdateCartItemHandler(ICartRepository cartRepository) : IRequestHandler<UpdateCartItemCommand, Result>
{
    public async Task<Result> Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var cart = await cartRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (cart == null)
                return Result.Failure("Cart not found");

            cart.UpdateItemQuantity(request.ProductId, request.Quantity);
            await cartRepository.SaveAsync(cart, cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
