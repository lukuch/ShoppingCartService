namespace ShoppingCart.Domain.Entities;

public class Cart
{
    public required string Id { get; init; }
    public List<CartItem> Items { get; init; } = [];

    public decimal Subtotal => Items.Sum(item => item.TotalPrice);
    public decimal Tax => Subtotal * 0.23m; // 23% VAT
    public decimal Total => Subtotal + Tax;
    public int ItemCount => Items.Sum(item => item.Quantity);
}
