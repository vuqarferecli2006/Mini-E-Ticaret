using E_Biznes.Application.DTOs.ProducDtos;
using FluentValidation;

namespace E_Biznes.Application.Validations.ProductsValidator;

public class ProductCreateWithImagesDtoValidator : AbstractValidator<ProductCreateWithImagesDto>
{
    public ProductCreateWithImagesDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(100).WithMessage("Name must be at most 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must be at most 1000 characters.");

        RuleFor(x => x.Price)
            .NotEmpty().WithMessage("Product price is required.")
            .GreaterThan(0).WithMessage("Price must be greater than 0.");

        RuleFor(x => x.CategoryId)
            .NotEqual(Guid.Empty).WithMessage("Category ID is required.");

        RuleFor(x => x.Condition)
            .IsInEnum().WithMessage("Invalid product condition.");

        RuleFor(x => x.Stock)
            .NotEmpty().WithMessage("Product Stock count is required.")
            .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative.");

        RuleFor(x => x.Images)
            .NotEmpty().WithMessage("At least one image is required.")
            .Must(images => images.All(i => i.Length > 0)).WithMessage("All images must be non-empty.");
    }
}
