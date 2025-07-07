using E_Biznes.Application.DTOs.CategoryDtos;
using FluentValidation;

namespace E_Biznes.Application.Validations.CategoryValidations;

public class CategoryMainCreateDtoValidator : AbstractValidator<CategoryMainCreateDto>
{
    public CategoryMainCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Main category name is required.")
            .MaximumLength(100).WithMessage("Name must be at most 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must be at most 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}
