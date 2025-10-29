using MediatR;
using ShoppingCart.Application.Interfaces;
using ShoppingCart.Domain.Common;
using ShoppingCart.Domain.Entities;

namespace ShoppingCart.Application.Commands.RemoveCartItem;

public class RemoveCartItemHandler(ICartRepository cartRepository) : IRequestHandler<RemoveCartItemCommand, Result>
{
    public async Task<Result> Handle(RemoveCartItemCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var cart = await cartRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (cart == null)
                return Result.Failure("Cart not found");

            cart.RemoveItem(request.ProductId);
            await cartRepository.SaveAsync(cart, cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
