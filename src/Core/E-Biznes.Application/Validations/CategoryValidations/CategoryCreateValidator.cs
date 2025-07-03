using E_Biznes.Application.DTOs.CategoryDtos;
using FluentValidation;

namespace E_Biznes.Application.Validations.CategoryValidations;

public class CategoryCreateValidator:AbstractValidator<CategoryMainCreateDto>
{
    public CategoryCreateValidator()
    {
        RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name boş ola bilməz.")
                .MaximumLength(100).WithMessage("Name maksimum 100 simvol ola bilər.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description maksimum 500 simvol ola bilər.");

    }
}
