using ShoppingCart.Domain.Common;
using ShoppingCart.Domain.Events;

namespace ShoppingCart.Domain.Entities;

public class Cart : AggregateRoot<string>
{
    private readonly List<CartItem> _items = new();
    
    public IReadOnlyList<CartItem> Items => _items.AsReadOnly();
    public decimal Subtotal => _items.Sum(item => item.TotalPrice);
    public decimal Tax => Subtotal * 0.23m; // 23% VAT
    public decimal Total => Subtotal + Tax;
    public int ItemCount => _items.Sum(item => item.Quantity);

    private Cart() { } // For Marten

    public static Cart Create(string userId)
    {
        return new Cart { Id = userId };
    }

    public void AddItem(int productId, string productName, decimal unitPrice, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        var existingItem = _items.FirstOrDefault(i => i.ProductId == productId);
        if (existingItem != null)
        {
            existingItem.UpdateQuantity(existingItem.Quantity + quantity);
        }
        else
        {
            _items.Add(new CartItem(productId, productName, unitPrice, quantity));
        }

        AddDomainEvent(new ItemAdded(Id, productId, productName, unitPrice, quantity));
    }

    public void UpdateItemQuantity(int productId, int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(newQuantity));

        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item == null)
            throw new InvalidOperationException($"Item with ProductId {productId} not found in cart");

        item.UpdateQuantity(newQuantity);
        AddDomainEvent(new ItemQuantityUpdated(Id, productId, newQuantity));
    }

    public void RemoveItem(int productId)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item == null)
            throw new InvalidOperationException($"Item with ProductId {productId} not found in cart");

        _items.Remove(item);
        AddDomainEvent(new ItemRemoved(Id, productId));
    }

    // Marten event handling methods
    public void Apply(ItemAdded @event)
    {
        var existingItem = _items.FirstOrDefault(i => i.ProductId == @event.ProductId);
        if (existingItem != null)
        {
            existingItem.UpdateQuantity(existingItem.Quantity + @event.Quantity);
        }
        else
        {
            _items.Add(new CartItem(@event.ProductId, @event.Name, @event.UnitPrice, @event.Quantity));
        }
    }

    public void Apply(ItemQuantityUpdated @event)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == @event.ProductId);
        if (item != null)
        {
            item.UpdateQuantity(@event.NewQuantity);
        }
    }

    public void Apply(ItemRemoved @event)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == @event.ProductId);
        if (item != null)
        {
            _items.Remove(item);
        }
    }
}
