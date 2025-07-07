using E_Biznes.Application.DTOs.CategoryDtos;
using FluentValidation;

namespace E_Biznes.Application.Validations.CategoryValidations;

public class CategorySubCreateDtoValidator : AbstractValidator<CategorySubCreateDto>
{
    public CategorySubCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Subcategory name is required.")
            .MaximumLength(100).WithMessage("Name must be at most 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must be at most 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.ParentCategoryId)
            .NotNull().WithMessage("Parent category ID is required.")
            .Must(id => id != Guid.Empty).WithMessage("Parent category ID must be valid.");
    }
}
