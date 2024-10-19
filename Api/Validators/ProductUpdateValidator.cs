using Application.Products;
using FluentValidation;

namespace Api.Validators;

public class ProductUpdateValidator : AbstractValidator<ProductUpdateCommand>
{
    private const int MIN_LENGTH = 8;

    public ProductUpdateValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MinimumLength(MIN_LENGTH).WithMessage($"La longitud del nombre del producto debe ser mayor a {MIN_LENGTH}");
        RuleFor(x => x.Price).NotEmpty().WithMessage("El precio no puede ser vació.")
            .GreaterThanOrEqualTo(1).WithMessage("El precio del producto debe ser mayor o igual a 1.");
    }
}
