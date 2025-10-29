namespace ShoppingCart.Application.DTOs;

public record CartItemDto(
    int ProductId,
    string Name,
    decimal UnitPrice,
    int Quantity,
    decimal TotalPrice);
