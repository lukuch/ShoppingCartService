using Mapster;
using MediatR;
using ShoppingCart.Application.DTOs;
using ShoppingCart.Application.Interfaces;

namespace ShoppingCart.Application.Queries.GetCart;

public class GetCartHandler(ICartRepository cartRepository) : IRequestHandler<GetCartQuery, CartDto?>
{
    public async Task<CartDto?> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        var cart = await cartRepository.GetCartAsync(request.UserId, cancellationToken);
        return cart?.Adapt<CartDto>();
    }
}
