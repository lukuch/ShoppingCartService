using ShoppingCart.Domain.Entities;
using Xunit;

namespace ShoppingCart.Domain.Tests
{
    public class CartItemTests
    {
        [Fact]
        public void CartItem_TotalPrice_CalculatesCorrectly()
        {
            var item = new CartItem(1, "Banana", 2.5m, 4);
            Assert.Equal(10.0m, item.TotalPrice);
        }

        [Fact]
        public void UpdateQuantity_Propagates_NewValue()
        {
            var item = new CartItem(1, "Banana", 2m, 2);
            item.UpdateQuantity(8);
            Assert.Equal(8, item.Quantity);
        }
    }
}
