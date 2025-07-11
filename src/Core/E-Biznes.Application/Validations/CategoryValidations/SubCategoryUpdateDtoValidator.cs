﻿using E_Biznes.Application.DTOs.CategoryDtos;
using FluentValidation;

namespace E_Biznes.Application.Validations.CategoryValidations;

public class SubCategoryUpdateDtoValidator : AbstractValidator<CategorySubUpdateDto>
{
    public SubCategoryUpdateDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty).WithMessage("Subcategory ID must be valid.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must be at most 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must be at most 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.NewParentCategoryId)
            .NotEqual(Guid.Empty).WithMessage("New parent category ID must be valid.");

    }
}
