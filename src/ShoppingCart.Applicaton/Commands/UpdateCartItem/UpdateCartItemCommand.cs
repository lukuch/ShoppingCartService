using MediatR;

namespace ShoppingCart.Application.Commands.UpdateCartItem;

public record UpdateCartItemCommand(string UserId, int ProductId, int Quantity) : IRequest<Result>;

public record Result(bool Success, string? ErrorMessage = null);
