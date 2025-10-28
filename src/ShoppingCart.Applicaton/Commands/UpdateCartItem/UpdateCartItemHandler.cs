using MediatR;
using ShoppingCart.Application.Interfaces;
using ShoppingCart.Domain.Events;

namespace ShoppingCart.Application.Commands.UpdateCartItem;

public class UpdateCartItemHandler(ICartRepository cartRepository) : IRequestHandler<UpdateCartItemCommand, Result>
{
    public async Task<Result> Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var cart = await cartRepository.GetCartAsync(request.UserId, cancellationToken);
            if (cart == null || !cart.Items.Any(item => item.ProductId == request.ProductId))
                return new Result(false, "Cart item not found");

            var @event = new ItemQuantityUpdated(
                request.UserId,
                request.ProductId,
                request.Quantity,
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
