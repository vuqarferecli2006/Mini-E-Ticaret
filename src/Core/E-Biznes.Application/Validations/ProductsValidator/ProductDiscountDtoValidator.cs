using E_Biznes.Application.DTOs.ProducDtos;
using FluentValidation;

namespace E_Biznes.Application.Validations.ProductsValidator;

public class ProductDiscountDtoValidator : AbstractValidator<ProductDiscountDto>
{
    public ProductDiscountDtoValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("ProductId mustn't be empty");

        RuleFor(x => x.Discount)
            .InclusiveBetween(0, 100).WithMessage("Discount must be between 0 and 100");
    }
}
