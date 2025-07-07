using E_Biznes.Application.DTOs.OrderDtos;
using FluentValidation;

namespace E_Biznes.Application.Validations.OrderValidations;

public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderDtoValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEqual(Guid.Empty).WithMessage("Product ID must be valid.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0.");
    }
}
