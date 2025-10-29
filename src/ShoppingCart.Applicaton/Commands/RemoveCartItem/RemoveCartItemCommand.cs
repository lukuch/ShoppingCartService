using MediatR;
using ShoppingCart.Domain.Common;

namespace ShoppingCart.Application.Commands.RemoveCartItem;

public record RemoveCartItemCommand(string UserId, int ProductId) : IRequest<Result>;
