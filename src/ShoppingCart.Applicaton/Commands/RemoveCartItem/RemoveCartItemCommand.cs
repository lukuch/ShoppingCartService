using MediatR;

namespace ShoppingCart.Application.Commands.RemoveCartItem;

public record RemoveCartItemCommand(string UserId, int ProductId) : IRequest<Result>;

public record Result(bool Success, string? ErrorMessage = null);
