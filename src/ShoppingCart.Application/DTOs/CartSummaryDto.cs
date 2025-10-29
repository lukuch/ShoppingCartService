namespace ShoppingCart.Application.DTOs;

public record CartSummaryDto(
    string UserId,
    int ItemCount,
    decimal Subtotal,
    decimal Tax,
    decimal Total);
