using E_Biznes.Application.DTOs.ProducDtos;
using FluentValidation;

namespace E_Biznes.Application.Validations.ProductsValidator;

public class ProductUpdateWithImagesDtoValidator : AbstractValidator<ProductUpdateWithImagesDto>
{
    public ProductUpdateWithImagesDtoValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEqual(Guid.Empty).WithMessage("Product ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(100).WithMessage("Name must be at most 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must be at most 1000 characters.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0.");

        RuleFor(x => x.CategoryId)
            .NotEqual(Guid.Empty).WithMessage("Category ID is required.");

        RuleFor(x => x.Condition)
            .IsInEnum().WithMessage("Invalid product condition.");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative.");

        RuleForEach(x => x.Images)
            .Must(i => i.Length > 0).WithMessage("All images must be non-empty.");
    }
}
