namespace ShoppingCart.Domain.Entities;

public record CartItem
{
    public required int ProductId { get; init; }
    public required string Name { get; init; }
    public required decimal UnitPrice { get; init; }
    public required int Quantity { get; set; }

    public decimal TotalPrice => UnitPrice * Quantity;
}
