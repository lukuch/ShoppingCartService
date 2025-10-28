using FluentValidation;

namespace ShoppingCart.Application.Commands.AddItemToCart;

public class AddItemToCartValidator : AbstractValidator<AddItemToCartCommand>
{
    public AddItemToCartValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required");
        RuleFor(x => x.ProductId).GreaterThan(0).WithMessage("ProductId must be greater than 0");
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than 0");
    }
}
