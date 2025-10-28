using MediatR;
using ShoppingCart.Application.DTOs;

namespace ShoppingCart.Application.Queries.GetCart;

public record GetCartQuery(string UserId) : IRequest<CartDto?>;
