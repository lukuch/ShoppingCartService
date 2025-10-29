using FluentValidation;

namespace ShoppingCart.Application.Commands.RemoveCartItem;

public class RemoveCartItemValidator : AbstractValidator<RemoveCartItemCommand>
{
    public RemoveCartItemValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required");
        RuleFor(x => x.ProductId).GreaterThan(0).WithMessage("ProductId must be greater than 0");
    }
}
