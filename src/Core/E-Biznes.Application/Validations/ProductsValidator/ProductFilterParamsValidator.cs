using E_Biznes.Application.DTOs.ProducDtos;
using FluentValidation;

namespace E_Biznes.Application.Validations.ProductsValidator;

public class ProductFilterParamsValidator : AbstractValidator<ProductFilterParams>
{
    public ProductFilterParamsValidator()
    {
       

        RuleFor(x => x.MinRating)
            .IsInEnum().WithMessage("Invalid minimum rating.");

        RuleFor(x => x.MaxRating)
            .IsInEnum().WithMessage("Invalid maximum rating.");

        RuleFor(x => x)
             .Must(x =>(!x.MinRating.HasValue || !x.MaxRating.HasValue) || x.MaxRating >= x.MinRating)
                    .WithMessage("Max rating must be greater than or equal to min rating.");

        RuleFor(x => x.SortBy)
            .Must(s => string.IsNullOrEmpty(s) || new[] { "price", "name", "rating", "createdDate" }.Contains(s.ToLower()))
            .WithMessage("SortBy must be one of: price, name, rating, createdDate.");

        RuleFor(x => x.SortDirection)
            .Must(d => string.IsNullOrEmpty(d) || new[] { "asc", "desc" }.Contains(d.ToLower()))
            .WithMessage("SortDirection must be 'asc' or 'desc'.");
    }
}
