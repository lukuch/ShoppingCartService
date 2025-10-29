namespace ShoppingCart.Domain.Entities;

public class CartItem
{
    public int ProductId { get; }
    public string Name { get; }
    public decimal UnitPrice { get; }
    public int Quantity { get; private set; }

    public decimal TotalPrice => UnitPrice * Quantity;

    public CartItem(int productId, string name, decimal unitPrice, int quantity)
    {
        ProductId = productId;
        Name = name;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    public void UpdateQuantity(int newQuantity)
    {
        Quantity = newQuantity;
    }
}
