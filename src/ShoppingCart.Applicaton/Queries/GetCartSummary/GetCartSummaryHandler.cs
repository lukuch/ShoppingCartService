using Mapster;
using MediatR;
using ShoppingCart.Application.DTOs;
using ShoppingCart.Application.Interfaces;

namespace ShoppingCart.Application.Queries.GetCartSummary;

public class GetCartSummaryHandler(ICartRepository cartRepository) : IRequestHandler<GetCartSummaryQuery, CartSummaryDto?>
{
    public async Task<CartSummaryDto?> Handle(GetCartSummaryQuery request, CancellationToken cancellationToken)
    {
        var cart = await cartRepository.GetByIdAsync(request.UserId, cancellationToken);
        return cart?.Adapt<CartSummaryDto>();
    }
}
