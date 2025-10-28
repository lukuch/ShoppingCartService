using MediatR;
using ShoppingCart.Application.DTOs;

namespace ShoppingCart.Application.Queries.GetProducts;

public record GetProductsQuery : IRequest<List<ProductDto>>;
