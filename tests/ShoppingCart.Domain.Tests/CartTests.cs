using ShoppingCart.Domain.Entities;
using System;
using Xunit;

namespace ShoppingCart.Domain.Tests
{
    public class CartTests
    {
        [Fact]
        public void CreateCart_ShouldInitializeWithNoItems()
        {
            var cart = Cart.Create("user1");
            Assert.Empty(cart.Items);
            Assert.Equal(0, cart.Subtotal);
            Assert.Equal(0, cart.Total);
            Assert.Equal(0, cart.ItemCount);
        }

        [Fact]
        public void AddItem_ShouldAddCartItem()
        {
            var cart = Cart.Create("user1");
            cart.AddItem(1, "Apple", 1.0m, 2);
            Assert.Single(cart.Items);
            Assert.Equal(2, cart.ItemCount);
            Assert.Equal(2.0m, cart.Subtotal);
            Assert.Equal(2.0m * 1.23m, cart.Total, 2);
        }

        [Fact]
        public void AddItem_ExceedMaxQuantity_Throws()
        {
            var cart = Cart.Create("user1");
            Assert.Throws<ArgumentException>(() => cart.AddItem(1, "Apple", 1.0m, 100));
        }

        [Fact]
        public void AddItem_NegativeQuantity_Throws()
        {
            var cart = Cart.Create("user1");
            Assert.Throws<ArgumentException>(() => cart.AddItem(1, "Apple", 1.0m, -1));
        }

        [Fact]
        public void UpdateItemQuantity_Valid_ShouldUpdate()
        {
            var cart = Cart.Create("user1");
            cart.AddItem(1, "Apple", 1.0m, 2);
            cart.UpdateItemQuantity(1, 5);
            Assert.Equal(5, cart.Items[0].Quantity);
        }

        [Fact]
        public void UpdateItemQuantity_Invalid_Throws()
        {
            var cart = Cart.Create("user1");
            cart.AddItem(1, "Apple", 1.0m, 2);
            Assert.Throws<ArgumentException>(() => cart.UpdateItemQuantity(1, 0));
        }

        [Fact]
        public void RemoveItem_ShouldRemove()
        {
            var cart = Cart.Create("user1");
            cart.AddItem(1, "Apple", 1.0m, 2);
            cart.RemoveItem(1);
            Assert.Empty(cart.Items);
        }

        [Fact]
        public void RemoveItem_NotExists_Throws()
        {
            var cart = Cart.Create("user1");
            Assert.Throws<InvalidOperationException>(() => cart.RemoveItem(99));
        }
    }
}
