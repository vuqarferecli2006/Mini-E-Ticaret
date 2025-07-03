using E_Biznes.Application.DTOs.CategoryDtos;
using FluentValidation;

namespace E_Biznes.Application.Validations.CategoryValidations;

public class CategoryUpdateValidator : AbstractValidator<CategoryUpdateDto>
{

    public CategoryUpdateValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id boş ola bilməz.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name boş ola bilməz.")
            .MaximumLength(100).WithMessage("Name maksimum 100 simvol ola bilər.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description maksimum 500 simvol ola bilər.");

        RuleFor(x => x.ParentCategoryId)
            .Must(x => x == null || x != Guid.Empty)
            .WithMessage("ParentCategoryId düzgün formatda olmalıdır.");
    }
}
