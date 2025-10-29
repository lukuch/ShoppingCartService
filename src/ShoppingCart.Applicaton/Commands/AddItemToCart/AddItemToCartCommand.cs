using MediatR;
using ShoppingCart.Domain.Common;

namespace ShoppingCart.Application.Commands.AddItemToCart;

public record AddItemToCartCommand(string UserId, int ProductId, int Quantity) : IRequest<Result>;
