namespace ShoppingCart.Application.DTOs;

public record CartDto(
    string UserId,
    List<CartItemDto> Items,
    int ItemCount,
    decimal Subtotal,
    decimal Tax,
    decimal Total);
