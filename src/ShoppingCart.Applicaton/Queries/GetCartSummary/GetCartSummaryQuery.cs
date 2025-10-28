using MediatR;
using ShoppingCart.Application.DTOs;

namespace ShoppingCart.Application.Queries.GetCartSummary;

public record GetCartSummaryQuery(string UserId) : IRequest<CartSummaryDto?>;
