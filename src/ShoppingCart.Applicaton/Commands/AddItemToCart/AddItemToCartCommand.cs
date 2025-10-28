using MediatR;

namespace ShoppingCart.Application.Commands.AddItemToCart;

public record AddItemToCartCommand(string UserId, int ProductId, int Quantity) : IRequest<Result>;

public record Result(bool Success, string? ErrorMessage = null);
