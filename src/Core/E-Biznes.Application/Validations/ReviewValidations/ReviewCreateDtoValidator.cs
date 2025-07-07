using E_Biznes.Application.DTOs.ReviewDtos;
using FluentValidation;

namespace E_Biznes.Application.Validations.ReviewValidations;

public class ReviewCreateDtoValidator : AbstractValidator<ReviewCreateDto>
{
    public ReviewCreateDtoValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Review content is required.")
            .MaximumLength(1000).WithMessage("Review content cannot exceed 1000 characters.");

        RuleFor(x => x.Rating)
            .IsInEnum().WithMessage("Invalid rating value.");
    }
}
