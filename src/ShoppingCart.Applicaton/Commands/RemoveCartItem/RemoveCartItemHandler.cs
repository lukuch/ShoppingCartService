using MediatR;
using ShoppingCart.Application.Interfaces;
using ShoppingCart.Domain.Events;

namespace ShoppingCart.Application.Commands.RemoveCartItem;

public class RemoveCartItemHandler(ICartRepository cartRepository) : IRequestHandler<RemoveCartItemCommand, Result>
{
    public async Task<Result> Handle(RemoveCartItemCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var cart = await cartRepository.GetCartAsync(request.UserId, cancellationToken);
            if (cart == null || !cart.Items.Any(item => item.ProductId == request.ProductId))
                return new Result(false, "Cart item not found");

            var @event = new ItemRemoved(
                request.UserId,
                request.ProductId,
                DateTime.UtcNow
            );

            await cartRepository.SaveEventAsync(@event, cancellationToken);

            return new Result(true);
        }
        catch (Exception ex)
        {
            return new Result(false, ex.Message);
        }
    }
}
