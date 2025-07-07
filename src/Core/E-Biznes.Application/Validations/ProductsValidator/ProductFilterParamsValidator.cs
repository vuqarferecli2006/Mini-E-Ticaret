using E_Biznes.Application.DTOs.ProducDtos;
using FluentValidation;

namespace E_Biznes.Application.Validations.ProductsValidator;

public class ProductFilterParamsValidator : AbstractValidator<ProductFilterParams>
{
    public ProductFilterParamsValidator()
    {
        When(x => x.MinPrice.HasValue, () =>
        {
            RuleFor(x => x.MinPrice.Value)
                .GreaterThanOrEqualTo(0).WithMessage("Min price must be at least 0.");
        });

        When(x => x.MaxPrice.HasValue, () =>
        {
            RuleFor(x => x.MaxPrice.Value)
                .GreaterThan(0).WithMessage("Max price must be greater than 0.");
        });

        RuleFor(x => x)
            .Must(x => !x.MinPrice.HasValue || !x.MaxPrice.HasValue || x.MaxPrice >= x.MinPrice)
            .WithMessage("Max price must be greater than or equal to min price.");

        RuleFor(x => x.MinRating)
            .IsInEnum().WithMessage("Invalid minimum rating.");

        RuleFor(x => x.MaxRating)
            .IsInEnum().WithMessage("Invalid maximum rating.");

        RuleFor(x => x)
            .Must(x => x.MaxRating >= x.MinRating)
            .WithMessage("Max rating must be greater than or equal to min rating.");

        RuleFor(x => x.SortBy)
            .Must(s => string.IsNullOrEmpty(s) || new[] { "price", "name", "rating", "createdDate" }.Contains(s.ToLower()))
            .WithMessage("SortBy must be one of: price, name, rating, createdDate.");

        RuleFor(x => x.SortDirection)
            .Must(d => string.IsNullOrEmpty(d) || new[] { "asc", "desc" }.Contains(d.ToLower()))
            .WithMessage("SortDirection must be 'asc' or 'desc'.");
    }
}
