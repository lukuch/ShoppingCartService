using MediatR;
using ShoppingCart.Domain.Common;

namespace ShoppingCart.Application.Commands.UpdateCartItem;

public record UpdateCartItemCommand(string UserId, int ProductId, int Quantity) : IRequest<Result>;
